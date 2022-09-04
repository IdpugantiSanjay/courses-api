/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'primary': {
          default: '#538D22',
          '50': '#AFE285',
          '100': '#A5DE75',
          '200': '#8FD654',
          '300': '#7ACE33',
          '400': '#66AE2A',
          '500': '#538D22',
          '600': '#386017',
          '700': '#1E330C',
          '800': '#030501',
          '900': '#143601'
        },
      }
    },
  },
  plugins: [],
}
