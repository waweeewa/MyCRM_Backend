<template>
  <div>
    <div class="overlay" v-if="displayModal || displayModalEdit"></div>
    <div :class="{'content': !displayModal}" style="margin-left: 300px;">
      <Header />
      <NavBar />
      <div class="actions-container">
        <Button label="Add" icon="pi pi-plus" @click="onAdd" class="mr-2" />
        <AutoComplete 
          v-model="emailFilter" 
          @complete="searchEmails" 
          placeholder="Filter by email" 
          field="email" 
          class="email-autocomplete"
          :disabled="!isAdmin"
        />
        <Calendar v-model="selectedFromDate" dateFormat="dd-mm-yy" placeholder="From Date" @change="applyFilters" class="mr-2" />
        <Calendar v-model="selectedToDate" dateFormat="dd-mm-yy" placeholder="To Date" @change="applyFilters" />
      </div>
      <div class="datatable-background">
        <DataTable :value="filteredDevices" class="custom-datatable">
          <Column field="email" header="E-mail" class="wide-column"></Column>
          <Column field="name" header="Device name" class="wide-column"></Column>
          <Column field="from_date" header="From date" class="wide-column"></Column>
          <Column field="to_date" header="To date" class="wide-column"></Column>
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
      <UserDeviceModal @close="closeForm" addEdit="Add"/>
    </Dialog>
    <Dialog header="Edit User" v-model:visible="displayModalEdit" :closable="true" :style="{ width: '850px' }" @hide="closeForm">
      <UserDeviceModal @close="closeForm" :currentUserData="currentUser" addEdit="Edit" />
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
import UserDeviceModal from './UserDeviceModal.vue';
import AutoComplete from 'primevue/autocomplete';
import Calendar from 'primevue/calendar';
import { GetDevices, DeleteDevice } from '../services/services.js';

export default {
  components: {
    NavBar,
    Header,
    DataTable,
    Column,
    Button,
    Dialog,
    UserDeviceModal,
    AutoComplete,
    Calendar,
  },
  data() {
    return {
      emailFilter: '',
      devices: [],
      filteredEmails: [],
      displayModal: false,
      displayModalEdit: false,
      currentUser: null,
      displayConfirmDeleteDialog: false,
      userToDelete: null,
      isAdmin: localStorage.getItem('isAdmin') === '1',
      userEmail: localStorage.getItem('email'),
      selectedMonth: null,
      selectedYear: null,
      selectedFromDate: null,
    selectedToDate: null,
    };
  },
  computed: {
  filteredDevices() {
    let devices = this.devices;
    if (!this.isAdmin) {
      devices = devices.filter(device => device.email === this.userEmail);
    }
    if (this.emailFilter) {
      devices = devices.filter(device => device.email.includes(this.emailFilter));
    }
    if (this.selectedFromDate && this.selectedToDate) {
      const fromDateFilter = new Date(this.selectedFromDate).getTime();
      const toDateFilter = new Date(this.selectedToDate).getTime();

      devices = devices.filter(device => {
        const fromDateDevice = new Date(device.from_date.split('-').reverse().join('-')).getTime();
        const toDateDevice = new Date(device.to_date.split('-').reverse().join('-')).getTime();

        // Check if there is any overlap in date ranges
        return (
          (fromDateFilter <= toDateDevice && toDateFilter >= fromDateDevice) ||
          (fromDateDevice <= toDateFilter && toDateDevice >= fromDateFilter)
        );
      });
    }
    return devices;
  },
},
  methods: {
    onAdd() {
      this.displayModal = true;
      this.currentUserData = null;
    },
    async onEdit(device) {
      console.log('Editing user:', device);
      this.currentUser = { ...device }; // Ensure udId is included
      console.log('Current user:', this.currentUser);
      this.displayModalEdit = true;
    },
    onDelete(device) {
      this.userToDelete = device;
      this.displayConfirmDeleteDialog = true;
    },
    async confirmDeleteUser() {
      console.log('Deleting user:', this.userToDelete);
      await DeleteDevice(this.userToDelete.udId);
      await this.fetchDevices();
      this.displayConfirmDeleteDialog = false;
      this.userToDelete = null;
    },
    closeForm() {
      this.displayModal = false;
      this.displayModalEdit = false;
      this.fetchDevices();
    },
    async fetchDevices() {
  try {
    const response = await GetDevices();
    console.log('Response:', response.data);
    if (response && response.data) {
      this.devices = response.data.map(device => ({
        udId: device.udId,
        email: device.email,
        name: device.name,
        from_date: this.formatDate(device.from_date),
        to_date: this.formatDate(device.to_date),
      }));
    }
  } catch (error) {
    console.error('Error fetching devices:', error);
  }
},
formatDate(dateString) {
  const date = new Date(dateString);
  const day = String(date.getDate()).padStart(2, '0');
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const year = String(date.getFullYear());
  return `${day}-${month}-${year}`;
},
    searchEmails(event) {
      const query = event.query.toLowerCase();
      this.filteredEmails = this.devices
        .map(device => device.email)
        .filter(email => email.toLowerCase().includes(query));
    },
    applyFilters() {
  console.log('Filters applied:', { fromDate: this.selectedFromDate, toDate: this.selectedToDate, email: this.emailFilter });
},
  },
  mounted() {
    this.fetchDevices();
  },
};
</script>

<style scoped>
.actions-container {
  display: flex;
  align-items: center;
  margin-bottom: 10px;
}


.p-autocomplete-panel {
  width: 400px !important;
}

.p-autocomplete-loader {
  display: none !important;
}

.datatable-background {
  transform: translateX(-100px);
}

.actions-container {
  transform: translateX(-100px);
}
</style>