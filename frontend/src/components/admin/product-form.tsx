"use client";

import { useState, useEffect } from "react";
import Image from "next/image";
import { Upload, X, ImageIcon } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Product } from "@/services/catalog-service";

type ProductFormProps = {
  initialData?: Product;
  onSubmit: (data: Omit<Product, 'id' | 'createdAt'> & { imageFile?: File }) => void;
  onCancel: () => void;
};

const categories = [
  "Electronics",
  "Clothing",
  "Home",
  "Accessories",
  "Beauty",
  "Sports",
  "Books",
  "Food",
  "Other"
];

const AdminProductForm = ({ initialData, onSubmit, onCancel }: ProductFormProps) => {
  const [formData, setFormData] = useState({
    name: initialData?.name || "",
    price: initialData?.price || 0,
    image: initialData?.image || "",
    category: initialData?.category || categories[0],
    description: initialData?.description || "",
  });
  
  const [errors, setErrors] = useState({
    name: "",
    price: "",
    description: "",
  });
  
  const [imagePreview, setImagePreview] = useState<string | null>(initialData?.imageUri || initialData?.image || null);
  const [imageFile, setImageFile] = useState<File | undefined>(undefined);
  const [imageError, setImageError] = useState(false);
  
  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
    

    if (errors[name as keyof typeof errors]) {
      setErrors(prev => ({ ...prev, [name]: "" }));
    }
  };

  const handlePriceChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const value = parseFloat(e.target.value);
    setFormData(prev => ({ ...prev, price: isNaN(value) ? 0 : value }));
    
    if (errors.price) {
      setErrors(prev => ({ ...prev, price: "" }));
    }
  };
  
  const handleImageChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (file) {
      setImageError(false);
      
      setImageFile(file);
      
      const imageUrl = URL.createObjectURL(file);
      setImagePreview(imageUrl);
    }
  };
  
  const handleRemoveImage = () => {
    setImagePreview(null);
    setImageFile(undefined);
    setFormData(prev => ({ ...prev, image: "" }));
    setImageError(false);
  };
  
  const handleImageError = () => {
    setImageError(true);
  };
  
  const validateForm = () => {
    let valid = true;
    const newErrors = {
      name: "",
      price: "",
      description: "",
    };
    
    if (!formData.name.trim()) {
      newErrors.name = "Product name is required";
      valid = false;
    }
    
    if (formData.price <= 0) {
      newErrors.price = "Price must be greater than zero";
      valid = false;
    }
    
    if (!formData.description.trim()) {
      newErrors.description = "Description is required";
      valid = false;
    } else if (formData.description.length < 10) {
      newErrors.description = "Description must be at least 10 characters";
      valid = false;
    }
    
    setErrors(newErrors);
    return valid;
  };
  
  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    if (validateForm()) {
      onSubmit({
        ...formData,
        imageFile
      });
    }
  };
  
  useEffect(() => {
    return () => {
      if (imagePreview && !initialData?.image) {
        URL.revokeObjectURL(imagePreview);
      }
    };
  }, [imagePreview, initialData]);
  
  return (
    <form onSubmit={handleSubmit} className="space-y-6 bg-card shadow-md p-6 rounded-lg border border-border">
      {/* Product Name */}
      <div>
        <label htmlFor="name" className="block text-sm font-medium mb-1 text-foreground">
          Product Name *
        </label>
        <input
          type="text"
          id="name"
          name="name"
          value={formData.name}
          onChange={handleChange}
          className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-primary focus:outline-none border-input bg-background text-foreground"
          placeholder="Enter product name"
        />
        {errors.name && (
          <p className="mt-1 text-sm text-destructive font-medium">{errors.name}</p>
        )}
      </div>
      
      {/* Price and Category */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div>
          <label htmlFor="price" className="block text-sm font-medium mb-1 text-foreground">
            Price ($) *
          </label>
          <input
            type="number"
            id="price"
            name="price"
            min="0"
            step="0.01"
            value={formData.price}
            onChange={handlePriceChange}
            className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-primary focus:outline-none border-input bg-background text-foreground"
            placeholder="0.00"
          />
          {errors.price && (
            <p className="mt-1 text-sm text-destructive font-medium">{errors.price}</p>
          )}
        </div>
        
        <div>
          <label htmlFor="category" className="block text-sm font-medium mb-1 text-foreground">
            Category
          </label>
          <select
            id="category"
            name="category"
            value={formData.category}
            onChange={handleChange}
            className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-primary focus:outline-none border-input bg-background text-foreground appearance-none"
          >
            {categories.map(category => (
              <option 
                key={category} 
                value={category}
                className="bg-background text-foreground dark:bg-card"
              >
                {category}
              </option>
            ))}
          </select>
        </div>
      </div>
      
      {/* Image Upload */}
      <div>
        <label className="block text-sm font-medium mb-1 text-foreground">
          Product Image
        </label>
        <div className="mt-1 flex justify-center px-6 pt-5 pb-6 border-2 border-dashed rounded-lg border-input dark:border-input bg-background dark:bg-card">
          {imagePreview ? (
            <div className="relative">
              <div className="relative h-64 w-64">
                {!imageError ? (
                  <img 
  src={imagePreview || ''} 
  alt="Product preview" 
  className="rounded-lg object-cover h-64 w-64"
  onError={handleImageError}
/>
                ) : (
                  <div className="absolute inset-0 flex flex-col items-center justify-center bg-muted rounded-lg">
                    <ImageIcon className="h-12 w-12 mb-2 text-muted-foreground" />
                    <span className="text-sm text-muted-foreground">Image not available</span>
                  </div>
                )}
              </div>
              <button
                type="button"
                onClick={handleRemoveImage}
                className="absolute top-2 right-2 p-1 bg-destructive text-destructive-foreground rounded-full hover:bg-destructive/90 transition-standard"
              >
                <X className="h-4 w-4" />
              </button>
            </div>
          ) : (
            <div className="space-y-1 text-center">
              <Upload className="mx-auto h-12 w-12 text-foreground/70" />
              <div className="flex text-sm text-foreground/80">
                <label htmlFor="image-upload" className="relative cursor-pointer rounded-md font-medium text-primary hover:text-primary/80">
                  <span>Upload an image</span>
                  <input 
                    id="image-upload" 
                    name="image-upload" 
                    type="file" 
                    accept="image/*"
                    className="sr-only" 
                    onChange={handleImageChange}
                  />
                </label>
                <p className="pl-1">or drag and drop</p>
              </div>
              <p className="text-xs text-foreground/70">
                PNG, JPG, GIF up to 10MB
              </p>
            </div>
          )}
        </div>
      </div>
      
      {/* Description */}
      <div>
        <label htmlFor="description" className="block text-sm font-medium mb-1 text-foreground">
          Description *
        </label>
        <textarea
          id="description"
          name="description"
          rows={4}
          value={formData.description}
          onChange={handleChange}
          className="w-full px-4 py-2 border rounded-lg focus:ring-2 focus:ring-primary focus:outline-none border-input bg-background text-foreground"
          placeholder="Enter product description"
        />
        {errors.description && (
          <p className="mt-1 text-sm text-destructive font-medium">{errors.description}</p>
        )}
      </div>
      
      {/* Form Actions */}
      <div className="flex justify-end gap-3 pt-4">
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          className="border-input bg-background text-foreground hover:bg-secondary/50"
        >
          Cancel
        </Button>
        <Button
          type="submit"
          variant="default"
          className="bg-primary text-primary-foreground hover:bg-primary/90"
        >
          {initialData ? 'Update Product' : 'Create Product'}
        </Button>
      </div>
    </form>
  );
};

export default AdminProductForm;