"use client";

import { useState, useEffect } from "react";
import Link from "next/link";
import Image from "next/image";
import { motion } from "framer-motion";
import { ArrowRight, ShoppingBag, Headphones, ImageIcon } from "lucide-react";
import { Button } from "@/components/ui/button";

const HeroSection = () => {
  const [isLoaded, setIsLoaded] = useState(false);

  useEffect(() => {
    setIsLoaded(true);
  }, []);

  const containerVariants = {
    hidden: { opacity: 0 },
    visible: {
      opacity: 1,
      transition: {
        staggerChildren: 0.2,
      },
    },
  };

  const itemVariants = {
    hidden: { opacity: 0, y: 20 },
    visible: {
      opacity: 1,
      y: 0,
      transition: {
        duration: 0.5,
      },
    },
  };

  return (
    <section className="relative pt-24 pb-16 md:pt-32 md:pb-24 bg-background overflow-hidden">
      <div className="absolute inset-0 bg-gradient-to-b from-primary/5 to-transparent dark:from-primary/10 dark:to-transparent -z-10" aria-hidden="true"></div>
      
      <div 
        className="absolute top-20 right-0 w-64 h-64 rounded-full blur-3xl 
                   bg-primary/5 dark:bg-primary/10 -z-10" 
        aria-hidden="true"
      ></div>
      <div 
        className="absolute bottom-20 left-0 w-72 h-72 rounded-full blur-3xl 
                   bg-primary/5 dark:bg-primary/10 -z-10" 
        aria-hidden="true"
      ></div>
      
      <div className="container mx-auto px-4">
        <div className="grid grid-cols-1 lg:grid-cols-2 gap-12 items-center">
          <motion.div
            initial="hidden"
            animate={isLoaded ? "visible" : "hidden"}
            variants={containerVariants}
            className="max-w-xl"
          >
            <motion.span 
              variants={itemVariants}
              className="inline-block px-4 py-1.5 mb-6 text-sm font-medium rounded-full 
                         bg-primary/10 text-primary dark:bg-primary/20 dark:text-primary"
            >
              Spring Collection 2025
            </motion.span>
            
            <motion.h1 
              variants={itemVariants}
              className="text-4xl md:text-5xl lg:text-6xl font-bold leading-tight mb-6 text-foreground"
            >
              Discover Luxury for <span className="text-primary">Every Style</span>
            </motion.h1>
            
            <motion.p 
              variants={itemVariants}
              className="text-lg mb-8 text-muted-foreground"
            >
              Explore our curated collection of premium products designed for modern living. From stylish accessories to cutting-edge electronics.
            </motion.p>
            
            <motion.div 
              variants={itemVariants}
              className="flex flex-col sm:flex-row gap-4"
            >
              <Button size="lg" variant="default" className="rounded-full border border-input">
                <ShoppingBag className="mr-2 h-5 w-5" />
                Shop Now
              </Button>
              <Button variant="outline" size="lg" className="rounded-full" asChild>
                <Link href="/products">
                  View Collection
                  <ArrowRight className="ml-2 h-5 w-5" />
                </Link>
              </Button>
            </motion.div>
          </motion.div>
          
          <div className="hidden lg:block relative">
            <div className="grid grid-cols-2 gap-4">
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.2 }}
                className="w-full h-64 bg-blue-100 dark:bg-blue-900/30 rounded-xl overflow-hidden shadow-md 
                           border border-border hover:shadow-lg transition-standard"
              >
                <div className="w-full h-full flex items-center justify-center">
                  <Headphones size={48} className="text-primary" />
                  <span className="ml-2 font-medium">Premium Headphones</span>
                </div>
              </motion.div>
              
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.3 }}
                className="w-full h-48 bg-amber-100 dark:bg-amber-900/30 rounded-xl overflow-hidden shadow-md 
                           border border-border hover:shadow-lg transition-standard"
              >
                <div className="w-full h-full flex items-center justify-center">
                  <ShoppingBag size={48} className="text-primary" />
                  <span className="ml-2 font-medium">Leather Wallet</span>
                </div>
              </motion.div>
              
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.4 }}
                className="w-full h-40 bg-green-100 dark:bg-green-900/30 rounded-xl overflow-hidden shadow-md 
                           border border-border hover:shadow-lg transition-standard"
              >
                <div className="w-full h-full flex items-center justify-center">
                  <ImageIcon size={48} className="text-primary" />
                  <span className="ml-2 font-medium">Ceramic Mugs</span>
                </div>
              </motion.div>
              
              <motion.div
                initial={{ opacity: 0, y: 20 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: 0.5 }}
                className="w-full h-64 bg-purple-100 dark:bg-purple-900/30 rounded-xl overflow-hidden shadow-md 
                           border border-border hover:shadow-lg transition-standard"
              >
                <div className="w-full h-full flex items-center justify-center">
                  <Headphones size={48} className="text-primary" />
                  <span className="ml-2 font-medium">Bluetooth Speaker</span>
                </div>
              </motion.div>
            </div>
            
            <div className="absolute -bottom-6 -left-6 w-12 h-12 bg-primary/80 rounded-full"></div>
            <div className="absolute top-1/2 -right-6 w-8 h-8 bg-primary/60 rounded-full"></div>
          </div>
        </div>
      </div>
    </section>
  );
};

export default HeroSection;