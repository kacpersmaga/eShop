"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import { motion } from "framer-motion";
import { ArrowRight, Loader2 } from "lucide-react";
import ProductCard from "@/components/ui/product-card";
import catalogService, { Product } from "@/services/catalog-service";

interface ApiError {
  message: string;
  status?: number;
  data?: unknown;
}

const FeaturedProducts = () => {
  const [products, setProducts] = useState<Product[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [imageErrors, setImageErrors] = useState<Record<number, boolean>>({});

  useEffect(() => {
    const loadFeaturedProducts = async () => {
      setIsLoading(true);
      setError(null);

      try {
        const result = await catalogService.getProducts();

        if (result.success) {
          const productItems = result.data?.items || [];

          const origin = typeof window !== "undefined" ? window.location.origin : "";

          const normalizedProducts = productItems.map((product) => ({
            ...product,
            image: product.imageUri ? `${origin}${product.imageUri}` : product.image || "",
          }));

          setProducts(normalizedProducts.slice(0, 4));
        } else {
          setError(result.errors?.join(", ") || "Failed to load featured products");
        }
      } catch (err: unknown) {
        const apiError = err as ApiError;
        setError(apiError.message || "Failed to load featured products");
        console.error("Error loading featured products:", err);
      } finally {
        setIsLoading(false);
      }
    };

    loadFeaturedProducts();
  }, []);

  const handleImageError = (productId: number) => {
    setImageErrors((prev) => ({ ...prev, [productId]: true }));
  };

  return (
    <section className="py-16 bg-background">
      <div className="container mx-auto px-4">
        <div className="flex flex-col md:flex-row justify-between items-center mb-12">
          <div>
            <h2 className="text-3xl font-bold text-foreground mb-3">Featured Products</h2>
            <p className="text-muted-foreground max-w-2xl">
              Explore our handpicked collection of premium items, curated for quality and style.
            </p>
          </div>
          <Link
            href="/products"
            className="mt-6 md:mt-0 flex items-center text-primary hover:text-primary/80 font-medium transition-colors"
          >
            View All Products
            <ArrowRight className="ml-2 h-4 w-4" />
          </Link>
        </div>

        {isLoading && (
          <div className="flex justify-center items-center py-12">
            <Loader2 className="h-8 w-8 animate-spin text-primary" />
            <span className="ml-2 text-lg text-muted-foreground">Loading products...</span>
          </div>
        )}

        {error && (
          <div className="text-center py-12 bg-destructive/10 text-destructive rounded-lg">
            <p className="text-xl mb-4">Error loading products</p>
            <p className="text-muted-foreground">{error}</p>
          </div>
        )}

        {!isLoading && !error && (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
            {products.length > 0 ? (
              products.map((product, index) => (
                <motion.div
                  key={product.id}
                  initial={{ opacity: 0, y: 30 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5, delay: index * 0.1 }}
                >
                  <ProductCard
                    product={product}
                    onImageError={() => handleImageError(product.id)}
                    hasImageError={imageErrors[product.id]}
                  />
                </motion.div>
              ))
            ) : (
              <div className="col-span-full text-center py-12">
                <p className="text-muted-foreground">No featured products available.</p>
              </div>
            )}
          </div>
        )}
      </div>
    </section>
  );
};

export default FeaturedProducts;