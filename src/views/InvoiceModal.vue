<template>
    <div class="modalDevice">
        <div class="deviceData" style="margin-bottom: 20px">
            <div class="leftInput" >
                <label v-if="addEdit !== 'Add'">User:</label>
                <Dropdown id="user" v-model="selectedEmail" :options="users" :disabled="!isAdmin" placeholder="User" style="width: 250px;" />
            </div>
            <div class="rightInput" v-if="addEdit === 'Edit'">
                <label>Left to pay: {{ tariffData.price - tariffData.paidAmount }}$</label>
                <InputText id="paidAmount" v-model="enteredPaidAmount" :disabled="tariffData.price - tariffData.paidAmount == 0" placeholder="Paid Amount" style="width: 250px; height: 44px; font-size: medium;" />
            </div>
        </div>
        <div class="deviceData" style="margin-bottom: 20px">
            <div class="leftInput">
                <Dropdown v-model="selectedMonth" :options="months" optionLabel="name" :disabled="addEdit === 'Edit'" optionValue="value" placeholder="Select a Month" style="width: 250px;" />
            </div>
            <div class="rightInput">
                <Dropdown v-model="selectedYear" :options="years" optionLabel="name" :disabled="addEdit === 'Edit'" optionValue="code" placeholder="Select a Year" style="width: 250px;" />
            </div>
        </div>
        <!-- Show the devices if any are available -->
        <div class="deviceData" v-for="(device, index) in devices" :key="index" style="margin-bottom: 20px">
            <div class="leftInput">
                <label>Device:</label>
                <InputText v-model="device.input1" :placeholder="device.name" style="width: 250px; height: 44px; font-size: medium;" disabled />
            </div>
            <div class="rightInput">
                <label>min: {{ device.lastMonthPowerUsage }}</label>
                <InputText v-model="device.input2" placeholder="Power Usage(kWh)" style="width: 250px; height: 44px; font-size: medium;" />
            </div>
        </div>
        <div class="modalActions">
            <Button label="Cancel" @click="cancel" class="p-button-text" />
            <Button label="Save" @click="validateAndSave" class="p-button-success" :disabled="isSaveDisabled" />
        </div>
    </div>
</template>
<script>
import { ref, watch, onMounted } from 'vue';
import InputText from 'primevue/inputtext';
import Dropdown from 'primevue/dropdown';
import Button from 'primevue/button';
import { GetPartners, AvailableDevices, GetInvoice, PostInvoices, PutInvoices } from '../services/services.js';
import { toast } from 'vue3-toastify';
import 'vue3-toastify/dist/index.css';

