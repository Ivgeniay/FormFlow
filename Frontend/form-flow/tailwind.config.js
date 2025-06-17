/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{js,jsx,ts,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        primary: 'var(--primary-color)',
        background: 'var(--background-color)',
        surface: 'var(--surface-color)',
        text: 'var(--text-color)',
        textMuted: 'var(--text-muted-color)',
        border: 'var(--border-color)',
        success: 'var(--success-color)',
        warning: 'var(--warning-color)',
        error: 'var(--error-color)',
      }
    },
  },
  plugins: [],
  darkMode: 'class',
}