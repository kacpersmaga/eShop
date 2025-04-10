"use client";

import Link from "next/link";
import { useState, useEffect } from "react";
import { usePathname } from "next/navigation";
import { motion, AnimatePresence } from "framer-motion";
import { User, Menu, ShoppingCart, Sun, Moon, Settings, X } from "lucide-react";
import { Button } from "@/components/ui/button";

const Header = () => {
  const [isScrolled, setIsScrolled] = useState(false);
  const [isDarkMode, setIsDarkMode] = useState(false);
  const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
  const pathname = usePathname();

  const navItems = [
    { name: "Home", path: "/" },
    { name: "Products", path: "/products" },
    { name: "About", path: "/about" },
    { name: "Contact", path: "/contact" },
  ];

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 10);
    };
    window.addEventListener("scroll", handleScroll);
    handleScroll();
    
    setIsDarkMode(document.documentElement.classList.contains("dark"));
    
    const observer = new MutationObserver((mutations) => {
      mutations.forEach((mutation) => {
        if (
          mutation.type === "attributes" &&
          mutation.attributeName === "class"
        ) {
          setIsDarkMode(document.documentElement.classList.contains("dark"));
        }
      });
    });
    
    observer.observe(document.documentElement, {
      attributes: true,
      attributeFilter: ["class"],
    });
    
    return () => {
      window.removeEventListener("scroll", handleScroll);
      observer.disconnect();
    };
  }, []);

  const toggleDarkMode = () => {
    if (typeof window.toggleDarkMode === "function") {
      window.toggleDarkMode();
    }
  };

  useEffect(() => {
    setIsMobileMenuOpen(false);
  }, [pathname]);

  return (
    <header
      className={`fixed top-0 left-0 right-0 z-50 transition-all duration-300 bg-background/95 backdrop-blur-sm border-b border-border shadow-sm ${
        isScrolled ? "py-2" : "py-4"
      }`}
    >
      <div className="container mx-auto px-4 relative">
        <div className="flex items-center justify-between">
          <Link href="/" className="text-2xl font-bold relative z-10 text-foreground">
            <motion.div
              initial={{ opacity: 0, y: -20 }}
              animate={{ opacity: 1, y: 0 }}
              transition={{ duration: 0.5 }}
            >
              LUXEMART
            </motion.div>
          </Link>

          <nav className="hidden md:flex space-x-8">
            {navItems.map((item) => (
              <Link
                key={item.name}
                href={item.path}
                className={`text-base transition-colors relative z-10 tracking-wide ${
                  pathname === item.path
                    ? "text-primary font-bold"
                    : "text-foreground hover:text-primary font-medium"
                }`}
              >
                {item.name}
              </Link>
            ))}
          </nav>

          <div className="flex items-center space-x-2 md:space-x-4 relative z-10">
            <Button 
              variant="ghost" 
              size="icon" 
              onClick={toggleDarkMode} 
              className="relative text-foreground"
            >
              {isDarkMode ? (
                <Sun className="h-5 w-5" />
              ) : (
                <Moon className="h-5 w-5" />
              )}
              <span className="sr-only">{isDarkMode ? "Light mode" : "Dark mode"}</span>
            </Button>

            <Link href="/admin">
              <Button 
                variant="ghost"
                size="icon" 
                className="relative text-foreground"
              >
                <Settings className="h-5 w-5" />
                <span className="sr-only">Admin</span>
              </Button>
            </Link>

            <Link href="/account">
              <Button 
                variant="ghost" 
                size="icon" 
                className="relative text-foreground"
              >
                <User className="h-5 w-5" />
                <span className="sr-only">Account</span>
              </Button>
            </Link>

            <Button 
              variant="ghost" 
              size="icon" 
              className="relative text-foreground"
            >
              <ShoppingCart className="h-5 w-5" />
              <span className="absolute -top-1 -right-1 bg-primary text-primary-foreground text-xs font-bold rounded-full h-5 w-5 flex items-center justify-center shadow-md">
                3
              </span>
              <span className="sr-only">Cart</span>
            </Button>

            <Button 
              variant="ghost" 
              size="icon" 
              className="md:hidden text-foreground"
              onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
              aria-expanded={isMobileMenuOpen}
              aria-controls="mobile-menu"
            >
              {isMobileMenuOpen ? (
                <X className="h-5 w-5" />
              ) : (
                <Menu className="h-5 w-5" />
              )}
              <span className="sr-only">Menu</span>
            </Button>
          </div>
        </div>
      </div>

      <AnimatePresence>
        {isMobileMenuOpen && (
          <motion.div 
            id="mobile-menu"
            initial={{ opacity: 0, height: 0 }} 
            animate={{ opacity: 1, height: "auto" }} 
            exit={{ opacity: 0, height: 0 }}
            transition={{ duration: 0.3 }}
            className="md:hidden bg-background border-t border-border shadow-lg"
          >
            <div className="container mx-auto py-4 px-4">
              <nav className="flex flex-col space-y-1">
                {navItems.map((item) => (
                  <Link
                    key={item.name}
                    href={item.path}
                    className={`text-lg py-3 px-4 rounded-md transition-colors tracking-wide ${
                      pathname === item.path
                        ? "bg-accent text-foreground font-bold"
                        : "text-foreground hover:bg-accent/50 font-medium"
                    }`}
                  >
                    {item.name}
                  </Link>
                ))}
                <div className="pt-3 mt-3 border-t border-border">
                  <Link
                    href="/account"
                    className="flex items-center py-3 px-4 text-base font-medium text-foreground hover:bg-accent/50 rounded-md"
                  >
                    <User className="h-5 w-5 mr-3" />
                    Account
                  </Link>
                  <Link
                    href="/cart"
                    className="flex items-center py-3 px-4 text-base font-medium text-foreground hover:bg-accent/50 rounded-md"
                  >
                    <ShoppingCart className="h-5 w-5 mr-3" />
                    Cart (3)
                  </Link>
                </div>
              </nav>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </header>
  );
};

export default Header;