export default {
    components: {
        InputText,
        Button,
        Dropdown,
    },
    props: {
        tariffData: {
            type: Object,
            default: () => ({
                id: null,
                userId: null,
                email: '',
                month: null,
                year: null,
                usedPower: null,
                paidAmount: '',
                price: '',
            }),
        },
        addEdit: {
            type: String,
            required: true,
        },
        yearRange: {
            type: Object,
            default: () => ({ start: 2023, end: 2028 }),
        },
    },
    setup(props, { emit }) {
        const data = ref([]);
        const users = ref([]);
        const selectedEmail = ref(props.tariffData.email);
        const selectedMonth = ref(props.tariffData.month);
        const selectedYear = ref(props.tariffData.year);
        const isSaveDisabled = ref(false);
        const enteredPaidAmount = ref(''); // New variable for entered paid amount
        const months = ref([
            { name: '1', value: 1 },
            { name: '2', value: 2 },
            { name: '3', value: 3 },
            { name: '4', value: 4 },
            { name: '5', value: 5 },
            { name: '6', value: 6 },
            { name: '7', value: 7 },
            { name: '8', value: 8 },
            { name: '9', value: 9 },
            { name: '10', value: 10 },
            { name: '11', value: 11 },
            { name: '12', value: 12 },
        ]);
        const devices = ref([]);
        const years = ref(generateYearsArray(props.yearRange.start, props.yearRange.end));
        const isAdmin = ref(localStorage.getItem('isAdmin') === '1');
        const userEmail = ref(localStorage.getItem('email'));

        function cancel() {
            emit('close');
        }

        async function fetchPartners() {
            try {
                const response = await GetPartners();
                if (response && response.data && response.data.result) {
                    data.value = response.data.result;
                    users.value = data.value.map((user) => ({
                        label: user.userEmail,
                        value: user.userEmail
                    }));
                    console.log('Email options:', users.value); // Log all email options
                    if (!isAdmin.value) {
                        selectedEmail.value = userEmail.value;
                    } else {
                        selectedEmail.value = props.tariffData.email;
                    }
                } else {
                    console.error('No partners data or invalid structure:', response);
                    data.value = [];
                }
            } catch (error) {
                console.error('Failed to fetch partners:', error);
                data.value = [];
            }
        }

        async function fetchDevicesByUser(email, month, year) {
            // Fetch devices associated with the selected email (user)
            const selectedUser = data.value.find((user) => user.userEmail === email);
            if (selectedUser && selectedUser.userId) {
                const response = await AvailableDevices(selectedUser.userId, month, year);
                devices.value = response.data
                    .filter((device) => device.email === email)
                    .map((device) => ({
                        ...device,
                        input1: device.name,
                        input2: device.powerUsage || '', // Assuming `powerUsage` contains power usage data
                        input3: device.billingId
                    }));

                // Fetch invoice data and update devices with power usage
                const invoiceResponse = await GetInvoice(selectedUser.userId, month, year);
                if (invoiceResponse && invoiceResponse.data && invoiceResponse.data.result.devices) {
                    const invoiceDevices = invoiceResponse.data.result.devices;
                    devices.value.forEach((device) => {
                        const invoiceDevice = invoiceDevices.find((d) => d.name === device.name);
                        if (invoiceDevice) {
                            device.input2 = invoiceDevice.powerUsage;
                            device.billingId = invoiceDevice.billingId; // Assign billingId
                        }
                    });
                }
                let lastMonth = month - 1;
                let lastYear = year;
                if (lastMonth === 0) {
                    lastMonth = 12;
                    lastYear = year - 1;
                }
                const invoiceResponseLastMonth = await GetInvoice(selectedUser.userId, lastMonth, lastYear);
                if (invoiceResponseLastMonth && invoiceResponseLastMonth.data && invoiceResponseLastMonth.data.result.devices) {
                    const lastMonthDevices = invoiceResponseLastMonth.data.result.devices;
                    devices.value.forEach((device) => {
                        const lastMonthDevice = lastMonthDevices.find((d) => d.name === device.name);
                        if (lastMonthDevice) {
                            device.lastMonthPowerUsage = lastMonthDevice.powerUsage;
                        }
                    });
                }
                // Check if any device has an empty value under "Power Usage" when addEdit is "Add"
                if (props.addEdit === 'Add') {
                    isSaveDisabled.value = devices.value.some(device => device.input2);
                }
                if(isSaveDisabled.value) {
                    toast.error('You are unable to create invoice for this device because this invoice already exists.', {
                        position: toast.POSITION.TOP_CENTER,
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                    });
                }
            }
        }

        function validateAndSave() {
            let invalidDevices = [];

            devices.value.forEach((device) => {
                const currentUsage = parseFloat(device.input2);
                const lastMonthUsage = parseFloat(device.lastMonthPowerUsage);

                if (currentUsage < lastMonthUsage) {
                    invalidDevices.push(device.name);
                }
            });

            if (invalidDevices.length > 0) {
                const errorMessage = `The following devices have lower power usage than last month: ${invalidDevices.join(', ')}. Please check and update the values.`;
                toast.error(errorMessage, {
                    position: toast.POSITION.TOP_CENTER,
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                });
                return;
            }

            if (props.addEdit === 'Edit') {
                // Check if enteredPaidAmount + tariffData.paidAmount exceeds tariffData.price
                const totalPaidAmount = parseFloat(enteredPaidAmount.value) + parseFloat(props.tariffData.paidAmount);
                if (isNaN(parseFloat(enteredPaidAmount.value))) {
                    const errorMessage = `The paid amount (${enteredPaidAmount.value}) must be a valid number. Please check and update the values.`;
                    toast.error(errorMessage, {
                        position: toast.POSITION.TOP_CENTER,
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                    });
                    return;
                } else if (parseFloat(enteredPaidAmount.value) < 0) {
                    const errorMessage = `The paid amount (${enteredPaidAmount.value}) cannot be negative. Please check and update the values.`;
                    toast.error(errorMessage, {
                        position: toast.POSITION.TOP_CENTER,
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                    });
                    return;
                } else if (totalPaidAmount > parseFloat(props.tariffData.price)) {
                    const errorMessage = `The total paid amount (${totalPaidAmount}) exceeds the price (${props.tariffData.price}). Please check and update the values.`;
                    toast.error(errorMessage, {
                        position: toast.POSITION.TOP_CENTER,
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                    });
                    return;
                }
            }

            // If validation passes, proceed with saving
            save();
        }

        function save() {
            const selectedUser = data.value.find((user) => user.userEmail === selectedEmail.value);
            if (props.addEdit === 'Add') {
                devices.value.forEach((device) => {
                    PostInvoices(selectedUser.userId, selectedMonth.value, selectedYear.value, device.input2, selectedUser.tariffId, device.udId);
                });
                console.log('Add', { ...props.tariffData, month: selectedMonth.value, year: selectedYear.value });
                toast.success('Data added successfully!', {
                    position: toast.POSITION.TOP_CENTER,
                    autoClose: 3000,
                });
                emit('refreshData');
                emit('close');
            } else if (props.addEdit === 'Edit') {
                let isFirstDevice = true; // Flag to track the first device
                const values = parseFloat(enteredPaidAmount.value) + parseFloat(props.tariffData.paidAmount);
                devices.value.forEach((device) => {
                    if (device.billingId) {
                        PutInvoices(
                            device.billingId,
                            selectedUser.userId,
                            "aa",
                            selectedMonth.value,
                            selectedYear.value,
                            device.input2,
                            isFirstDevice ? values : 0, // Use enteredPaidAmount for the first device, 0 for others
                            device.udId,
                            selectedUser.tariffId
                        );
                    } else {
                        PutInvoices(
                            device.billingId,
                            selectedUser.userId,
                            "aa",
                            selectedMonth.value,
                            selectedYear.value,
                            device.input2,
                            0,
                            device.udId,
                            selectedUser.tariffId
                        );
                    }
                    isFirstDevice = false; // Set the flag to false after the first iteration
                });

                // Here you would typically call an API to update the data
                toast.success('Data updated successfully!', {
                    position: toast.POSITION.TOP_CENTER,
                    autoClose: 3000,
                });
                emit('refreshData');
                emit('close');
            }
        }

        function generateYearsArray(startYear, endYear) {
            let years = [];
            for (let year = startYear; year <= endYear; year++) {
                years.push({ name: year.toString(), code: year });
            }
            return years;
        }

        // Watch for changes to the selectedEmail, selectedMonth, and selectedYear and fetch devices accordingly
        watch([selectedEmail, selectedMonth, selectedYear], ([newEmail, newMonth, newYear]) => {
            if (newEmail && newMonth && newYear) {
                fetchDevicesByUser(newEmail, newMonth, newYear); // Fetch devices whenever the user, month, or year is selected
            }
        });

        onMounted(async () => {
            await fetchPartners(); // Fetch users on mount
            if (!isAdmin.value) {
                console.log('User email:', userEmail.value);
                selectedEmail.value = userEmail.value;
                console.log('Selected email:', selectedEmail.value);
            } else {
                selectedEmail.value = props.tariffData.email;
            }
            if (props.addEdit === 'Edit') {
                fetchDevicesByUser(selectedEmail.value, selectedMonth.value, selectedYear.value); // Fetch devices for the selected user in edit mode
            }
        });

        return { cancel, validateAndSave, save, users, selectedEmail, selectedMonth, selectedYear, enteredPaidAmount, months, years, devices, isAdmin, isSaveDisabled };
    },
};
</script>

<style scoped>
.modalDevice {
    display: flex;
    flex-direction: column;
}

.deviceData {
    display: flex;
    justify-content: space-between;
}

.leftInput,
.rightInput {
    flex: 1;
    margin: 0 10px;
}

.leftInput input,
.rightInput input {
    font-size: 1.25rem;
    padding: 10px;
}

.modalActions {
    display: flex;
    justify-content: flex-end;
    margin-top: 20px;
}

.modalActions .p-button {
    margin-left: 10px;
}
</style>
