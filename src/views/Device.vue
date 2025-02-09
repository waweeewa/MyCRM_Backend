<template>
    <div>
      <div class="overlay" v-if="displayModal || displayModalEdit"></div>
      <div :class="{'content': !displayModal}" style="margin-left: 400px;">
        <Header />
        <NavBar />
        <Button label="Add" icon="pi pi-plus" @click="onAdd" class="mb-2" style="overflow-y: auto; margin-top: 300px"/>
        <div class="datatable-background">
          <DataTable :value="devices" class="custom-datatable">
            <Column field="model" header="Model" class="wide-column"></Column>
            <Column field="hwid" header="HWID" class="wide-column"></Column>
            <Column header="Actions">
              <template #body="slotProps">
                <Button icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="onEdit(slotProps.data)" />
                <Button icon="pi pi-trash" class="p-button-rounded p-button-warning" @click="onDelete(slotProps.data)" />
              </template>
            </Column>
          </DataTable>
        </div>
      </div>
      <Dialog header="Add Device" v-model:visible="displayModal" :closable="true" :style="{ width: '600px' }" @hide="closeForm">
        <DeviceModal @close="closeForm" addEdit="Add"/>
      </Dialog>
      <Dialog header="Edit Device" v-model:visible="displayModalEdit" :closable="true" :style="{ width: '600px' }" @hide="closeForm">
        <DeviceModal @close="closeForm" :deviceData="currentDevice" addEdit="Edit" />
      </Dialog>
      <Dialog header="Confirm Delete" v-model:visible="displayConfirmDeleteDialog" :modal="true" :closable="false" :style="{ width: '30vw' }">
        <h2>Are you sure you want to delete this device?</h2>
        <template #footer>
      <Button label="Cancel" @click="displayConfirmDeleteDialog = false" class="p-button-text" />
      <Button label="Delete" @click="confirmDeleteDevice" class="p-button-danger" />
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
  import DeviceModal from './DeviceModal.vue';
  import { GetVerifiedDevices, DeleteVerifiedDevices } from '../services/services.js'
  
  export default {
    components: {
      NavBar,
      Header,
      DataTable,
      Column,
      Button,
      Dialog,
      DeviceModal,
    },
    data() {
      return {
        devices: [],
        displayModal: false,
        displayModalEdit: false,
        currentDevice: null,
        displayConfirmDeleteDialog: false,
        deviceToDelete: null,
      };
    },
    methods: {
      onAdd() {
        this.displayModal = true;
        this.currentDevice = null;
      },
      onEdit(device) {
        console.log('Edit button clicked', device);
        this.currentDevice = device;
        this.displayModalEdit = true;
      },
      onDelete(device) {
        this.deviceToDelete = device;
        this.displayConfirmDeleteDialog = true;
      },
      async confirmDeleteDevice() {
        await DeleteVerifiedDevices(this.deviceToDelete.id,this.deviceToDelete.model,this.deviceToDelete.hwid)
        console.log('Deleting device:', this.deviceToDelete);
        this.displayConfirmDeleteDialog = false;
        this.deviceToDelete = null;
        window.location.reload();
      },
      async closeForm() {
        this.displayModal = false;
        this.displayModalEdit = false;
        window.location.reload();
        console.log('Close button clicked');
      },
      async fetchDevices() {
        try {
          const response = await GetVerifiedDevices();
          if (response && response.data.result) {
            this.devices = response.data.result.map(device => ({
              id: device.vdId,
              model: device.model,
              hwid: device.serialNum,
            }));
          }
          console.log(this.devices);
        } catch (error) {
          console.error('Failed to fetch devices:', error);
        }
      },
    },
    mounted() {
      this.fetchDevices();
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

/* Apply specific width to columns with class "wide-column" */
.wide-column {
    min-width: 40rem; /* Adjust this value as needed */
    max-width: 40rem; /* Adjust this value as needed */
    white-space: nowrap; /* Prevent content wrapping */
    overflow: hidden; /* Hide overflow content */
    text-overflow: ellipsis; /* Show ellipsis for overflow text */
}

/* Ensure consistent cell height and padding */
.custom-datatable .p-datatable-tbody > tr > td {
    padding: 0.5rem; /* Adjust padding as needed */
    line-height: 1.2; /* Adjust line-height as needed */
    height: 70px; /* Ensure height is automatic */
    white-space: nowrap; /* Prevent content wrapping in all cells */
    overflow: hidden; /* Hide overflow content */
    text-overflow: ellipsis; /* Show ellipsis for overflow text */
}

.datatable-background {
    background-color: #3a3a3a; /* Your desired background color */
    border-radius: 15px; /* Adjust for desired roundness */
    padding: 1rem; /* Add some padding around the DataTable */
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); /* Optional: Adds a shadow for depth */
    overflow: hidden; /* Ensures the rounded corners are applied to the child components */
}
.custom-datatable .p-datatable-thead > tr > th {
    background-color: #3a3a3a; /* Your desired header background color */
    color: white; /* Adjust text color as needed */
}
/* Dark mode adjustments */
.datatable-background {
    background-color: #232323; /* Darker background for dark mode */
    color: #f0f0f0; /* Lighter text color for better readability in dark mode */
}

/* Hover effect for rows in dark mode */
.custom-datatable .p-datatable-tbody > tr:hover {
    background-color: #3a3a3a; /* Slightly lighter than row color for hover effect */
}

/* Alternating row colors in dark mode */
.custom-datatable .p-datatable-tbody > tr:nth-child(odd) {
    background-color: #2a2a2a; /* Dark color for odd rows */
}

.custom-datatable .p-datatable-tbody > tr:nth-child(even) {
    background-color: #202020; /* Even darker color for even rows */
}

/* Adjust header colors for dark mode */
.custom-datatable .p-datatable-thead > tr > th {
    background-color: #232323; /* Dark background for headers */
    color: #f0f0f0; /* Light text color for headers */
}
.overlay {
    position: fixed; /* Cover the entire screen */
    top: 0;
    left: 0;
    right: 0;
    bottom: 0;
    background: rgba(0, 0, 0, 0.5); /* Semi-transparent black background */
    backdrop-filter: blur(8px); /* Apply blur to the overlay itself */
    z-index: 999; /* Ensure it's above other content but below the modal */
}
.custom-datatable .p-datatable-tbody > tr > td {
  text-align: left; /* Ensure text alignment matches header */
  padding: 8px; /* Adjust padding to align with header */
}
.custom-datatable .p-datatable-thead > tr > th {
  text-align: left; /* Adjust text alignment as needed */
  padding: 8px; /* Adjust padding to ensure alignment with fields */
}
</style>
