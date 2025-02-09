<template>
  <div>
    <div class="overlay" v-if="displayModal || displayModalEdit"></div>
    <div :class="{'content': !displayModal}" style="margin-left: 100px">
      <Header />
      <NavBar />
      <Button label="Add" icon="pi pi-plus" @click="onAdd" class="mb-2" style="overflow-y: auto; margin-top: 300px"/>
        <Calendar style="margin-left: 10px;" v-model="selectedYear" view="year" dateFormat="yy" placeholder="Select Year" class="mb-2" @change="applyFilters" />
        <Calendar style="margin-left: 10px;" v-model="selectedMonth" view="month" dateFormat="mm" placeholder="Select Month" class="mb-2" @change="applyFilters" />
        <InputText style="margin-left: 10px;" v-model="selectedEmail" placeholder="Type Email" class="mb-2" @input="applyFilters" />
      <div class="datatable-background">
        <DataTable :value="filteredTariffs" class="custom-datatable">
          <Column field="email" header="E-mail" class="wide-column"></Column>
          <Column field="month" header="Month" style="min-width: 10rem;"></Column>
          <Column field="year" header="Year" style="min-width: 10rem;"></Column>
          <Column field="usedPower" header="Total Used Power(kWh)" style="min-width: 10rem;"></Column>
          <Column field="price" header="Old Balance($)" style="min-width: 10rem;"></Column>
          <Column field="new" header="New Balance($)" style="min-width: 10rem;"></Column>
          <Column field="unit" header="Unit Price($)" style="min-width: 10rem;"></Column>
          <Column header="Actions">
            <template #body="slotProps">
              <Button v-if="slotProps.data.isArchive == false" icon="pi pi-download"  class="p-button-rounded p-button-success mr-2" @click="onDownload(slotProps.data)" />
              <Button v-if="slotProps.data.isArchive == true" icon="pi pi-refresh"  class="p-button-rounded p-button-success mr-2" @click="onDownload(slotProps.data)" />
              <Button icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="onEdit(slotProps.data)" />
              <Button icon="pi pi-trash" class="p-button-rounded p-button-warning" @click="onDelete(slotProps.data)" />
            </template>
          </Column>
        </DataTable>
      </div>
    </div>
    <Dialog header="Add Invoice" v-model:visible="displayModal" :closable="true" :style="{ width: '600px' }" @hide="closeForm">
      <InvoiceModal @close="closeForm" addEdit="Add"/>
    </Dialog>
    <Dialog header="Edit Invoice" v-model:visible="displayModalEdit" :closable="true" :style="{ width: '600px' }" @hide="closeForm">
      <InvoiceModal @close="closeForm" :tariffData="currentTariff" addEdit="Edit" />
    </Dialog>
    <Dialog header="Confirm Delete" v-model:visible="displayConfirmDeleteDialog" :modal="true" :closable="false" :style="{ width: '30vw' }">
      <h2>Are you sure you want to delete this tariff?</h2>
      <template #footer>
        <Button label="Cancel" @click="displayConfirmDeleteDialog = false" class="p-button-text" />
        <Button label="Delete" @click="confirmDeleteTariff" class="p-button-danger" />
      </template>
    </Dialog>
  </div>
</template>

<script>
import NavBar from '../components/NavBar.vue';
import Header from '../components/Header.vue';
import DataTable from 'primevue/datatable';
import Column from 'primevue/column';
import InputText from 'primevue/inputtext';
import Calendar from 'primevue/calendar';
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import InvoiceModal from './InvoiceModal.vue';
import { GetInvoices, DownloadBill, DeleteInvoices } from '../services/services.js';

