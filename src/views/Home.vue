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
        <div class="controls-container" style="margin-bottom: 20px;">
            <!-- New grouped radio buttons container -->
            <div class="radio-container" style="display: flex; gap: 40px; align-items: center;">
                <div class="consumption-mode">
                    <div style="display: inline-flex; align-items: center;">
                        <label style="margin-right: 6px;">Per month</label>
                        <RadioButton v-model="consumptionMode" value="monthly" :disabled="!(selectedStartDate && selectedEndDate)" @change="updateChartData" />
                    </div>
                    <div style="display: inline-flex; align-items: center; margin-left: 20px;">
                        <label style="margin-right: 6px;">Per year</label>
                        <RadioButton v-model="consumptionMode" value="yearly" :disabled="!(selectedStartDate && selectedEndDate)" @change="updateChartData" />
                    </div>
                </div>
                <div class="chart-metric">
                    <div style="display: inline-flex; align-items: center;">
                        <label style="margin-right: 6px;">Power</label>
                        <RadioButton v-model="chartMetric" value="used" :disabled="!(selectedStartDate && selectedEndDate)" @change="updateChartData" />
                    </div>
                    <div style="display: inline-flex; align-items: center; margin-left: 20px;">
                        <label style="margin-right: 6px;">Price</label>
                        <RadioButton v-model="chartMetric" value="price" :disabled="!(selectedStartDate && selectedEndDate)" @change="updateChartData" />
                    </div>
                </div>
            </div>
        </div>
        <div class="chart-container">
            <apexchart type="bar" 
            width="1100" height="500" :options="chartOptions" :series="chartSeries" />
        </div>
        <div style="width: 1100px; margin: 0 auto;">
            <Paginator :totalRecords="totalPages" :rows="1" :pageLinkSize="5" @page="onPageChange">
                <template #page="slotProps">
                    <a :class="slotProps.className" href="javascript:void(0)">
                        {{ chartPages[slotProps.page][0] }}
                    </a>
                </template>
            </Paginator>
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
import { GetPartners, fetchYearsForUser, GetPartnerDataDashboard, GetPartnerDataDataDashboard, GetPartnerDataDataDashboardPrice } from '../services/services';
import { ref, watch, onMounted } from 'vue';

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
        const selectedStartDate = ref(new Date(2024, 0, 1));
        const selectedEndDate = ref(new Date(2024, 11, 31));
        const consumptionMode = ref('monthly');
        const chartMetric = ref('used'); // new reactive variable for metric selection

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
            let user = data.value.find(u => u.userEmail === selectedEmail);
            if (!user) {
                selectedEmail = localStorage.getItem("email");
                user = data.value.find(u => u.userEmail === selectedEmail);
            }
            const userId = user.userId;
            ID.value = userId;
            // Remove fetching years; rely on calendar instead.
            years.value = [];
            selectedYear.value = null;
            // Optionally, call updateChartData only if calendar dates are set.
            if(selectedStartDate.value && selectedEndDate.value){
                updateChartData();
            }
        };

        // Fetch backend data and setup pagination
        const updateChartData = async () => {
            console.log(selectedYear);
            if (!ID.value || !selectedStartDate.value) {
                console.error('ID or selectedYear is not set');
                return;
            }
            // If date range provided, use GetPartnerDataDataDashboard, otherwise use GetPartnerDataDashboard:
            let response;
            if(selectedStartDate.value && selectedEndDate.value) {
                const fromMonth = new Date(selectedStartDate.value).getMonth() + 1;
                const fromYear = new Date(selectedStartDate.value).getFullYear();
                const toMonth = new Date(selectedEndDate.value).getMonth() + 1;
                const toYear = new Date(selectedEndDate.value).getFullYear();
                try {
                    if(chartMetric.value === 'price'){
                        response = await GetPartnerDataDataDashboardPrice(ID.value, fromMonth, fromYear, toMonth, toYear, consumptionMode.value);
                    } else {
                        response = await GetPartnerDataDataDashboard(ID.value, fromMonth, fromYear, toMonth, toYear, consumptionMode.value);
                    }
                } catch (error) {
                    console.error('Failed to fetch dashboard report data:', error);
                    return;
                }
            } else {
                try {
                    response = await GetPartnerDataDashboard(ID.value, selectedYear.value);
                } catch (error) {
                    console.error('Failed to fetch chart data:', error);
                    return;
                }
            }
            let resultData;
            if (response && response.data) {
                resultData = response.data.result ? response.data.result : (Array.isArray(response.data) ? response.data : null);
            }
            if (resultData) {
                // For monthly mode, split results into pages of 12 datapoints
                if(Array.isArray(resultData) && consumptionMode.value === 'monthly'){
                    const pages = [];
                    const pageCount = Math.ceil(resultData.length / 12);
                    for (let i = 0; i < pageCount; i++){
                        // Instead of "Page X", always use the selectedYear as label.
                        pages.push([`${selectedYear.value}`, resultData.slice(i * 12, i * 12 + 12)]);
                    }
                    chartPages.value = pages;
                } else if(Array.isArray(resultData)) {
                    // For other modes (e.g., yearly) keep as a single page.
                    chartPages.value = [["All", resultData]];
                } else {
                    chartPages.value = Object.entries(resultData);
                }
                totalPages.value = chartPages.value.length;
                currentPage.value = 0;
                updateChartPage();
            } else {
                console.error('Invalid response structure:', response);
            }
        };

        // Update chart view based on currentPage, consumptionMode, and chartMetric
        const updateChartPage = () => {
            if (chartPages.value.length > 0) {
                let [label, dataArrayRaw] = chartPages.value[currentPage.value];
                const dataArray = Array.isArray(dataArrayRaw) ? dataArrayRaw : [];
                console.log('Updating chart page:', label, dataArray);
                if (label == "null") {
                    label = selectedStartDate.value.getFullYear();
                }
                // For monthly mode, convert label to number and add currentPage.value properly.
                const numericLabel = parseInt(label);
                const displayYear = numericLabel + currentPage.value;
                let newYAxisTitle, newChartTitle;
                if (consumptionMode.value === 'yearly') {
                    const roundedData = dataArray.map(val => parseFloat(val.toFixed(2)));
                    chartSeries.value = [{
                        name: chartMetric.value === 'price'
                              ? `Price in: ${label}`
                              : `Electricity Consumption in: ${label}`,
                        data: roundedData
                    }];
                    newYAxisTitle = chartMetric.value === 'price' ? 'Price($)' : 'kW / h';
                    newChartTitle = chartMetric.value === 'price'
                          ? `Price in: ${label}`
                          : `Electricity Consumption in: ${label}`;
                } else {
                    chartSeries.value = [{
                        name: chartMetric.value === 'price'
                              ? `Price in: ${displayYear}`
                              : `Electricity Consumption in: ${displayYear}`,
                        data: dataArray
                    }];
                    // Use month names instead of numerical data labels
                    const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
                    chartOptions.value.xaxis.categories = months.slice(0, dataArray.length);
                    newYAxisTitle = chartMetric.value === 'price' ? 'Price($)' : 'kW / h';
                    newChartTitle = chartMetric.value === 'price'
                          ? `Price in: ${displayYear}`
                          : `Electricity Consumption in: ${displayYear}`;
                }
                // Reassign chartOptions.value so the nested updates are reactive
                chartOptions.value = {
                    ...chartOptions.value,
                    yaxis: {
                        ...chartOptions.value.yaxis,
                        title: {
                            ...chartOptions.value.yaxis.title,
                            text: newYAxisTitle,
                            style: {
                                fontSize: '20px',
                                color: '#E0E0E0'
                            }
                        }
                    },
                    title: {
                        ...chartOptions.value.title,
                        text: newChartTitle
                    }
                };
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

        // Watch changes to the new metric selection
        watch(chartMetric, () => {
            updateChartPage();
        });

        // Add watch on calendars to update chart data when both dates are filled
        watch([selectedStartDate, selectedEndDate], ([start, end]) => {
            if(start && end) {
                const startYear = new Date(start).getFullYear();
                const endYear = new Date(end).getFullYear();
                const newYears = [];
                for(let y = startYear; y <= endYear; y++){
                    newYears.push(y);
                }
                years.value = newYears;
                selectedYear.value = newYears[0];
                updateChartData();
            }
        });

        // New watcher to update xaxis.categories when yearly mode is active
        watch([consumptionMode, years], () => {
            if(consumptionMode.value === 'yearly'){
                chartOptions.value = {
                    ...chartOptions.value,
                    xaxis: {
                        ...chartOptions.value.xaxis,
                        categories: [...years.value]
                    }
                };
            }
        });

        onMounted(async () => {
            await fetchPartners();
            // Use default user selection from the fetched list
            if (selectedUser.value) {
                onUserSelected(selectedUser.value);
            }
            if (selectedStartDate.value && selectedEndDate.value) {
                updateChartData();
            }
        });

        const chartSeries = ref([]);

        const chartOptions = ref({
            chart: {
                height: 500, // changed from 900 to 500
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
            onPageChange,
            chartMetric
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