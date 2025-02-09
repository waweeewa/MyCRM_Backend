<template>
    <div class="modalDevice">
        <div class="deviceData" style="margin-bottom: 20px; margin-left: 80px;">
            <div class="leftInput">
                <Dropdown v-model="currentUserData.email" :options="emails" optionLabel="label" optionValue="value" style="width: 244px; height: 50px;" placeholder="Select E-mail" class="custom-dropdown" @change="fetchDevices"/>
            </div>
            <div class="rightInput">
                <InputText v-model="currentUserData.name" placeholder="Device name" />
            </div>
        </div>
        <div class="deviceData" style="margin-bottom: 20px; margin-left: 80px">
            <div class="leftInput">
                <Calendar id="from_date" dateFormat="dd-mm-yy" style="width: 244px;" v-model="currentUserData.from_date" placeholder="From date" showIcon class="custom-calendar" :firstDayOfWeek="1" />
            </div>
            <div class="rightInput">
                <Calendar id="to_date" dateFormat="dd-mm-yy" style="width: 244px;" v-model="currentUserData.to_date" placeholder="To date" showIcon class="custom-calendar" :firstDayOfWeek="1" />
            </div>
        </div>
        <div class="deviceData" v-for="(device, index) in devices" :key="index" style="margin-bottom: 20px; margin-left: 80px;">
            <div class="leftInput">
                <InputText v-model="device.input1" :placeholder="device.name" style="width: 250px; height: 44px; font-size: medium;" disabled/>
            </div>
            <div class="rightInput">
                <InputText v-model="device.input2" placeholder="Power Usage(kWh)" style="width: 250px; height: 44px; font-size: medium;"/>
            </div>
        </div>
    </div>
    
    <div class="modalActions">
        <Button label="Cancel" @click="cancel" class="p-button-text" />
        <Button label="Save" @click="save" class="p-button-success" />
    </div>
</template>

<script>
import { ref, onMounted } from 'vue';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Calendar from 'primevue/calendar';
import Dropdown from 'primevue/dropdown';
import { GetUsers, PostDevices, AvailableDevices } from '../services/services.js';

export default {
    components: {
        InputText,
        Button,
        Calendar,
        Dropdown
    },
    props: {
        currentUserData: {
            type: Object,
            default: () => ({
                udId: null,
                email: '',
                name: '',
                from_date: null,
                to_date: null
            })
        },
        addEdit: {
            type: String,
            required: true
        }
    },
    emits: ['close', 'refreshData'],
    setup(props, { emit }) {
        const emails = ref([]);
        const currentUserData = ref(props.currentUserData);
        const devices = ref([]);
        const isAdmin = ref(localStorage.getItem('isAdmin') === '1');
        const userEmail = ref(localStorage.getItem('email'));

        const fetchUsers = async () => {
            const users = await GetUsers();
            emails.value = users.data.map(user => ({
                label: user.email,
                value: user.email
            }));
        };

        const fetchDevices = async () => {
            if (currentUserData.value.email) {
                const response = await AvailableDevices(currentUserData.value.email);
                devices.value = response.data.map(device => ({
                    ...device,
                    input1: device.name,
                    input2: device.powerUsage || '' // Assuming `powerUsage` is the field containing power usage data
                }));
            }
        };

        onMounted(() => {
            fetchUsers();
            if (!isAdmin.value) {
                currentUserData.value.email = userEmail.value;
                fetchDevices();
            }
        });

        return {
            emails,
            currentUserData,
            fetchUsers,
            fetchDevices,
            devices,
            isAdmin,
            userEmail
        };
    },
    methods: {
        cancel() {
            this.$emit('close');
        },
        formatDateToYYMMDD(date) {
            if (date instanceof Date) {
                const day = String(date.getDate()).padStart(2, '0');
                const month = String(date.getMonth() + 1).padStart(2, '0');
                const year = String(date.getFullYear()).slice(-2);
                return `20${year}-${month}-${day}`;
            } else if (typeof date === 'string') {
                const [day, month, year] = date.split('-');
                return `20${year}-${month}-${day}`;
            }
            return date;
        },
        async save() {
            if (this.currentUserData.from_date) {
                this.currentUserData.from_date = this.formatDateToYYMMDD(this.currentUserData.from_date);
            }
            if (this.currentUserData.to_date) {
                this.currentUserData.to_date = this.formatDateToYYMMDD(this.currentUserData.to_date);
            }

            if (this.addEdit === 'Add') {
                console.log('Add', this.currentUserData);
                await PostDevices(this.currentUserData);
                this.$emit('refreshData');
                this.$emit('close');
            } else if (this.addEdit === 'Edit') {
                console.log('Edit', this.currentUserData);
                this.$emit('refreshData');
                this.$emit('close');
            }
        }
    }
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

.leftInput, .rightInput {
    flex: 1;
    margin: 0 10px;
}

.leftInput input, .rightInput input {
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

.custom-calendar .p-inputtext {
    font-size: 1.25rem;
    padding: 10px;
    height: 50px;
    width: 100%;
}

.custom-calendar .p-calendar {
    width: 100%;
}

.custom-calendar .p-calendar .p-datepicker {
    font-size: 1.25rem;
    padding: 10px;
    width: 350px;
}

.deviceData .leftInput .p-calendar {
    width: 64%;
    height: 121%;
    font-size: 1.25rem;
}

.deviceData .rightInput .p-calendar {
    width: 64%;
    height: 121%;
    font-size: 1.25rem;
}

.custom-dropdown .p-dropdown {
    width: 100%;
}

.custom-dropdown .p-dropdown-label {
    font-size: 1.25rem;
    padding: 10px;
    height: 50px;
}
</style>