<template>
    <div class="home-container">
        <Header />
        <NavBar />
        <h1 style="margin-left: 460px ; margin-top: 130px;">Welcome Home!</h1>
        <div class="controls-container" style="margin-bottom: 20px;">
            <Dropdown v-if="false" style="margin-right: 20px;" :options="years" v-model="selectedYear" placeholder="Select a Year" @change="updateChartData"  />
            <Dropdown v-if="isAdmin" :options="users" v-model="selectedUser" placeholder="Select a User" @change="onUserSelected(selectedUser)" />
        </div>
        <div>
        </div>
        <div style="margin-bottom: 50px;" >
            <h3 style="display: inline; margin-right: 20px;" >Date from:</h3>
            <Calendar dateFormat="dd-mm-yy" style=" margin-bottom: 20px;" v-model="selectedStartDate" placeholder="Select a date"></Calendar>
            <br>
            <h3 style="display: inline; margin-right: 44px;" >Date to:</h3>
            <Calendar dateFormat="dd-mm-yy" v-model="selectedEndDate" placeholder="Select a date"></Calendar>
        </div>
        <div>
            <div>
                <label style="margin-right: 6px;">Per month</label>
                <RadioButton v-model="consumptionMode" value="month" />
            </div>
            <div>
                <label style="margin-right: 20px;">Per year</label>
                <RadioButton v-model="consumptionMode" value="year" />
            </div>
        </div>
        <div class="chart-container">
            <apexchart type="bar" 
            width="1100" height="500" :options="chartOptions" :series="chartSeries" />
        </div>
        <div style="width: 1100px; margin: 0 auto;">
            <Paginator :totalRecords="totalPages" :rows="1" :pageLinkSize="5" @page="onPageChange" />
        </div>
    </div>
</template>

<script>
import NavBar from '../components/NavBar.vue';
import Header from '../components/Header.vue';
import RadioButton from 'primevue/radiobutton';
import VueApexCharts from 'vue3-apexcharts';
import Dropdown from 'primevue/dropdown';
import Calendar from 'primevue/calendar';
import Paginator from 'primevue/paginator';
import { GetPartners, fetchYearsForUser, GetPartnerDataDashboard } from '../services/services';
import { ref, watch } from 'vue';

