<template>
  <div>
    <div class="overlay" v-if="displayModal || displayModalEdit"></div>
    <div :class="{'content': !displayModal}" style="margin-left: 400px">
      <Header />
      <NavBar />
      <Button label="Add" icon="pi pi-plus" @click="onAdd" class="mb-2" style="overflow-y: auto; margin-top: 300px;" />
      <InputText style="margin-left: 10px;" v-model="searchQuery" placeholder="Search Tariff Models..." class="mb-2" @input="applyFilters" />
      <div class="datatable-background">
        <DataTable :value="filteredTariffs" class="custom-datatable">
          <Column field="id" header="ID" style="min-width: 5rem;"></Column>
          <Column field="tariffModel" header="Tariff Model" class="wide-column"></Column>
          <Column field="price" header="Price" class="wide-column"></Column>
          <Column header="Actions">
            <template #body="slotProps">
              <Button icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="onEdit(slotProps.data)" />
              <Button icon="pi pi-trash" class="p-button-rounded p-button-warning" @click="onDelete(slotProps.data)" />
            </template>
          </Column>
        </DataTable>
      </div>
    </div>
    <Dialog header="Add Tariff" v-model:visible="displayModal" :closable="true" :style="{ width: '600px' }" @hide="closeForm">
      <TariffModal @close="closeForm" addEdit="Add"/>
    </Dialog>
    <Dialog header="Edit Tariff" v-model:visible="displayModalEdit" :closable="true" :style="{ width: '600px' }" @hide="closeForm">
      <TariffModal @close="closeForm" :tariffData="currentTariff" addEdit="Edit" />
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
import Button from 'primevue/button';
import Dialog from 'primevue/dialog';
import TariffModal from './TariffModal.vue';
import InputText from 'primevue/inputtext';
import { GetTariffs, DeleteTariffs } from '../services/services.js';

export default {
  components: {
    NavBar,
    Header,
    DataTable,
    Column,
    Button,
    Dialog,
    TariffModal,
    InputText,
  },
  data() {
    return {
      tariffs: [],
      displayModal: false,
      displayModalEdit: false,
      currentTariff: null,
      displayConfirmDeleteDialog: false,
      tariffToDelete: null,
      searchQuery: '', // Add searchQuery to hold the search input
    };
  },
  computed: {
    filteredTariffs() {
      if (!this.searchQuery) {
        return this.tariffs;
      }
      const query = this.searchQuery.toLowerCase();
      return this.tariffs.filter(tariff =>
        tariff.tariffModel.toLowerCase().includes(query) ||
        tariff.price.toString().toLowerCase().includes(query)
      );
    },
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
    async confirmDeleteTariff() {
      await DeleteTariffs(this.tariffToDelete.id, this.tariffToDelete.tariffModel, this.tariffToDelete.price);
      console.log('Deleting tariff:', this.tariffToDelete);
      this.displayConfirmDeleteDialog = false;
      this.tariffToDelete = null;
      window.location.reload();
    },
    async closeForm() {
      this.displayModal = false;
      this.displayModalEdit = false;
      window.location.reload();
      console.log('Close button clicked');
    },
    async fetchTariffs() {
      try {
        const response = await GetTariffs();
        if (response && response.data.result) {
          this.tariffs = response.data.result.map(device => ({
            id: device.tId,
            tariffModel: device.name,
            price: device.price,
          }));
        }
        console.log(this.tariffs);
      } catch (error) {
        console.error('Failed to fetch tariffs:', error);
      }
    },
    applyFilters() {
      // This method is called when the search input changes
      // The filtering is handled by the computed property 'filteredTariffs'
      console.log('Search query:', this.searchQuery);
    },
  },
  mounted() {
    this.fetchTariffs();
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
  min-width: 29rem;
  max-width: 29rem;
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
.custom-datatable .p-datatable-thead > tr > th {
  text-align: left;
  padding: 8px;
}
.custom-datatable .p-datatable-tbody > tr > td {
  text-align: left;
  padding: 8px;
}
</style>