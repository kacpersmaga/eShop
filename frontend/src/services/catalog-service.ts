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

export interface PagedResult<T> {
  data: {
    items: T[];
    totalItems: number;
    totalPages: number;
    pageNumber: number;
    pageSize: number;
    hasNextPage: boolean;
    hasPreviousPage: boolean;
  };
  succeeded: boolean;
  errors: string[];
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
  ): Promise<PagedResult<Product>> => {
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
    const raw = response.data;  

    if (raw?.data?.items && Array.isArray(raw.data.items)) {
      raw.data.items = raw.data.items.map(transformProduct);
    } else {
      raw.data = {
        items: [],
        totalItems: 0,
        totalPages: 0,
        pageNumber: 1,
        pageSize: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }

    return {
      data: raw.data,
      succeeded: raw.succeeded ?? false,
      errors: raw.errors ?? ["Unexpected response format"]
    };
  },

  searchProducts: async (term: string): Promise<PagedResult<Product>> => {
    const response = await apiClient.get(`/catalog/products/search?term=${encodeURIComponent(term)}`);
    const raw = response.data;

    if (raw?.data?.items && Array.isArray(raw.data.items)) {
      raw.data.items = raw.data.items.map(transformProduct);
    } else {
      raw.data = {
        items: [],
        totalItems: 0,
        totalPages: 0,
        pageNumber: 1,
        pageSize: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }

    return {
      data: raw.data,
      succeeded: raw.succeeded ?? false,
      errors: raw.errors ?? []
    };
  },

  getProductById: async (id: number): Promise<PagedResult<Product>> => {
    const response = await apiClient.get(`/catalog/products/${id}`);
    const raw = response.data;

    if (raw?.data?.items && Array.isArray(raw.data.items) && raw.data.items.length > 0) {
      raw.data.items = raw.data.items.map(transformProduct);
    } else {
      raw.data = {
        items: [],
        totalItems: 0,
        totalPages: 0,
        pageNumber: 1,
        pageSize: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }

    return {
      data: raw.data,
      succeeded: raw.succeeded ?? false,
      errors: raw.errors ?? []
    };
  },

  getProductByIdSimple: async (id: number): Promise<{product: Product | null, succeeded: boolean, errors: string[]}> => {
    const result = await catalogService.getProductById(id);
    
    return {
      product: result.data.items.length > 0 ? result.data.items[0] : null,
      succeeded: result.succeeded,
      errors: result.errors
    };
  },

  getProductsByPriceRange: async (minPrice: number, maxPrice: number): Promise<PagedResult<Product>> => {
    const response = await apiClient.get(`/catalog/products/price-range?minPrice=${minPrice}&maxPrice=${maxPrice}`);
    const raw = response.data;

    if (raw?.data?.items && Array.isArray(raw.data.items)) {
      raw.data.items = raw.data.items.map(transformProduct);
    } else {
      raw.data = {
        items: [],
        totalItems: 0,
        totalPages: 0,
        pageNumber: 1, 
        pageSize: 0,
        hasNextPage: false,
        hasPreviousPage: false
      };
    }

    return {
      data: raw.data,
      succeeded: raw.succeeded ?? false,
      errors: raw.errors ?? []
    };
  },

  createProduct: async (productData: CreateProductDto): Promise<{product: Product | null, succeeded: boolean, errors: string[]}> => {
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

    const raw = response.data;
    
    let product = null;
    if (raw?.data) {
      product = transformProduct(raw.data);
    }

    return {
      product,
      succeeded: raw.succeeded ?? false,
      errors: raw.errors ?? []
    };
  },

  updateProduct: async (id: number, productData: UpdateProductDto): Promise<{product: Product | null, succeeded: boolean, errors: string[]}> => {
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

    const raw = response.data;
    
    let product = null;
    if (raw?.data) {
      product = transformProduct(raw.data);
    }

    return {
      product,
      succeeded: raw.succeeded ?? false,
      errors: raw.errors ?? []
    };
  },

  deleteProduct: async (id: number): Promise<{succeeded: boolean, errors: string[]}> => {
    const response = await apiClient.delete(`/catalog/products/${id}`);
    const raw = response.data;

    return {
      succeeded: raw.succeeded ?? false,
      errors: raw.errors ?? []
    };
  },
};

export default catalogService;