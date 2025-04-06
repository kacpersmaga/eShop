"use client";

import { useState, useEffect, useMemo } from "react";
import { useRouter } from "next/navigation";
import Header from "@/components/layout/header";
import Footer from "@/components/layout/footer";
import { Plus, Pencil, Trash2, AlertCircle, Search, Loader2 } from "lucide-react";
import AdminProductForm from "@/components/admin/product-form";
import { Button } from "@/components/ui/button";
import catalogService, { Product } from "@/services/catalog-service";
import { useToast } from "@/components/ui/toast";
import Image from "next/image";

type ModalState = {
  isOpen: boolean;
  type: 'create' | 'edit' | 'delete';
  product?: Product;
};

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
  }
];

export default function AdminPage() {
  const router = useRouter();
  const [products, setProducts] = useState<Product[]>([]);
  const [modal, setModal] = useState<ModalState>({
    isOpen: false,
    type: 'create',
  });
  const [searchTerm, setSearchTerm] = useState("");
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [useFallback, setUseFallback] = useState(false);
  const { open: showToast } = useToast();

  const loadProducts = async () => {
    setIsLoading(true);
    setError(null);
    
    try {
      if (useFallback) {
        setProducts(fallbackProducts);
        setIsLoading(false);
        return;
      }
      
      const result = await catalogService.getProducts();
      if (result.success) {
        const productItems = result.data?.items || [];
        setProducts(Array.isArray(productItems) ? productItems : []);
      } else {
        throw new Error(result.errors?.join(', ') || 'Failed to load products');
      }
    } catch (err: unknown) {
      const apiError = err as ApiError;
      setError(apiError.message || 'Failed to load products');
      console.error('Error loading products:', err);
      
      if (products.length === 0) {
        console.warn('Using fallback product data due to API error');
        setUseFallback(true);
        setProducts(fallbackProducts);
      }
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    loadProducts();
  }, []);

  const filteredProducts = useMemo(() => {
    if (!Array.isArray(products)) {
      console.warn('Products is not an array:', products);
      return [];
    }
    
    if (!searchTerm) return products;
    
    return products.filter(product => 
      product.name?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.category?.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [products, searchTerm]);

  const handleCreateProduct = async (formData: Omit<Product, 'id' | 'createdAt'> & { imageFile?: File }) => {
    try {
      if (useFallback) {
        const newProduct: Product = {
          id: Math.max(0, ...products.map(p => p.id)) + 1,
          name: formData.name,
          price: formData.price,
          description: formData.description,
          category: formData.category,
          image: "",
          createdAt: new Date().toISOString()
        };
        
        setProducts([...products, newProduct]);
        showToast({
          title: "Product Created (Offline Mode)",
          description: "The product has been added to the local catalog.",
          variant: "default",
        });
        
        setModal({ isOpen: false, type: 'create' });
        return;
      }
      
      const productDto = {
        name: formData.name,
        price: formData.price,
        description: formData.description,
        category: formData.category,
        image: formData.imageFile
      };
      
      const result = await catalogService.createProduct(productDto);
      
      if (result.success) {
        showToast({
          title: "Product Created",
          description: "The product has been successfully created.",
          variant: "default",
        });
        
        await loadProducts();
        
        setModal({ isOpen: false, type: 'create' });
      } else {
        showToast({
          title: "Error",
          description: result.errors?.join(', ') || "An unknown error occurred",
          variant: "destructive",
        });
      }
    } catch (err: unknown) {
      const apiError = err as ApiError;
      showToast({
        title: "Error",
        description: apiError.message || "Failed to create product",
        variant: "destructive",
      });
    }
  };

  const handleUpdateProduct = async (formData: Omit<Product, 'id' | 'createdAt'> & { imageFile?: File }) => {
    if (!modal.product) return;
    
    try {
      if (useFallback) {
        const updatedProducts = products.map(p => 
          p.id === modal.product?.id 
            ? { 
                ...p, 
                name: formData.name,
                price: formData.price,
                description: formData.description,
                category: formData.category
              } 
            : p
        );
        
        setProducts(updatedProducts);
        showToast({
          title: "Product Updated (Offline Mode)",
          description: "The product has been updated in the local catalog.",
          variant: "default",
        });
        
        setModal({ isOpen: false, type: 'edit' });
        return;
      }
      
      const productDto = {
        name: formData.name,
        price: formData.price,
        description: formData.description,
        category: formData.category,
        image: formData.imageFile
      };
      
      const result = await catalogService.updateProduct(modal.product.id, productDto);
      
      if (result.success) {
        showToast({
          title: "Product Updated",
          description: "The product has been successfully updated.",
          variant: "default",
        });
        
        await loadProducts();
        
        setModal({ isOpen: false, type: 'edit' });
      } else {
        showToast({
          title: "Error",
          description: result.errors?.join(', ') || "An unknown error occurred",
          variant: "destructive",
        });
      }
    } catch (err: unknown) {
      const apiError = err as ApiError;
      showToast({
        title: "Error",
        description: apiError.message || "Failed to update product",
        variant: "destructive",
      });
    }
  };

  const handleDeleteProduct = async (id: number) => {
    try {
      if (useFallback) {
        setProducts(products.filter(p => p.id !== id));
        showToast({
          title: "Product Deleted (Offline Mode)",
          description: "The product has been removed from the local catalog.",
          variant: "default",
        });
        
        setModal({ isOpen: false, type: 'delete' });
        return;
      }
      
      const result = await catalogService.deleteProduct(id);
      
      if (result.success) {
        showToast({
          title: "Product Deleted",
          description: "The product has been successfully deleted.",
          variant: "default",
        });
        
        await loadProducts();
        
        setModal({ isOpen: false, type: 'delete' });
      } else {
        showToast({
          title: "Error",
          description: result.errors?.join(', ') || "An unknown error occurred",
          variant: "destructive",
        });
      }
    } catch (err: unknown) {
      const apiError = err as ApiError;
      showToast({
        title: "Error",
        description: apiError.message || "Failed to delete product",
        variant: "destructive",
      });
    }
  };

  return (
    <>
      <Header />
      <main className="pt-20 pb-16 px-4 bg-background">
        <div className="container mx-auto">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6 sm:mb-8">
            <h1 className="text-2xl sm:text-3xl font-bold text-foreground">Admin Panel</h1>
            <Button
              onClick={() => setModal({ isOpen: true, type: 'create' })}
              variant="default"
              className="flex items-center gap-2 w-full sm:w-auto justify-center border border-input rounded-lg"
            >
              <Plus className="h-5 w-5" />
              <span>Add Product</span>
            </Button>
          </div>

          {useFallback && (
            <div className="mb-4 p-4 bg-yellow-100 dark:bg-yellow-900/30 text-yellow-800 dark:text-yellow-200 rounded-lg">
              <p className="font-medium">⚠️ Using offline catalog mode</p>
              <p className="text-sm">Unable to connect to the product service. Showing limited functionality.</p>
            </div>
          )}

          {/* Search */}
          <div className="mb-6 relative">
            <div className="relative max-w-full sm:max-w-md">
              <input
                type="text"
                placeholder="Search products..."
                className="w-full pl-10 pr-4 py-2 border border-input rounded-lg bg-background text-foreground focus:outline-none focus:ring-2 focus:ring-primary"
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
              />
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-5 w-5" />
            </div>
          </div>

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

          {/* Products Table */}
          {!isLoading && filteredProducts && filteredProducts.length > 0 ? (
            <div className="overflow-x-auto rounded-lg shadow border border-border bg-card">
              <div className="min-w-full divide-y divide-border">
                {/* Desktop Table Header - Hidden on mobile */}
                <div className="bg-muted/50 hidden sm:flex">
                  <div className="px-6 py-3 w-1/2 sm:w-2/5 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">
                    Product
                  </div>
                  <div className="px-6 py-3 w-1/5 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider hidden sm:block">
                    Category
                  </div>
                  <div className="px-6 py-3 w-1/5 text-left text-xs font-medium text-muted-foreground uppercase tracking-wider">
                    Price
                  </div>
                  <div className="px-6 py-3 w-1/6 text-right text-xs font-medium text-muted-foreground uppercase tracking-wider">
                    Actions
                  </div>
                </div>

                {/* Product List */}
                <div className="divide-y divide-border">
                  {filteredProducts.map((product, index) => (
                    <div key={product.id} className={`flex flex-col sm:flex-row items-start sm:items-center py-3 sm:py-0 ${index % 2 === 0 ? "bg-card" : "bg-muted/30"}`}>
                      {/* Product Info - Mobile Layout */}
                      <div className="px-6 py-2 sm:py-4 w-full sm:w-2/5 flex items-center">
                        <div className="h-10 w-10 flex-shrink-0 rounded-full bg-muted overflow-hidden relative">
                          {product.image || product.imageUri ? (
                            <div 
                              className="h-full w-full bg-cover bg-center"
                              style={{ backgroundImage: `url(${product.imageUri || product.image})` }}
                            />
                          ) : (
                            <div className="h-full w-full flex items-center justify-center text-xs text-muted-foreground">
                              No img
                            </div>
                          )}
                        </div>
                        <div className="ml-4 flex-1">
                          <div className="text-sm font-medium text-foreground">
                            {product.name}
                          </div>
                          {/* Mobile-only category */}
                          <div className="text-xs text-muted-foreground mt-1 sm:hidden">
                            {product.category} • ${product.price.toFixed(2)}
                          </div>
                          <div className="text-xs text-muted-foreground mt-1 sm:hidden">
                            Added: {new Date(product.createdAt).toLocaleDateString()}
                          </div>
                        </div>
                      </div>

                      {/* Category - Desktop only */}
                      <div className="px-6 py-4 w-1/5 text-sm text-muted-foreground hidden sm:block">
                        {product.category}
                      </div>

                      {/* Price - Desktop only */}
                      <div className="px-6 py-4 w-1/5 text-sm text-foreground hidden sm:block">
                        ${product.price.toFixed(2)}
                      </div>

                      {/* Actions */}
                      <div className="px-6 py-2 sm:py-4 w-full sm:w-1/6 flex justify-end gap-2 mt-2 sm:mt-0">
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => setModal({ isOpen: true, type: 'edit', product })}
                          className="text-blue-600 hover:text-blue-900 dark:text-blue-400 dark:hover:text-blue-300"
                        >
                          <Pencil className="h-4 w-4" />
                          <span className="sr-only sm:not-sr-only sm:ml-2 text-xs">Edit</span>
                        </Button>
                        <Button
                          variant="ghost"
                          size="sm"
                          onClick={() => setModal({ isOpen: true, type: 'delete', product })}
                          className="text-red-600 hover:text-red-900 dark:text-red-400 dark:hover:text-red-300"
                        >
                          <Trash2 className="h-4 w-4" />
                          <span className="sr-only sm:not-sr-only sm:ml-2 text-xs">Delete</span>
                        </Button>
                      </div>
                    </div>
                  ))}
                </div>
              </div>
            </div>
          ) : !isLoading ? (
            <div className="px-6 py-10 text-center text-muted-foreground bg-card rounded-lg border border-border">
              <div className="flex flex-col items-center">
                <AlertCircle className="h-8 w-8 mb-2" />
                <p className="text-lg mb-4">No products found</p>
                <Button
                  variant="outline"
                  onClick={() => setSearchTerm("")}
                  className="mt-2"
                >
                  Clear Search
                </Button>
              </div>
            </div>
          ) : null}

          {/* Create/Edit Product Modal */}
          {modal.isOpen && modal.type !== 'delete' && (
            <div className="fixed inset-0 bg-background/80 backdrop-blur-sm flex items-center justify-center p-4 z-50">
              <div className="bg-card max-w-2xl w-full max-h-[90vh] overflow-y-auto rounded-lg shadow-lg border border-border">
                <div className="p-4 sm:p-6">
                  <h2 className="text-xl sm:text-2xl font-bold mb-4 sm:mb-6 text-foreground">
                    {modal.type === 'create' ? 'Add New Product' : 'Edit Product'}
                  </h2>
                  <AdminProductForm
                    initialData={modal.product}
                    onSubmit={modal.type === 'create' ? handleCreateProduct : handleUpdateProduct}
                    onCancel={() => setModal({ isOpen: false, type: 'create' })}
                  />
                </div>
              </div>
            </div>
          )}

          {/* Delete Confirmation Modal */}
          {modal.isOpen && modal.type === 'delete' && modal.product && (
            <div className="fixed inset-0 bg-background/80 backdrop-blur-sm flex items-center justify-center p-4 z-50">
              <div className="bg-card max-w-md w-full rounded-lg shadow-lg border border-border">
                <div className="p-4 sm:p-6">
                  <div className="flex items-center gap-3 mb-4">
                    <div className="bg-red-100 dark:bg-red-900/30 p-2 rounded-full">
                      <AlertCircle className="h-6 w-6 text-red-600 dark:text-red-400" />
                    </div>
                    <h2 className="text-lg sm:text-xl font-bold text-foreground">Delete Product</h2>
                  </div>
                  <p className="mb-6 text-muted-foreground">
                    Are you sure you want to delete <span className="font-semibold text-foreground">{modal.product.name}</span>? This action cannot be undone.
                  </p>
                  <div className="flex justify-end gap-3">
                    <Button
                      variant="outline"
                      onClick={() => setModal({ isOpen: false, type: 'delete' })}
                      className="dark:border-border"
                    >
                      Cancel
                    </Button>
                    <Button
                      variant="destructive"
                      onClick={() => handleDeleteProduct(modal.product!.id)}
                      className="dark:border dark:border-border"
                    >
                      Delete
                    </Button>
                  </div>
                </div>
              </div>
            </div>
          )}
        </div>
      </main>
      <Footer />
    </>
  );
}