<template>
  <header class="header">
    <img src="../img/logo.png" alt="Logo" class="logo">
    <div class="menu-and-email">
      <span class="user-email">{{ userEmail }}</span>
      <Menu :model="items" popup ref="menu" />
      <Button icon="pi pi-bars" @click="toggleMenu" class="custom-button" />
    </div>
  </header>
  <Dialog v-model:visible="displayModal" :style="{ width: '850px' }">
    <UserModal :currentUserData="user" @close="displayModal = false" addEdit="Edit"/>
  </Dialog>
  <Dialog v-model:visible="reportModalVisible" :style="{ width: '850px' }" draggable={false}>
    <ElectricitySummaryModal :userId="userID" :visible="reportModalVisible" @update:visible="reportModalVisible = $event" />
  </Dialog>
</template>

<script>
import Menu from 'primevue/menu';
import Button from 'primevue/button';
import { ref, computed } from 'vue';
import { GetUser } from '../services/services.js';
import UserModal from '../views/UserModal.vue';
import Dialog from 'primevue/dialog';
import ElectricitySummaryModal from '../views/ElectricitySummaryModal.vue';

export default {
  components: {
    Menu,
    Button,
    UserModal,
    Dialog,
    ElectricitySummaryModal
  },
  setup() {
    const menu = ref(null);

    const toggleMenu = (event) => {
      menu.value.toggle(event);
    };

    // Example: Fetching email from localStorage or a store
    const userEmail = computed(() => localStorage.getItem('email'));
    const userID = localStorage.getItem('userID');

    return { menu, toggleMenu, userEmail, userID };
  },
  data() {
    return {
      items: [
        {
          label: 'Profile',
          icon: 'pi pi-user',
          command: () => {
            this.profile();
          }
        },
        {
          label: 'Report',
          icon: 'pi pi-book',
          command: () => {
            this.report();
          }
        },
        {
          label: 'Sign out',
          icon: 'pi pi-sign-out',
          command: () => {
            this.signOut();
          }
        }
        // Add more items as needed
      ],
      displayModal: false,
      reportModalVisible: false,
      user: {}
    };
  },
  methods: {
    signOut() {
      localStorage.removeItem('userID');
      localStorage.removeItem('isLoggedIn');
      localStorage.removeItem('isAdmin');
      localStorage.removeItem('email');
      localStorage.removeItem('tariffId');
      this.$router.push('/');
    },
     async profile() {
      try {
        const response = await GetUser(localStorage.getItem('userID'));
        console.log('Response:', response);
        if (response && Array.isArray(response.data)) {
          this.user = response.data.map(user => ({
            uId: user.uId,
            firstname: user.firstName,
            lastname: user.lastName,
            email: user.email,
            password: user.password,
            address: user.address,
            city: user.city,
            zipcode: user.zipcode,
            country: user.country,
            birthdate: user.birthdate,
            tarriff: user.tariffId,
            admincheck: user.admincheck,
          }));
        } else if (response && response.data) {
          this.user = {
            uId: response.data.result.uId,
            firstname: response.data.result.firstName,
            lastname: response.data.result.lastName,
            email: response.data.result.email,
            password: response.data.result.password,
            address: response.data.result.address,
            city: response.data.result.city,
            zipcode: response.data.result.zipcode,
            country: response.data.result.country,
            birthdate: response.data.result.birthdate,
            tarriff: response.data.result.tariffId,
            admincheck: response.data.result.admincheck,
          };
        }
        console.log('User:', this.user);
        this.displayModal = true;
      } catch (error) {
        console.error('Error fetching user data:', error);
      }
    },
    async report() {
      try{
        this.reportModalVisible = true;
      } catch (error) {
        console.error('Error opening report modal:', error);
      }
    }
  }
};
</script>

<style scoped>
/* Existing styles */
.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: fixed;
  width: 100%;
  top: 0;
  left: 0;
  background-color: rgb(19, 19, 19);
  border-bottom: 1px solid #444;
  padding: 10px;
  z-index: 1000;
}

.logo {
  height: 100px;
  width: 200px;
}

/* New or adjusted styles */
.menu-and-email {
  display: flex;
  align-items: center;
}

.user-email {
  margin-right: 20px; /* Adjust spacing as needed */
  color: white; /* Adjust color as needed */
  font-size: 20px; /* Adjust font size as needed */
}
</style>