import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import path from "path";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      "@src": path.join(path.resolve(__dirname, "./src")),
      "@root": path.join(path.resolve(__dirname, "./")),
      "@components": path.join(path.resolve(__dirname, "./src/components")),
      "@store": path.join(path.resolve(__dirname, "./src/store")),
      "@styles": path.join(path.resolve(__dirname, "./src/styles")),
      "@images": path.join(path.resolve(__dirname, "./src/images")),
      "@utils": path.join(path.resolve(__dirname, "./src/utils")),
      "@pages": path.join(path.resolve(__dirname, "./src/pages")),
      "@public": path.join(path.resolve(__dirname, "./public")),
      "@config": path.join(path.resolve(__dirname, "./src/config")),
      "@assets": path.join(path.resolve(__dirname, "./src/assets")),
    }
  }
})
