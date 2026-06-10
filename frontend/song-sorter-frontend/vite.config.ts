import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    react(),
    tailwindcss()
  ],
  server: {
    proxy: {
      // Усі запити, що починаються з /api, Vite буде перехоплювати...
      '/api': {
        target: 'https://localhost:7197', // ...і непомітно пересилати на твій бекенд
        changeOrigin: true,
        secure: false, // Дозволяє працювати з локальним HTTPS (самопідписаними сертифікатами)
      }
    }
  }
})
