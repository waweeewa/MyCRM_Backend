import './assets/main.css'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import 'vue3-toastify/dist/index.css';
import PrimeVue from "primevue/config";
import 'primevue/resources/themes/vela-blue/theme.css'; // Dark Theme
import 'primevue/resources/primevue.min.css';           // Core CSS
import 'primeicons/primeicons.css';  

const app = createApp(App)

app.use(router).use(PrimeVue)

app.mount('#app')
