"use client";

import { useState, useEffect } from "react";
import { useParams, useRouter } from "next/navigation";
import Header from "@/components/layout/header";
import Footer from "@/components/layout/footer";
import { Button } from "@/components/ui/button";
import { ShoppingCart, Heart, ArrowLeft, Loader2, ImageIcon, AlertCircle, Star } from "lucide-react";
import catalogService, { Product } from "@/services/catalog-service";
import Link from "next/link";
import { motion } from "framer-motion";

interface ApiError {
  message: string;
  status?: number;
  data?: unknown;
}

export default function ProductDetailPage() {
  const { id } = useParams();
  const router = useRouter();
  const [product, setProduct] = useState<Product | null>(null);
  const [quantity, setQuantity] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [isFavorite, setIsFavorite] = useState(false);
  const [imageError, setImageError] = useState(false);
  
  useEffect(() => {
    const loadProduct = async () => {
      if (!id) return;
      
      setIsLoading(true);
      setError(null);
      
      try {
        const result = await catalogService.getProductByIdSimple(Number(id));
        
        if (result.success && result.product) {
          setProduct(result.product);
        } else {
          setError(result.errors?.join(', ') || 'Product not found');
        }
      } catch (err: unknown) {
        const apiError = err as ApiError;
        setError(apiError.message || 'Failed to load product details');
        console.error('Error loading product:', err);
      } finally {
        setIsLoading(false);
      }
    };
    
    loadProduct();
  }, [id]);
  
  const handleQuantityChange = (delta: number) => {
    setQuantity(prev => {
      const newQuantity = prev + delta;
      return newQuantity > 0 ? newQuantity : 1;
    });
  };
  
  const handleAddToCart = () => {
    if (!product) return;
    
    alert(`Added ${quantity} ${product.name} to cart`);
  };
  
  return (
    <>
      <Header />
      <main className="pt-20 pb-16 bg-background">
        <div className="container mx-auto px-4">
          {/* Back Button */}
          <Button 
            variant="ghost" 
            className="mb-6 pl-0 text-foreground hover:text-primary"
            onClick={() => router.back()}
          >
            <ArrowLeft className="mr-2 h-4 w-4" />
            Back to Products
          </Button>
          
          {/* Loading State */}
          {isLoading && (
            <div className="flex justify-center items-center py-24">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <span className="ml-2 text-lg text-muted-foreground">Loading product details...</span>
            </div>
          )}
          
          {/* Error State */}
          {error && (
            <div className="text-center py-12 bg-destructive/10 text-destructive rounded-lg">
              <div className="flex flex-col items-center">
                <AlertCircle className="h-12 w-12 mb-4" />
                <p className="text-xl mb-4">Error loading product</p>
                <p className="text-muted-foreground mb-6">{error}</p>
                
                <Link href="/products">
                  <Button variant="outline" className="mt-2">
                    Browse Other Products
                  </Button>
                </Link>
              </div>
            </div>
          )}
          
          {/* Product Details */}
          {!isLoading && product && (
            <div className="grid grid-cols-1 lg:grid-cols-2 gap-8 lg:gap-12">
              {/* Product Image */}
              <motion.div 
                initial={{ opacity: 0, x: -20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.5 }}
                className="bg-card rounded-xl overflow-hidden border border-border shadow-md"
              >
                {product.image && !imageError ? (
                  <div 
                    className="w-full h-96 lg:h-[500px] bg-center bg-cover"
                    style={{ backgroundImage: `url(${product.imageUri || product.image})` }}
                    onError={() => setImageError(true)}
                  />
                ) : (
                  <div className="w-full h-96 lg:h-[500px] bg-muted flex flex-col items-center justify-center p-4">
                    <ImageIcon className="h-16 w-16 mb-4 text-muted-foreground" />
                    <span className="text-lg text-muted-foreground text-center">
                      {product.name}
                    </span>
                  </div>
                )}
              </motion.div>
              
              {/* Product Info */}
              <motion.div 
                initial={{ opacity: 0, x: 20 }}
                animate={{ opacity: 1, x: 0 }}
                transition={{ duration: 0.5, delay: 0.2 }}
                className="flex flex-col"
              >
                <div className="mb-2">
                  <span className="inline-block px-2 py-1 text-xs font-medium rounded-full bg-primary/10 text-primary">
                    {product.category}
                  </span>
                </div>
                <h1 className="text-2xl sm:text-3xl font-bold mb-2 text-foreground">
                  {product.name}
                </h1>
                
                {/* Price */}
                <div className="flex items-center mb-4">
                  <span className="text-2xl font-bold text-foreground">
                    ${product.price.toFixed(2)}
                  </span>
                  {product.currency && product.currency !== "USD" && (
                    <span className="ml-2 text-sm text-muted-foreground">
                      {product.currency}
                    </span>
                  )}
                </div>
                
                {/* Rating*/}
                <div className="flex items-center mb-6">
                  <div className="flex">
                    {[1, 2, 3, 4, 5].map((star) => (
                      <Star 
                        key={star} 
                        className={`h-5 w-5 ${star <= 4 ? "text-yellow-400 fill-yellow-400" : "text-muted-foreground"}`} 
                      />
                    ))}
                  </div>
                  <span className="ml-2 text-sm text-muted-foreground">
                    (24 reviews)
                  </span>
                </div>
                
                {/* Description */}
                <p className="text-muted-foreground mb-8">
                  {product.description}
                </p>
                
                {/* Availability */}
                <div className="mb-6">
                  <span className={`inline-flex items-center ${product.isAvailable !== false ? "text-green-600 dark:text-green-400" : "text-red-600 dark:text-red-400"}`}>
                    <span className={`w-2 h-2 rounded-full mr-2 ${product.isAvailable !== false ? "bg-green-600 dark:bg-green-400" : "bg-red-600 dark:bg-red-400"}`}></span>
                    {product.isAvailable !== false ? "In Stock" : "Out of Stock"}
                  </span>
                </div>
                
                {/* Quantity Selector */}
                <div className="flex items-center mb-6">
                  <span className="text-sm font-medium text-foreground mr-4">Quantity:</span>
                  <div className="flex items-center border border-input rounded-md">
                    <button 
                      onClick={() => handleQuantityChange(-1)}
                      className="px-3 py-1 text-foreground hover:bg-accent focus:outline-none"
                      disabled={quantity <= 1}
                    >
                      -
                    </button>
                    <span className="px-4 py-1 text-foreground">{quantity}</span>
                    <button 
                      onClick={() => handleQuantityChange(1)}
                      className="px-3 py-1 text-foreground hover:bg-accent focus:outline-none"
                    >
                      +
                    </button>
                  </div>
                </div>
                
                {/* Actions */}
                <div className="flex flex-col sm:flex-row gap-4">
                  <Button 
                    onClick={handleAddToCart}
                    disabled={product.isAvailable === false}
                    className="flex-1 bg-primary text-primary-foreground hover:bg-primary/90 h-12"
                  >
                    <ShoppingCart className="mr-2 h-5 w-5" />
                    Add to Cart
                  </Button>
                  <Button 
                    variant="outline" 
                    onClick={() => setIsFavorite(!isFavorite)}
                    className={`flex-1 h-12 border-input ${
                      isFavorite 
                        ? "bg-red-100 text-red-600 border-red-200 dark:bg-red-900/30 dark:text-red-400 dark:border-red-800" 
                        : "bg-background text-foreground"
                    }`}
                  >
                    <Heart className={`mr-2 h-5 w-5 ${isFavorite ? "fill-current" : ""}`} />
                    {isFavorite ? "Saved" : "Save for Later"}
                  </Button>
                </div>
                
                {/* Additional Info */}
                <div className="mt-8 pt-6 border-t border-border">
                  <h3 className="text-lg font-medium mb-2 text-foreground">Product Details</h3>
                  <ul className="space-y-1 text-sm text-muted-foreground">
                    <li>SKU: PRD-{product.id.toString().padStart(5, '0')}</li>
                    <li>Category: {product.category}</li>
                    {product.createdAt && (
                      <li>Added: {new Date(product.createdAt).toLocaleDateString()}</li>
                    )}
                  </ul>
                </div>
              </motion.div>
            </div>
          )}
        </div>
      </main>
      <Footer />
    </>
  );
}