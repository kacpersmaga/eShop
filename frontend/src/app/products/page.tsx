"use client";

import { useState, useEffect, useCallback } from "react";
import Header from "@/components/layout/header";
import Footer from "@/components/layout/footer";
import ProductCard from "@/components/ui/product-card";
import { Search, SlidersHorizontal, X, Loader2, ImageIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import catalogService, { Product } from "@/services/catalog-service";

interface ApiError {
  message: string;
  status?: number;
  data?: unknown;
}

const fallbackProducts = [
  {
    id: 1,
    name: "Premium Leather Wallet",
    price: 89.99,
    image: "",
    category: "Accessories",
    description: "Handcrafted premium leather wallet with multiple card slots and coin pocket.",
    createdAt: new Date().toISOString(),
  },
  {
    id: 2,
    name: "Wireless Noise-Cancelling Headphones",
    price: 249.99,
    image: "",
    category: "Electronics",
    description: "High-quality wireless headphones with active noise cancellation.",
    createdAt: new Date().toISOString(),
  },
  {
    id: 3,
    name: "Organic Cotton T-Shirt",
    price: 34.99,
    image: "",
    category: "Clothing",
    description: "Soft, comfortable t-shirt made from 100% organic cotton.",
    createdAt: new Date().toISOString(),
  },
  {
    id: 4,
    name: "Stainless Steel Water Bottle",
    price: 29.99,
    image: "",
    category: "Home",
    description: "Double-walled insulated water bottle that keeps drinks cold for 24 hours.",
    createdAt: new Date().toISOString(),
  },
];

export default function ProductsPage() {
  const [products, setProducts] = useState<Product[]>([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [priceRange, setPriceRange] = useState({ min: 0, max: 300 });
  const [sortBy, setSortBy] = useState("newest");
  const [isFilterOpen, setIsFilterOpen] = useState(false);
  const [sortDropdownOpen, setSortDropdownOpen] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [useFallback, setUseFallback] = useState(false);
  
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(8);
  const [totalPages, setTotalPages] = useState(1);

  const getSortParams = () => {
    switch(sortBy) {
      case "newest":
        return { sortBy: "createdAt", ascending: false };
      case "price-asc":
        return { sortBy: "price", ascending: true };
      case "price-desc":
        return { sortBy: "price", ascending: false };
      default:
        return { sortBy: "createdAt", ascending: false };
    }
  };

  const loadProducts = useCallback(async () => {
    if (useFallback) {
      let filteredProducts = [...fallbackProducts];
      
      if (searchTerm) {
        filteredProducts = filteredProducts.filter(product => 
          product.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
          (product.description && product.description.toLowerCase().includes(searchTerm.toLowerCase()))
        );
      }
      
      filteredProducts = filteredProducts.filter(product => 
        product.price >= priceRange.min && product.price <= priceRange.max
      );
      
      switch(sortBy) {
        case "newest":
          filteredProducts.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
          break;
        case "price-asc":
          filteredProducts.sort((a, b) => a.price - b.price);
          break;
        case "price-desc":
          filteredProducts.sort((a, b) => b.price - a.price);
          break;
      }
      
      setProducts(filteredProducts);
      setTotalPages(Math.ceil(filteredProducts.length / pageSize));
      setIsLoading(false);
      return;
    }
    
    setIsLoading(true);
    setError(null);
    
    try {
      let result;
      const { sortBy: backendSortBy, ascending } = getSortParams();
      
      if (searchTerm) {
        result = await catalogService.searchProducts(searchTerm);
        if (result.success) {
          const productItems = result.data?.items || [];
          setProducts(Array.isArray(productItems) ? productItems : []);
          setTotalPages(result.data?.totalPages || 1);
        } else {
          throw new Error(result.errors?.join(', ') || 'Failed to search products');
        }
      } 
      else if (priceRange.min > 0 || priceRange.max < 300) {
        result = await catalogService.getProductsByPriceRange(priceRange.min, priceRange.max);
        if (result.success) {
          const productItems = result.data?.items || [];
          setProducts(Array.isArray(productItems) ? productItems : []);
          setTotalPages(result.data?.totalPages || 1);
        } else {
          throw new Error(result.errors?.join(', ') || 'Failed to filter products by price');
        }
      } 
      else {
        result = await catalogService.getProducts(
          currentPage, 
          pageSize, 
          undefined,
          backendSortBy, 
          ascending
        );
        
        if (result.success) {
          const productItems = result.data?.items || [];
          setProducts(Array.isArray(productItems) ? productItems : []);
          setTotalPages(result.data?.totalPages || 1);
        } else {
          throw new Error(result.errors?.join(', ') || 'Failed to fetch products');
        }
      }
    } catch (err: unknown) {
      console.error('Error loading products:', err);
      const apiError = err as ApiError;
      setError(apiError.message || 'Failed to load products');
      
      if (products.length === 0) {
        console.warn('Using fallback product data due to API error');
        setUseFallback(true);
        setTimeout(() => loadProducts(), 0);
      }
    } finally {
      setIsLoading(false);
    }
  }, [searchTerm, priceRange.min, priceRange.max, sortBy, currentPage, pageSize, useFallback, products.length]);

  useEffect(() => {
    loadProducts();
  }, [loadProducts]);

  const handlePriceRangeChange = (type: 'min' | 'max', value: number) => {
    setPriceRange(prev => ({
      ...prev,
      [type]: value
    }));
  };

  const handleSortSelection = (value: string) => {
    setSortBy(value);
    setSortDropdownOpen(false);
  };

  const handlePageChange = (page: number) => {
    setCurrentPage(page);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  };

  const getSortDisplayText = () => {
    switch(sortBy) {
      case "newest": return "Newest";
      case "price-asc": return "Price: Low to High";
      case "price-desc": return "Price: High to Low";
      default: return "Newest";
    }
  };

  return (
    <>
      <Header />
      <main className="pt-20 pb-16 px-4 bg-background text-foreground">
        <div className="container mx-auto">
          <h1 className="text-2xl sm:text-3xl font-bold mb-6 sm:mb-8 text-foreground">All Products</h1>
          
          {useFallback && (
            <div className="mb-4 p-4 bg-yellow-100 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-200 rounded-lg">
              <p className="font-medium">⚠️ Using offline catalog mode</p>
              <p className="text-sm">Unable to connect to the product service. Showing limited functionality.</p>
            </div>
          )}
          
          {/* Search and Filter Controls */}
          <div className="flex flex-col gap-4 mb-6 sm:mb-8">
            <div className="relative w-full">
              <input
                type="text"
                placeholder="Search products..."
                className="w-full pl-10 pr-4 py-2 border border-input rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-5 w-5" />
            </div>
            
            <div className="flex flex-col sm:flex-row gap-3 w-full">
              <Button 
                onClick={() => setIsFilterOpen(!isFilterOpen)}
                variant="outline"
                className="flex items-center justify-center gap-2 w-full h-10 sm:w-auto bg-background text-foreground border-input"
              >
                {isFilterOpen ? (
                  <>
                    <X className="h-4 w-4" />
                    <span>Close Filters</span>
                  </>
                ) : (
                  <>
                    <SlidersHorizontal className="h-4 w-4" />
                    <span>Filters</span>
                  </>
                )}
              </Button>
              
              {/* Custom dropdown for sort options */}
              <div className="w-full sm:w-auto relative">
                <button
                  type="button"
                  onClick={() => setSortDropdownOpen(!sortDropdownOpen)}
                  className="w-full h-10 px-4 py-2 rounded-md border border-input bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary flex items-center justify-between"
                >
                  <span>{getSortDisplayText()}</span>
                  <svg className="h-5 w-5 text-muted-foreground" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 20 20" fill="currentColor">
                    <path fillRule="evenodd" d="M5.293 7.293a1 1 0 011.414 0L10 10.586l3.293-3.293a1 1 0 111.414 1.414l-4 4a1 1 0 01-1.414 0l-4-4a1 1 0 010-1.414z" clipRule="evenodd" />
                  </svg>
                </button>
                
                {sortDropdownOpen && (
                  <div className="absolute z-10 mt-1 w-64 rounded-md border border-input bg-background text-foreground shadow-lg">
                    <ul className="py-1">
                      <li>
                        <button
                          type="button"
                          className={`w-full text-left px-4 py-2 hover:bg-accent truncate ${sortBy === 'newest' ? 'bg-primary/10 text-primary' : ''}`}
                          onClick={() => handleSortSelection('newest')}
                        >
                          Newest
                        </button>
                      </li>
                      <li>
                        <button
                          type="button"
                          className={`w-full text-left px-4 py-2 hover:bg-accent truncate ${sortBy === 'price-asc' ? 'bg-primary/10 text-primary' : ''}`}
                          onClick={() => handleSortSelection('price-asc')}
                        >
                          Price: Low to High
                        </button>
                      </li>
                      <li>
                        <button
                          type="button"
                          className={`w-full text-left px-4 py-2 hover:bg-accent truncate ${sortBy === 'price-desc' ? 'bg-primary/10 text-primary' : ''}`}
                          onClick={() => handleSortSelection('price-desc')}
                        >
                          Price: High to Low
                        </button>
                      </li>
                    </ul>
                  </div>
                )}
              </div>
            </div>
          </div>
          
          {/* Filter Panel */}
          {isFilterOpen && (
            <div className="bg-card text-card-foreground p-4 sm:p-6 rounded-lg shadow-md mb-6 border border-border">
              <h3 className="text-lg font-medium mb-4">Price Range</h3>
              <div className="flex flex-col gap-4">
                <div className="grid grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Min ($)</label>
                    <input
                      type="number"
                      min="0"
                      max={priceRange.max}
                      value={priceRange.min}
                      onChange={(e) => handlePriceRangeChange('min', Number(e.target.value))}
                      className="w-full px-3 py-2 border border-input rounded-lg bg-background text-foreground"
                    />
                  </div>
                  <div>
                    <label className="block text-sm text-muted-foreground mb-1">Max ($)</label>
                    <input
                      type="number"
                      min={priceRange.min}
                      value={priceRange.max}
                      onChange={(e) => handlePriceRangeChange('max', Number(e.target.value))}
                      className="w-full px-3 py-2 border border-input rounded-lg bg-background text-foreground"
                    />
                  </div>
                </div>
                <div className="flex flex-col sm:flex-row sm:items-center gap-2 sm:gap-0">
                  <span className="text-sm text-muted-foreground w-12">${priceRange.min}</span>
                  <div className="flex-1 mx-0 sm:mx-4">
                    <input
                      type="range"
                      min="0"
                      max="300"
                      value={priceRange.max}
                      onChange={(e) => handlePriceRangeChange('max', Number(e.target.value))}
                      className="w-full accent-primary"
                    />
                  </div>
                  <span className="text-sm text-muted-foreground w-12 text-right">${priceRange.max}</span>
                </div>
              </div>
            </div>
          )}
          
          {/* Loading State */}
          {isLoading && (
            <div className="flex justify-center items-center py-12">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <span className="ml-2 text-lg text-muted-foreground">Loading products...</span>
            </div>
          )}
          
          {/* Error State */}
          {error && !useFallback && (
            <div className="text-center py-12 bg-destructive/10 text-destructive rounded-lg shadow-sm border border-destructive/30">
              <p className="text-xl mb-4">Error loading products</p>
              <p className="text-muted-foreground">{error}</p>
              <Button 
                onClick={() => loadProducts()}
                className="mt-4 bg-primary text-primary-foreground hover:bg-primary/90"
              >
                Try Again
              </Button>
            </div>
          )}
          
          {/* Products Grid */}
          {!isLoading && (
            <>
              {products && products.length > 0 ? (
                <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 sm:gap-6">
                  {products.map((product) => (
                    <ProductCard key={product.id} product={product} />
                  ))}
                </div>
              ) : (
                <div className="text-center py-12 bg-card text-card-foreground rounded-lg shadow-sm border border-border">
                  <div className="flex flex-col items-center">
                    <ImageIcon className="h-12 w-12 mb-4 text-muted-foreground" />
                    <p className="text-xl text-muted-foreground mb-4">No products found matching your criteria.</p>
                    <Button 
                      onClick={() => {
                        setSearchTerm("");
                        setPriceRange({ min: 0, max: 300 });
                        setSortBy("newest");
                        setCurrentPage(1);
                      }}
                      className="mt-4 bg-primary text-primary-foreground hover:bg-primary/90"
                    >
                      Reset Filters
                    </Button>
                  </div>
                </div>
              )}
            </>
          )}
          
          {/* Pagination Controls */}
          {!isLoading && products && products.length > 0 && totalPages > 1 && (
            <div className="flex justify-center mt-8">
              <nav className="flex items-center gap-1 text-sm">
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => handlePageChange(Math.max(1, currentPage - 1))}
                  disabled={currentPage === 1}
                  className="h-8 border border-input hover:bg-accent hover:text-accent-foreground"
                >
                  Previous
                </Button>
                
                {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
                  <Button
                    key={page}
                    variant={page === currentPage ? "default" : "outline"}
                    size="sm"
                    onClick={() => handlePageChange(page)}
                    className={`h-8 w-8 p-0 border border-input ${
                      page === currentPage 
                        ? 'bg-primary text-primary-foreground' 
                        : 'hover:bg-accent hover:text-accent-foreground'
                    }`}
                  >
                    {page}
                  </Button>
                ))}
                
                <Button
                  variant="outline"
                  size="sm"
                  onClick={() => handlePageChange(Math.min(totalPages, currentPage + 1))}
                  disabled={currentPage === totalPages}
                  className="h-8 border border-input hover:bg-accent hover:text-accent-foreground"
                >
                  Next
                </Button>
              </nav>
            </div>
          )}
        </div>
      </main>
      <Footer />
    </>
  );
}