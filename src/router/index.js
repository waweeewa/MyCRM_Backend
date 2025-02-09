import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/Home.vue';
import Login from '../views/Login.vue'
import Device from '../views/Device.vue';
import Users from '../views/Users.vue';
import TariffModels from '../views/TariffModels.vue';
import Invoices from '../views/Invoices.vue';
import UserDevice from '../views/UserDevice.vue';
import InvoiceArchive from '../views/InvoiceArchive.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'login',
      component: Login
    },
    {
        path: '/home',
        name: 'home',
        component: Home,
        meta: {
            requiresAuth: true
        }
    },
    {
      path: '/devices',
      name: 'devices',
      component: UserDevice,
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/users',
      name: 'users',
      component: Users,
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/tariffmodels',
      name: 'tariffmodels',
      component: TariffModels,
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/invoices',
      name: 'invoices',
      component: Invoices,
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/invoicearchive',
      name: 'invoicearchive',
      component: InvoiceArchive,
      meta: {
        requiresAuth: true
      }
    },
    {
      path: '/about',
      name: 'about',
      // route level code-splitting
      // this generates a separate chunk (About.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      component: () => import('../views/AboutView.vue')
    }
  ]
})

router.beforeEach((to, from, next) => {
  if (to.matched.some(record => record.meta.requiresAuth)) {
      if (localStorage.getItem('isLoggedIn') === 'true') {
          next();
      } else {
          next({ name: 'login' });
      }
  } else {
      next();
  }
});

export default router
