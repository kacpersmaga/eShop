import Header from "@/components/layout/header";
import Footer from "@/components/layout/footer";
import HeroSection from "@/components/layout/hero";
import FeaturedProducts from "@/components/sections/featured-products";

export default function HomePage() {
  return (
    <>
      <Header />
      <main>
        <HeroSection />
        <FeaturedProducts />
      </main>
      <Footer/>
    </>
  );
}