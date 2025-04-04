import type { NextConfig } from 'next';

const nextConfig: NextConfig = {
  reactStrictMode: true,
  
  images: {
    remotePatterns: [
      {
        protocol: 'http',
        hostname: 'localhost',
        port: '8080',
        pathname: '/api/images/**',
      },
      {
        protocol: 'http',
        hostname: 'azurite',
        port: '10000',
        pathname: '/devstoreaccount1/images/**',
      },
      {
        protocol: 'http',
        hostname: 'localhost',
        port: '10000',
        pathname: '/devstoreaccount1/images/**',
      },
      {
        protocol: 'https',
        hostname: 'via.placeholder.com',
        port: '',
        pathname: '/**',
      },
    ],
    unoptimized: process.env.NODE_ENV === 'development',
  },

  async rewrites() {
    return [
      {
        source: '/api/:path*',
        destination: 'http://eshop:8080/api/:path*',
      },
      {
        source: '/storage/:path*',
        destination: 'http://azurite:10000/devstoreaccount1/:path*',
      },
    ];
  },  

  eslint: {
    ignoreDuringBuilds: true,
  },
};

export default nextConfig;