<template>
  <div>
    <div class="overlay" v-if="displayModal || displayModalEdit"></div>
    <div :class="{'content': !displayModal}" style="margin-left: 300px;">
      <Header />
      <NavBar />
      <Button label="Add" icon="pi pi-plus" @click="onAdd" class="mb-2" />
      <InputText style="margin-left: 10px;" v-model="searchQuery" placeholder="Search..." class="mb-2" @input="applyFilters" />
      <div class="datatable-background">
        <DataTable :value="filteredUsers" class="custom-datatable">
          <Column field="firstname" header="First Name" class="wide-column"></Column>
          <Column field="lastname" header="Last Name" class="wide-column"></Column>
          <Column field="email" header="E-mail" class="wide-column"></Column>
          <Column header="Actions">
            <template #body="slotProps">
              <Button icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="onEdit(slotProps.data)" />
              <Button icon="pi pi-trash" class="p-button-rounded p-button-warning" @click="onDelete(slotProps.data)" />
            </template>
          </Column>
        </DataTable>
      </div>
    </div>
    <Dialog header="Add User" v-model:visible="displayModal" :closable="true" :style="{ width: '850px' }" @hide="closeForm">
      <UserModal @close="closeForm" addEdit="Add"/>
    </Dialog>
    <Dialog header="Edit User" v-model:visible="displayModalEdit" :closable="true" :style="{ width: '850px' }" @hide="closeForm">
      <UserModal @close="closeForm" :currentUserData="currentUser" addEdit="Edit" />
    </Dialog>
    <Dialog header="Confirm Delete" v-model:visible="displayConfirmDeleteDialog" :modal="true" :closable="false" :style="{ width: '30vw' }">
      <h2>Are you sure you want to delete this user?</h2>
      <template #footer>
        <Button label="Cancel" @click="displayConfirmDeleteDialog = false" class="p-button-text" />
        <Button label="Delete" @click="confirmDeleteUser" class="p-button-danger" />
      </template>
    </Dialog>
  </div>
</template>

<script>
import NavBar from '../components/NavBar.vue';
import Header from '../components/Header.vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InputText from 'primevue/inputtext';
import UserModal from './UserModal.vue';
import { GetUsers, DeleteUsers } from '../services/services.js';

export default {
  components: {
    NavBar,
    Header,
    DataTable,
    Column,
    Button,
    Dialog,
    InputText,
    UserModal,
  },
  data() {
    return {
      users: [],
      displayModal: false,
      displayModalEdit: false,
      currentUser: null,
      displayConfirmDeleteDialog: false,
      userToDelete: null,
      searchQuery: '', // Add searchQuery to hold the search input
    };
  },
  computed: {
    filteredUsers() {
      if (!this.searchQuery) {
        return this.users;
      }
      const query = this.searchQuery.toLowerCase();
      return this.users.filter(user =>
        user.firstname.toLowerCase().includes(query) ||
        user.lastname.toLowerCase().includes(query) ||
        user.email.toLowerCase().includes(query)
      );
    },
  },
  methods: {
    onAdd() {
      this.displayModal = true;
      this.currentUserData = null;
    },
    async onEdit(user) {
      this.currentUser = user;
      this.displayModalEdit = true;
    },
    onDelete(user) {
      this.userToDelete = user;
      this.displayConfirmDeleteDialog = true;
    },
    async confirmDeleteUser() {
      console.log('Deleting user:', this.userToDelete);
      await DeleteUsers(this.userToDelete.uId);
      await this.fetchUsers();
      this.displayConfirmDeleteDialog = false;
      this.userToDelete = null;
    },
    closeForm() {
      this.displayModal = false;
      this.displayModalEdit = false;
      this.fetchUsers();
    },
    async fetchUsers() {
      try {
        const response = await GetUsers();
        if (response && response.data) {
          this.users = response.data.map(user => ({
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
        }
      } catch (error) {
        console.error('Error fetching users:', error);
      }
    },
    applyFilters() {
      // This method is called when the search input changes
      // The filtering is handled by the computed property 'filteredUsers'
      console.log('Search query:', this.searchQuery);
    },
  },
  mounted() {
    this.fetchUsers();
  },
};
</script>

<style>
.mb-2 {
  margin-bottom: 0.5rem;
}
.mr-2 {
  margin-right: 0.5rem;
}
.wide-column {
  min-width: 15rem;
  max-width: 20rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.custom-datatable .p-datatable-tbody > tr > td {
  padding: 0.5rem;
  line-height: 1.2;
  height: 70px;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
}
.datatable-background {
  background-color: #3a3a3a;
  border-radius: 15px;
  padding: 1rem;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}
.custom-datatable .p-datatable-thead > tr > th {
  background-color: #3a3a3a;
  color: white;
}
.overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(8px);
  z-index: 999;
}
</style>