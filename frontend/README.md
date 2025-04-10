# eShop Frontend

This is the frontend of the eShop project, a modular e-commerce system built with React and Next.js. It provides a responsive, user-friendly interface with dark/light mode support, SEO optimization, and seamless integration with the backend API.

## Features
- Responsive design with Tailwind CSS.
- Dark/light mode toggle with user preference persistence.
- Smooth animations using Framer Motion.
- API communication via Axios with CSRF protection.
- SEO optimization and image handling with Next.js.
- Product browsing, searching, filtering, and CRUD operations.

## Technologies
- **Languages**: JavaScript, TypeScript
- **Framework**: React, Next.js
- **Styling**: Tailwind CSS
- **Libraries**: Axios, Framer Motion, Lucide React, js-cookie
- **Tools**: Docker, ESLint, PostCSS

## Project Structure
```
frontend/
├── public/           # Static assets (e.g., favicon)
├── src/              # Source code
│   ├── app/          # Next.js app directory (pages and layouts)
│   ├── components/   # Reusable UI components
│   ├── lib/          # Utility functions
│   └── services/     # API client and service logic (e.g., catalogService.ts)
├── .gitignore        # Git ignore file
├── Dockerfile        # Docker configuration
├── next.config.ts    # Next.js configuration
├── package.json      # Dependencies and scripts
└── tsconfig.json     # TypeScript configuration
```

## Prerequisites
- Node.js (v18 or higher)
- npm or yarn
- Docker (optional for containerized deployment)
- Backend API running (e.g., at `http://localhost:8080/api` for development)

## Setup and Installation
1. **Clone the repository**:
   ```bash
   git clone https://github.com/kacpersmaga/eShop.git
   cd eShop/frontend
   ```
2. **Install dependencies**:
   ```bash
   npm install
   ```
3. **Run the development server**:
   ```bash
   npm run dev
   ```
   - The frontend will be available at `http://localhost:3000`.
   - Ensure the backend is running at `http://localhost:8080/api`.
4. **Build for production**:
   ```bash
   npm run build
   npm run start
   ```
   - The production build will be available at `http://localhost:3000`.
5. **Run with Docker**:
   ```bash
   docker-compose up --build
   ```
   - Use the root `docker-compose.yml` for production (proxied via Caddy at `https://kacpersmaga.pl`).
   - Use `docker-compose.override.yml` for development (direct access at `http://localhost:3000`).

## Configuration
- **API Base URL**: Set `NEXT_PUBLIC_API_URL` in your environment variables:
  - Development: `http://localhost:8080/api`
  - Production: `https://kacpersmaga.pl/api`
  - Configured in `src/services/api-client.ts`.
- **Image Handling**: Remote patterns for images are defined dynamically in `next.config.ts` using `NEXT_PUBLIC_IMAGE_DOMAIN`:
  - Default: `https://kacpersmaga.pl/api/images/**` and `https://kacpersmaga.pl/storage/images/**`
  - Customizable via environment variable (e.g., `https://twoja-domena.com`).
- **Development Rewrites**: API and storage requests are proxied to backend services (see `next.config.ts`).

## Environment Variables
- `NODE_ENV`: `development` or `production`.
- `NEXT_PUBLIC_API_URL`: Base URL for backend API requests (e.g., `https://kacpersmaga.pl/api`).
- `NEXT_PUBLIC_IMAGE_DOMAIN`: Domain for Next.js image optimization (e.g., `kacpersmaga.pl` or your custom domain).
- `NEXT_DISABLE_LIGHTNING_CSS`: Optional, disables Lightning CSS in development (set to `"true"`).

## API Integration
- The frontend communicates with the backend via `catalogService.ts`, which provides methods for:
  - Fetching products (`getProducts`, `searchProducts`, `getProductsByPriceRange`).
  - Product CRUD operations (`createProduct`, `updateProduct`, `deleteProduct`).
- Axios is configured with CSRF token support and error handling in `api-client.ts`.

## Author
Kacper Smaga  
- Email: kacper.smaga@onet.pl  
- GitHub: [kacpersmaga](https://github.com/kacpersmaga)