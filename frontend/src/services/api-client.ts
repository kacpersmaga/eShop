import axios from 'axios';
import Cookies from 'js-cookie';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || '/api';

const apiClient = axios.create({
  baseURL: API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
  withCredentials: true,
});

apiClient.interceptors.request.use(
  (config) => {
    const method = config.method?.toUpperCase();

    if (method && method !== 'GET' && method !== 'OPTIONS') {
      const csrfToken = Cookies.get('XSRF-TOKEN');
      if (csrfToken) {
        config.headers['X-CSRF-TOKEN'] = csrfToken;
      }
    }

    return config;
  },
  (error) => Promise.reject(error)
);

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.code === 'ECONNREFUSED' || !error.response) {
      const customError = {
        message: 'Cannot connect to server. Please check your connection.',
        status: 0,
        data: { connectionError: true },
      };

      if (process.env.NODE_ENV === 'development') {
        console.error('API Connection Error:', error);
      }

      return Promise.reject(customError);
    }

    const customError = {
      message: error.response?.data?.message || 'An unexpected error occurred',
      status: error.response?.status,
      data: error.response?.data,
    };

    if (process.env.NODE_ENV === 'development') {
      console.error('API Error:', customError);
    }

    return Promise.reject(customError);
  }
);

export default apiClient;
