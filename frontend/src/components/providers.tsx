"use client";

import { ThemeProvider } from "@/components/ui/theme-provider";
import { ToastContextProvider } from "@/components/ui/toast";

export function Providers({ children }: { children: React.ReactNode }) {
  return (
    <ThemeProvider defaultTheme="system">
      <ToastContextProvider>
        {children}
      </ToastContextProvider>
    </ThemeProvider>
  );
}