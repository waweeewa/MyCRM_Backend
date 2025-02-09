<template>
    <div>
        <Header />
        <NavBar />
        <div style="display: inline">
            <Calendar style="margin-left: 10px;" v-model="selectedYear" view="year" dateFormat="yy"
                placeholder="Select Year" class="mb-2" @change="applyFilters" />
            <Calendar style="margin-left: 10px;" v-model="selectedMonth" view="month" dateFormat="mm"
                placeholder="Select Month" class="mb-2" @change="applyFilters" />
            <Dropdown style="margin-left: 10px;" v-model="selectedUser" placeholder="Select Email" class="mb-2"
                @change="applyFilters" editable />
        </div>
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
              <Button icon="pi pi-download" class="p-button-rounded p-button-success mr-2" @click="onDownload(slotProps.data)" />
              <Button icon="pi pi-pencil" class="p-button-rounded p-button-success mr-2" @click="onEdit(slotProps.data)" />
              <Button icon="pi pi-trash" class="p-button-rounded p-button-warning" @click="onDelete(slotProps.data)" />
            </template>
          </Column>
        </DataTable>
      </div>
    </div>
</template>
<script>
import NavBar from '@/components/NavBar.vue';
import Header from '../components/Header.vue';
import Calendar from 'primevue/calendar';
import Dropdown from 'primevue/dropdown';
import Column from 'primevue/column';
import DataTable from 'primevue/datatable';

export default {
    components: {
        NavBar,
        Header,
        Calendar,
        Dropdown,
        Column,
        DataTable
    }
}
</script>
<style></style>