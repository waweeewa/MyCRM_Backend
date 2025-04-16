<template>
  <Dialog v-model:visible="internalVisible" :modal="true" :style="{ width: '850px' }" header="Electricity Summary">
    <!-- Updated admin Dropdown with optionValue -->
    <div v-if="isAdmin" style="margin-bottom: 10px;">
      <Dropdown :options="users" optionLabel="label" optionValue="value" v-model="selectedUser" placeholder="Select a User" @change="fetchSummary" />
    </div>
    <div class="summary-container">
      <div class="summary-box" v-for="(item, index) in summaryItems" :key="index">
        <h3>{{ item.title }}</h3>
        <p>{{ item.value }}</p>
      </div>
    </div>
    <div class="chart-section">
      <div class="graph-container">
        <Apexchart type="bar" height="300" :options="chartOptions" :series="chartSeries" />
      </div>
    </div>
    <div class="modalActions">
      <Button label="Close" @click="closeModal" class="p-button-text" />
    </div>
  </Dialog>
</template>

<script>
import { ref, watch, onMounted, computed } from 'vue';
import Dialog from 'primevue/dialog';
import Button from 'primevue/button';
import Apexchart from 'vue3-apexcharts';
import Dropdown from 'primevue/dropdown';
import { GetElectricitySummary, GetPartners } from '../services/services.js';

export default {
  name: "ElectricitySummaryModal",
  components: {
    Dialog,
    Button,
    Apexchart,
    Dropdown
  },
  props: {
    userId: { type: [String, Number], required: true },
    visible: { type: Boolean, required: true }
  },
  emits: ['update:visible'],
  setup(props, { emit }) {
    // computed property to bind v-model on visible prop
    const internalVisible = computed({
      get: () => props.visible,
      set: (value) => emit('update:visible', value)
    });

    const isAdmin = localStorage.getItem('isAdmin') === '1';
    const users = ref([]);
    const selectedUser = ref(null);

    const summaryItems = ref([]);
    const chartSeries = ref([]);
    const chartOptions = ref({
      chart: { background: '#181818', foreColor: '#E0E0E0' },
      xaxis: { categories: ['Jan','Feb','Mar','Apr','May','Jun','Jul','Aug','Sep','Oct','Nov','Dec'] },
      yaxis: {
        title: {
          text: 'kW/h',
          style: { fontSize: '20px', color: '#E0E0E0' }
        }
      },
      title: { text: 'Monthly Consumption', style: { fontSize: '20px', color: '#FFFFFF' } }
    });

    const fetchUsers = async () => {
      try {
        const response = await GetPartners();
        if(response && response.data && response.data.result){
          // Map partners to an array of objects with label and value
          users.value = response.data.result.map(u => ({
            label: u.userEmail,
            value: u.userId
          }));
          selectedUser.value = users.value[0]?.value;
        }
      } catch (error) {
        console.error('Failed to fetch partners:', error);
      }
    };

    const fetchSummary = async () => {
      let uid = props.userId;
      if (isAdmin) {
        uid = selectedUser.value;
      }
      if (!uid) return;
      try {
        const response = await GetElectricitySummary(uid);
        const data = response.data;
        summaryItems.value = [
          { title: 'Last Month Consumption', value: data.lastMonthConsumption  + ' kW/h' },
          { title: 'Current Year Consumption', value: data.currentYearConsumption + ' kW/h' },
          { title: 'Last Year Consumption', value: data.lastYearConsumption  + ' kW/h' },
          { title: 'Last Month Price', value: data.lastMonthPrice + '$' },
          { title: 'Current Year Price', value: data.currentYearPrice + '$' },
          { title: 'Last Year Price', value: data.lastYearPrice + '$' },
        ];
        chartSeries.value = [{ name: 'Consumption', data: data.monthlyConsumption }];
      } catch (error) {
        console.error('Failed to fetch summary data:', error);
      }
    };

    const closeModal = () => { internalVisible.value = false; };

    watch(() => props.visible, (val) => { if(val) fetchSummary(); });
    if (isAdmin) {
      watch(selectedUser, () => {
        fetchSummary();
      });
      onMounted(() => { fetchUsers(); });
    }
    onMounted(() => { if(props.visible) fetchSummary(); });

    return { internalVisible, summaryItems, chartSeries, chartOptions, closeModal, isAdmin, users, selectedUser, fetchSummary };
  }
};
</script>

<style scoped>
.summary-container { display: flex; flex-wrap: wrap; justify-content: space-around; margin-bottom: 20px; }
.summary-box { background: #242424; color: #E0E0E0; padding: 10px; margin: 5px; border-radius: 10px; flex: 1 1 30%; text-align: center; }
.chart-section { margin-top: 20px; }
.modalActions { display: flex; justify-content: flex-end; margin-top: 20px; }

.graph-container {
  border-radius: 10px;
  overflow: hidden;
  border: 1px solid #444;
  background-color: #181818; /* Ensures the bottom color matches the chart background */
}
</style>
