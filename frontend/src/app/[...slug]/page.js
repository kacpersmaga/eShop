export const dynamic = 'force-static';

import Link from "next/link";
import { ArrowLeft } from "lucide-react";
import { Button } from "@/components/ui/button";
import Header from "@/components/layout/header";
import Footer from "@/components/layout/footer";

export async function generateStaticParams() {
  const paths = [
    'about', 'contact', 'shop', 'new-arrivals', 'sale', 'categories',
    'shipping', 'faq', 'size-guide', 'careers', 'sustainability', 'press',
    'privacy-policy', 'terms-of-service', 'cookie-policy', 'account'
  ];
  
  return paths.map(path => ({
    slug: [path],
  }));
}

export default function PlaceholderPage({
  params,
}) {
  const pagePath = '/' + params.slug.join('/');
  const pageName = getPageName(params.slug);
  
  return (
    <>
      <Header />
      <main className="pt-24 pb-16 bg-background">
        <div className="container mx-auto px-4 text-center">
          <h1 className="text-3xl md:text-4xl font-bold mb-6 text-foreground">
            {pageName}
          </h1>
          
          <div className="max-w-2xl mx-auto bg-card border border-border rounded-lg p-8 mb-8 shadow-sm">
            <div className="flex flex-col items-center justify-center space-y-4">
              <div className="w-24 h-24 rounded-full bg-primary/10 flex items-center justify-center mb-2">
                <span className="text-4xl text-primary">🚧</span>
              </div>
              
              <h2 className="text-xl font-semibold text-foreground">
                Coming Soon
              </h2>
              
              <p className="text-muted-foreground text-center max-w-md mb-4">
                We're currently working on this page. Please check back later for updates.
              </p>
              
              <Button asChild variant="outline">
                <Link href="/" className="flex items-center gap-2">
                  <ArrowLeft className="w-4 h-4" />
                  Return to Home
                </Link>
              </Button>
            </div>
          </div>
        </div>
      </main>
      <Footer />
    </>
  );
}

function getPageName(slug) {
  if (slug.length > 1) {
    return slug
      .map(part => part.charAt(0).toUpperCase() + part.slice(1))
      .join(' ');
  }
  
  const pageName = slug[0];
  
  switch (pageName) {
    case 'faq':
      return 'Frequently Asked Questions';
    case 'about':
      return 'About Us';
    case 'contact':
      return 'Contact Us';
    default:
      return pageName
        .split('-')
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' ');
  }
}