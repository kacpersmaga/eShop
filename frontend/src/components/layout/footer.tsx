"use client";

import Link from "next/link";
import { Facebook, Instagram, Twitter, Youtube, Mail } from "lucide-react";

const Footer = () => {
  return (
    <footer className="bg-background dark:bg-background border-t border-border pt-8 pb-6">
      <div className="container mx-auto px-4">
        <div className="grid grid-cols-1 xs:grid-cols-2 md:grid-cols-4 gap-y-8 gap-x-4 mb-8 md:mb-12">
          <div className="text-center xs:text-left">
            <h4 className="font-semibold text-base sm:text-lg mb-3 text-foreground">Shop</h4>
            <ul className="space-y-2.5">
              <li>
                <Link
                  href="/shop"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  All Products
                </Link>
              </li>
              <li>
                <Link
                  href="/new-arrivals"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  New Arrivals
                </Link>
              </li>
              <li>
                <Link
                  href="/sale"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  Sale
                </Link>
              </li>
              <li>
                <Link
                  href="/categories"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  Categories
                </Link>
              </li>
            </ul>
          </div>
          
          <div className="text-center xs:text-left">
            <h4 className="font-semibold text-base sm:text-lg mb-3 text-foreground">Customer Service</h4>
            <ul className="space-y-2.5">
              <li>
                <Link
                  href="/contact"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  Contact Us
                </Link>
              </li>
              <li>
                <Link
                  href="/shipping"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  Shipping & Returns
                </Link>
              </li>
              <li>
                <Link
                  href="/faq"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  FAQ
                </Link>
              </li>
              <li>
                <Link
                  href="/size-guide"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  Size Guide
                </Link>
              </li>
            </ul>
          </div>
          
          <div className="text-center xs:text-left">
            <h4 className="font-semibold text-base sm:text-lg mb-3 text-foreground">Company</h4>
            <ul className="space-y-2.5">
              <li>
                <Link
                  href="/about"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  About Us
                </Link>
              </li>
              <li>
                <Link
                  href="/careers"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  Careers
                </Link>
              </li>
              <li>
                <Link
                  href="/sustainability"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  Sustainability
                </Link>
              </li>
              <li>
                <Link
                  href="/press"
                  className="text-sm text-muted-foreground hover:text-primary transition-colors"
                >
                  Press
                </Link>
              </li>
            </ul>
          </div>
          
          <div className="text-center xs:text-left xs:col-span-2 md:col-span-1">
            <h4 className="font-semibold text-base sm:text-lg mb-3 text-foreground">Follow Us</h4>
            
            <div className="flex justify-center xs:justify-start gap-4 mb-4">
              <Link
                href="https://facebook.com"
                target="_blank"
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary bg-muted/30 p-2.5 rounded-full transition-colors"
                aria-label="Facebook"
              >
                <Facebook className="h-5 w-5" />
              </Link>
              <Link
                href="https://instagram.com"
                target="_blank"
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary bg-muted/30 p-2.5 rounded-full transition-colors"
                aria-label="Instagram"
              >
                <Instagram className="h-5 w-5" />
              </Link>
              <Link
                href="https://twitter.com"
                target="_blank"
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary bg-muted/30 p-2.5 rounded-full transition-colors"
                aria-label="Twitter"
              >
                <Twitter className="h-5 w-5" />
              </Link>
              <Link
                href="https://youtube.com"
                target="_blank"
                rel="noopener noreferrer"
                className="text-muted-foreground hover:text-primary bg-muted/30 p-2.5 rounded-full transition-colors"
                aria-label="YouTube"
              >
                <Youtube className="h-5 w-5" />
              </Link>
            </div>
            
            <p className="text-muted-foreground flex items-center justify-center xs:justify-start text-sm">
              <Mail className="h-4 w-4 inline-block mr-2" />
              support@luxemart.com
            </p>
          </div>
        </div>

        <div className="pt-6 border-t border-border">
          <div className="flex flex-col items-center space-y-4 md:space-y-0 md:flex-row md:justify-between">
            <div className="mb-2 md:mb-0">
              <Link href="/" className="text-xl font-bold text-foreground">
                LUXEMART
              </Link>
            </div>
            
            <div className="flex flex-wrap justify-center gap-x-4 gap-y-2 text-xs text-muted-foreground">
              <Link
                href="/privacy-policy"
                className="hover:text-primary transition-colors"
              >
                Privacy Policy
              </Link>
              <Link
                href="/terms-of-service"
                className="hover:text-primary transition-colors"
              >
                Terms of Service
              </Link>
              <Link
                href="/cookie-policy"
                className="hover:text-primary transition-colors"
              >
                Cookie Policy
              </Link>
            </div>
          </div>
          
          <div className="text-center mt-6 text-xs text-muted-foreground">
            &copy; {new Date().getFullYear()} LUXEMART. All rights reserved.
          </div>
        </div>
      </div>
    </footer>
  );
};

export default Footer;