const nextConfig = {
  reactStrictMode: true,

  images: {
    remotePatterns: [
      {
        protocol: 'https',
        hostname: 'kacpersmaga.pl',
        port: '',
        pathname: '/api/images/**',
      },
      {
        protocol: 'https',
        hostname: 'kacpersmaga.pl',
        port: '',
        pathname: '/storage/images/**',
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
    if (process.env.NODE_ENV === 'production') {
      return [];
    }

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
