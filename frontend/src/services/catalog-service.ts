import apiClient from "./api-client";

export interface Product {
  id: number;
  name: string;
  price: number;
  description: string;
  category: string;
  image: string;
  imageUri?: string;
  currency?: string;
  isAvailable?: boolean;
  createdAt: string;
  updatedAt?: string;
}

export interface CreateProductDto {
  name: string;
  price: number;
  description: string;
  category: string;
  image?: File;
}

export interface UpdateProductDto {
  name: string;
  price: number;
  description: string;
  category: string;
  image?: File;
}

export interface ApiResponse<T> {
  success: boolean;
  statusCode: number;
  message?: string;
  data?: T;
  errors?: string[];
  timestamp: string;
  metadata?: Record<string, any>;
}

export interface PagedData {
  items: Product[];
  totalItems: number;
  totalPages: number;
  pageNumber: number;
  pageSize: number;
  hasNextPage: boolean;
  hasPreviousPage: boolean;
}

function transformProduct(apiProduct: any): Product {
  return {
    id: apiProduct.id ?? 0,
    name: apiProduct.name ?? "",
    price: apiProduct.price ?? 0,
    description: apiProduct.description ?? "",
    category: apiProduct.category ?? "",
    image: apiProduct.imageUri ?? "",      
    imageUri: apiProduct.imageUri ?? "",
    currency: apiProduct.currency ?? "USD",
    isAvailable: apiProduct.isAvailable !== undefined ? apiProduct.isAvailable : true,
    createdAt: apiProduct.createdAt ?? new Date().toISOString(),
    updatedAt: apiProduct.updatedAt ?? undefined,
  };
}

const catalogService = {
  getProducts: async (
    page?: number,
    pageSize?: number,
    category?: string,
    sortBy?: string,
    ascending?: boolean
  ): Promise<ApiResponse<PagedData>> => {
    let url = "/catalog/products";
    const params = new URLSearchParams();

    if (page) params.append("page", page.toString());
    if (pageSize) params.append("pageSize", pageSize.toString());
    if (category) params.append("category", category);
    if (sortBy) params.append("sortBy", sortBy);
    if (ascending !== undefined) params.append("ascending", ascending.toString());

    if (params.toString()) {
      url += `?${params.toString()}`;
    }

    const response = await apiClient.get(url);
    const apiResponse = response.data as ApiResponse<PagedData>;
    
    if (apiResponse.data?.items && Array.isArray(apiResponse.data.items)) {
      apiResponse.data.items = apiResponse.data.items.map(transformProduct);
    } else if (apiResponse.data) {
      // Ensure we always have a valid items array
      apiResponse.data.items = [];
    } else {
      // Create a default empty response if data is missing
      apiResponse.data = {
        items: [],
        totalItems: 0,
        totalPages: 0,
        pageNumber: 1,
        pageSize: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }

    return apiResponse;
  },

  searchProducts: async (term: string): Promise<ApiResponse<PagedData>> => {
    const response = await apiClient.get(`/catalog/products/search?term=${encodeURIComponent(term)}`);
    const apiResponse = response.data as ApiResponse<PagedData>;
    
    if (apiResponse.data?.items && Array.isArray(apiResponse.data.items)) {
      apiResponse.data.items = apiResponse.data.items.map(transformProduct);
    } else if (apiResponse.data) {
      apiResponse.data.items = [];
    } else {
      apiResponse.data = {
        items: [],
        totalItems: 0,
        totalPages: 0,
        pageNumber: 1,
        pageSize: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }

    return apiResponse;
  },

  getProductById: async (id: number): Promise<ApiResponse<PagedData>> => {
    const response = await apiClient.get(`/catalog/products/${id}`);
    const apiResponse = response.data as ApiResponse<PagedData>;

    if (apiResponse.data?.items && Array.isArray(apiResponse.data.items)) {
      apiResponse.data.items = apiResponse.data.items.map(transformProduct);
    } else if (apiResponse.data) {
      apiResponse.data.items = [];
    } else {
      apiResponse.data = {
        items: [],
        totalItems: 0,
        totalPages: 0,
        pageNumber: 1,
        pageSize: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }

    return apiResponse;
  },

  getProductByIdSimple: async (id: number): Promise<{product: Product | null, success: boolean, errors?: string[]}> => {
    const result = await catalogService.getProductById(id);
    
    return {
      product: result.data?.items.length ? result.data.items[0] : null,
      success: result.success,
      errors: result.errors
    };
  },

  getProductsByPriceRange: async (minPrice: number, maxPrice: number): Promise<ApiResponse<PagedData>> => {
    const response = await apiClient.get(`/catalog/products/price-range?minPrice=${minPrice}&maxPrice=${maxPrice}`);
    const apiResponse = response.data as ApiResponse<PagedData>;

    if (apiResponse.data?.items && Array.isArray(apiResponse.data.items)) {
      apiResponse.data.items = apiResponse.data.items.map(transformProduct);
    } else if (apiResponse.data) {
      apiResponse.data.items = [];
    } else {
      apiResponse.data = {
        items: [],
        totalItems: 0,
        totalPages: 0,
        pageNumber: 1,
        pageSize: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }

    return apiResponse;
  },

  createProduct: async (productData: CreateProductDto): Promise<{product: Product | null, success: boolean, errors?: string[], message?: string}> => {
    const formData = new FormData();
    formData.append("name", productData.name);
    formData.append("price", productData.price.toString());
    formData.append("description", productData.description);
    formData.append("category", productData.category);

    if (productData.image) {
      formData.append("image", productData.image);
    }

    const response = await apiClient.post("/catalog/products", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    const apiResponse = response.data as ApiResponse<string>;
    
    let product = null;
    // Since we're returning a string success message, we'll need
    // to fetch the product separately if needed
    
    return {
      product,
      success: apiResponse.success,
      errors: apiResponse.errors,
      message: apiResponse.message
    };
  },

  updateProduct: async (id: number, productData: UpdateProductDto): Promise<{product: Product | null, success: boolean, errors?: string[], message?: string}> => {
    const formData = new FormData();
    formData.append("name", productData.name);
    formData.append("price", productData.price.toString());
    formData.append("description", productData.description);
    formData.append("category", productData.category);

    if (productData.image) {
      formData.append("image", productData.image);
    }

    const response = await apiClient.put(`/catalog/products/${id}`, formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });

    const apiResponse = response.data as ApiResponse<string>;
    
    return {
      product: null, // Similar to create, we would need to fetch the updated product separately
      success: apiResponse.success,
      errors: apiResponse.errors,
      message: apiResponse.message
    };
  },

  deleteProduct: async (id: number): Promise<{success: boolean, errors?: string[], message?: string}> => {
    const response = await apiClient.delete(`/catalog/products/${id}`);
    const apiResponse = response.data as ApiResponse<string>;

    return {
      success: apiResponse.success,
      errors: apiResponse.errors,
      message: apiResponse.message
    };
  },
};

export default catalogService;