export default {
  components: {
    NavBar,
    Header,
    DataTable,
    Column,
    Button,
    Dialog,
    InputText,
    Calendar,
    InvoiceModal,
  },
  data() {
    return {
      tariffs: [],
      displayModal: false,
      displayModalEdit: false,
      currentTariff: null,
      displayConfirmDeleteDialog: false,
      tariffToDelete: null,
      selectedYear: null,
      selectedMonth: null,
      selectedEmail: '',
    };
  },
  computed: {
    uniqueTariffs() {
      const seen = new Set();
      return this.tariffs.filter(tariff => {
        const identifier = `${tariff.year}-${tariff.month}`;
        return !seen.has(identifier) && seen.add(identifier);
      });
    },
    filteredTariffs() {
      return this.uniqueTariffs.filter(tariff => {
        const yearMatch = !this.selectedYear || tariff.year === this.selectedYear.getFullYear();
        const monthMatch = !this.selectedMonth || tariff.month === this.selectedMonth.getMonth() + 1;
        const emailMatch = !this.selectedEmail || tariff.email.includes(this.selectedEmail);
        return yearMatch && monthMatch && emailMatch;
      });
    }
  },
  methods: {
    onAdd() {
      this.displayModal = true;
      this.currentTariff = null;
    },
    onEdit(tariff) {
      console.log('Edit button clicked', tariff);
      this.currentTariff = tariff;
      this.displayModalEdit = true;
    },
    onDelete(tariff) {
      this.tariffToDelete = tariff;
      this.displayConfirmDeleteDialog = true;
    },
    async onDownload(tariff) {
      await DownloadBill(tariff.userId, tariff.month, tariff.year);
      console.log('Download button clicked', tariff);
    },
    async confirmDeleteTariff() {
      console.log('Deleting tariff:', this.tariffToDelete);
      await DeleteInvoices(this.tariffToDelete.userId, this.tariffToDelete.month, this.tariffToDelete.year);
      this.displayConfirmDeleteDialog = false;
      this.tariffToDelete = null;
      this.fetchInvoices();
    },
    async closeForm() {
      this.displayModal = false;
      this.displayModalEdit = false;
      window.location.reload();
      console.log('Close button clicked');
    },
    async fetchInvoices() {
      try {
        const response = await GetInvoices();
        if (response && response.data) {
          this.tariffs = response.data.map(device => ({
            id: device.billId,
            userId: device.userId,
            email: device.email,
            month: device.month,
            year: device.year,
            usedPower: device.usedPower,
            paidAmount: device.paidAmount,
            price: device.totalAmount,
            new: device.totalAmount - device.paidAmount,
            unit: device.tariff,
            isArchive: device.isArchive
          }));
          if (!this.isAdmin) {
            this.tariffs = this.tariffs.filter(invoice => invoice.email === this.realEmail);
          }
          this.populateFilters();
        }
      } catch (error) {
        console.error('Failed to fetch invoices:', error);
      }
    },
    populateFilters() {
      // Populate years
      const uniqueYears = [...new Set(this.tariffs.map(t => t.year))];
      this.years = uniqueYears.map(year => ({ name: year.toString(), value: year }));

      // Populate months
      const uniqueMonths = [...new Set(this.tariffs.map(t => t.month))];
      this.months = uniqueMonths.map(month => ({ name: month, value: month }));

      // Populate emails
      const uniqueEmails = [...new Set(this.tariffs.map(t => t.email))];
      this.emails = uniqueEmails.map(email => ({ name: email, value: email }));
    },
    applyFilters() {
      // This method is called when a filter changes
      // The filtering is handled by the computed property 'filteredTariffs'
      console.log('Filters applied:', { year: this.selectedYear, month: this.selectedMonth, email: this.selectedEmail });
    },
  },
  mounted() {
    this.fetchInvoices();
  },
  created() {
    this.isAdmin = localStorage.getItem('isAdmin') === '1';
    this.realEmail = localStorage.getItem('email');
  }
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
    min-width: 15rem; /* Adjust this value as needed */
    max-width: 20rem; /* Adjust this value as needed */
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
.filter-container {
  display: flex;
  justify-content: flex-start;
  margin-bottom: 1rem;
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
.custom-datatable .p-datatable-thead > tr > th {
  text-align: left; /* Adjust text alignment as needed */
  padding: 8px; /* Adjust padding to ensure alignment with fields */
}

.custom-datatable .p-datatable-tbody > tr > td {
  text-align: left; /* Ensure text alignment matches header */
  padding: 8px; /* Adjust padding to align with header */
}

.p-autocomplete-loader {
  display: none !important;
}
</style>