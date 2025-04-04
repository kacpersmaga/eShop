"use client";

import { useState } from "react";
import Link from "next/link";
import { ImageIcon, ShoppingCart, Heart } from "lucide-react";

type ProductProps = {
  product: {
    id: number;
    name: string;
    price: number;
    image?: string;
    imageUri?: string;
    category: string;
    description?: string;
  };
  onImageError?: () => void;
  hasImageError?: boolean;
};

const ProductCard = ({ product, onImageError, hasImageError }: ProductProps) => {
  const [isFavorite, setIsFavorite] = useState(false);
  const [imageError, setImageError] = useState(false);

  const imageUrl = product.imageUri || product.image;

  const handleImageError = () => {
    setImageError(true);
    if (onImageError) onImageError();
  };

  return (
    <div className="bg-card text-card-foreground rounded-xl overflow-hidden shadow-md border border-border">
      <Link href={`/products/${product.id}`}>
        <div className="relative h-64 overflow-hidden bg-background">
          {imageUrl && !imageError && !hasImageError ? (
            <img
              src={imageUrl}
              alt={product.name}
              onError={handleImageError}
              className="absolute inset-0 w-full h-full object-cover"
            />
          ) : (
            <div className="w-full h-full flex flex-col items-center justify-center bg-muted">
              <ImageIcon className="h-12 w-12 mb-2 text-muted-foreground" />
              <span className="text-sm text-muted-foreground text-center">
                {product.name}
              </span>
            </div>
          )}

          <div className="absolute top-2 right-2 z-10">
            <button
              onClick={(e) => {
                e.preventDefault();
                setIsFavorite(!isFavorite);
              }}
              className={`p-2 rounded-full shadow-sm ${
                isFavorite
                  ? "bg-red-100 text-red-500 dark:bg-red-900/50 dark:text-red-400"
                  : "bg-background/90 text-muted-foreground dark:bg-card/90"
              }`}
              aria-label={isFavorite ? "Remove from favorites" : "Add to favorites"}
            >
              <Heart className={`h-5 w-5 ${isFavorite ? "fill-current" : ""}`} />
            </button>
          </div>
        </div>
      </Link>

      <div className="p-4">
        <div className="flex justify-between items-start">
          <div>
            <span className="text-xs text-muted-foreground uppercase tracking-wider">
              {product.category}
            </span>
            <h3 className="font-medium text-foreground mt-1 line-clamp-1">
              {product.name}
            </h3>
          </div>
          <span className="font-bold text-foreground">
            ${product.price.toFixed(2)}
          </span>
        </div>
        {product.description && (
          <p className="mt-2 text-sm text-muted-foreground line-clamp-2">
            {product.description}
          </p>
        )}
      </div>
    </div>
  );
};

export default ProductCard;