export default {
    components: {
        NavBar,
        Header,
        Calendar,
        RadioButton,
        Apexchart: VueApexCharts,
        Dropdown,
        Paginator
    },
    setup() {
        const data = ref([]);
        const users = ref([]);
        const selectedUser = ref(null);
        const years = ref([]);
        const selectedYear = ref(null);
        const ID = ref(null);
        const chartData = ref([]);
        
        const dashboard = ref(null);
        const selectedStartDate = ref(null);
        const selectedEndDate = ref(null);
        const consumptionMode = ref('month');

        // New reactive variables for pagination
        const chartPages = ref([]); // Array of [year, data]
        const currentPage = ref(0);
        const totalPages = ref(1);

        const fetchPartners = async () => {
            try {
                const response = await GetPartners();
                if (response && response.data && response.data.result) {
                    data.value = response.data.result;
                    users.value = data.value.map(user => user.userEmail);
                    selectedUser.value = users.value[0]; // Default to the first user's email
                    onUserSelected(selectedUser.value);
                } else {
                    console.error('No partners data or invalid structure:', response);
                    data.value = [];
                }
            } catch (error) {
                console.error('Failed to fetch partners:', error);
                data.value = [];
            }
        };

        const onUserSelected = async (selectedEmail) => {
            years.value = undefined;
            selectedYear.value = undefined;
            let user = data.value.find(u => u.userEmail === selectedEmail);
            if (!user) {
                selectedEmail = localStorage.getItem("email");
                user = data.value.find(u => u.userEmail === selectedEmail);
            }
            const userId = user.userId;
            ID.value = userId;
            try {
                const response = await fetchYearsForUser(userId);
                if (response && response.data && response.data.result) {
                    years.value = response.data.result;
                    selectedYear.value = years.value[0];
                    updateChartData();
                } else {
                    console.error('Invalid response structure:', response);
                    years.value = [];
                }
            } catch (error) {
                console.error('Failed to fetch years for user:', error);
                years.value = [];
            }
        };

        // Fetch backend data and setup pagination
        const updateChartData = async () => {
            if (!ID.value || !selectedYear.value) {
                console.error('ID or selectedYear is not set');
                return;
            }
            try {
                const response = await GetPartnerDataDashboard(ID.value, selectedYear.value);
                if (response && response.data && response.data.result) {
                    dashboard.value = response.data.result;
                    // Parse backend data into pagination pages.
                    // Assuming dashboard.value is an object with year keys.
                    chartPages.value = Object.entries(dashboard.value);
                    totalPages.value = chartPages.value.length;
                    currentPage.value = 0;
                    updateChartPage();
                    console.log(chartSeries.value);
                } else {
                    console.error('Invalid response structure:', response);
                    chartData.value = [];
                }
            } catch (error) {
                console.error('Failed to fetch chart data:', error);
                chartData.value = [];
            }
        };

        // Update chart view based on currentPage and consumptionMode
        const updateChartPage = () => {
            if(chartPages.value.length > 0) {
                const [year, dataArrayRaw] = chartPages.value[currentPage.value];
                const dataArray = Array.isArray(dataArrayRaw) ? dataArrayRaw : [];
                if(consumptionMode.value === 'year') {
                    const yearlyTotal = dataArray.reduce((sum, val) => sum + val, 0);
                    chartSeries.value = [{
                        name: `Electricity Consumption`,
                        data: [yearlyTotal]
                    }];
                    chartOptions.value.xaxis.categories = [`${year} Total`];
                } else {
                    chartSeries.value = [{
                        name: `Electricity Consumption`,
                        data: dataArray
                    }];
                    chartOptions.value.xaxis.categories = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                }
            }
        };

        // Add new onPageChange handler
        const onPageChange = (event) => {
            currentPage.value = event.page;
            updateChartPage();
        };

        watch(consumptionMode, () => {
            updateChartPage();
        });

        fetchPartners();

        const chartSeries = ref([{
            name: `Electricity Consumption`,
            data: chartData.value
        }]);

        const chartOptions = ref({
            chart: {
                height: 900,
                type: 'bar',
                background: '#181818',
                foreColor: '#E0E0E0',
            },
            plotOptions: {
                bar: {
                    horizontal: false,
                },
            },
            xaxis: {
                categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
                labels: {
                    style: {
                        fontSize: '14px',
                        colors: ['#E0E0E0']
                    }
                }
            },
            yaxis: {
                title: {
                    text: 'kW / h',
                    style: {
                        fontSize: '20px',
                        color: '#E0E0E0'
                    }
                },
                labels: {
                    style: {
                        fontSize: '14px',
                        colors: ['#E0E0E0']
                    }
                }
            },
            fill: {
                colors: ['#4e74bc']
            },
            dataLabels: {
                enabled: false,
                style: {
                    colors: ['#E0E0E0']
                }
            },
            title: {
                text: `Electricity Consumption`,
                align: 'left',
                style: {
                    fontSize: '20px',
                    color: '#FFFFFF'
                }
            },
            legend: {
                position: 'right',
                offsetY: 40,
                labels: {
                    colors: ['#E0E0E0'],
                    fontSize: '14px',
                }
            },
            tooltip: {
                theme: 'dark'
            },
        });
        return { 
            chartSeries,
            years,
            users,
            selectedYear,
            selectedUser,
            updateChartData,
            onUserSelected,
            chartOptions,
            selectedStartDate,
            selectedEndDate,
            consumptionMode,
            currentPage,
            totalPages,
            onPageChange
        };
    },
    created() {
        this.isAdmin = localStorage.getItem('isAdmin') === '1';
    },
};
</script>

<style scoped>
.home-container {
    margin-left: auto;
    margin-right: auto;
    padding-left: 50px; /* Adjust this value as needed to compensate for the NavBar width */
    max-width: calc(100% - 250px); /* Ensures the container does not extend full width considering the NavBar */
    box-sizing: border-box; /* Ensures padding is included in the element's total width and height */
}

/* Additional styles for responsiveness, if needed */
@media (max-width: 768px) {
    .home-container {
        padding-left: 0; /* Resets padding for smaller screens if NavBar is not fixed */
        max-width: 100%; /* Allows the container to use full width on smaller screens */
    }
}
</style>