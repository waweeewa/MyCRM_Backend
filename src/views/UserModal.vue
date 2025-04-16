<template>
    <div class="modalUser">
        <div class="userData" style="margin-bottom: 20px">
            <div class="leftInput">
                <InputText id="firstname" v-model="userData.firstname" placeholder="First Name" />
            </div>
            <div class="middleInput">
                <InputText id="lastname" v-model="userData.lastname" placeholder="Last name" />
            </div>
            <div class="rightInput">
                <InputText id="email" v-model="userData.email" placeholder="E-mail" />
            </div>
        </div>
        <div class="userData" style="margin-bottom: 20px">
            <div class="leftInput">
                <InputText id="password" v-model="userData.password" placeholder="Password" />
            </div>
            <div class="middleInput">
                <InputText id="address" v-model="userData.address" placeholder="Address" />
            </div>
            <div class="rightInput">
                <InputText id="city" v-model="userData.city" placeholder="City" />
            </div>
        </div>
        <div class="userData" style="margin-bottom: 20px">
            <div class="leftInput">
                <InputText id="zipcode" v-model="userData.zipcode" placeholder="Zipcode" />
            </div>
            <div class="middleInput">
                <InputText id="country" v-model="userData.country" placeholder="Country" />
            </div>
            <div class="rightInput">
                <Calendar id="birthdate" dateFormat="dd-mm-yy" v-model="userData.birthdate" placeholder="Birth date" showIcon class="custom-calendar" />
            </div>
        </div>
        <div class="userData" style="margin-bottom: 20px">
            <div class="leftInput">
                <Dropdown id="tariff" v-model="userData.tarriff" :options="tariffs" optionLabel="label" optionValue="value" placeholder="Select a tariff"/>
            </div>
            <div class="rightInput">
                <label for="admincheck" style="font-size:20px; margin-right: 10px; margin-left: 5px;">Admin</label>
                <Checkbox v-model="admincheck" :binary="true" :disabled="isEmailMatched"></Checkbox>            
            </div>
        </div>
        <div class="modalActions">
            <Button label="Cancel" @click="cancel" class="p-button-text" />
            <Button label="Save" @click="save" class="p-button-success" />
        </div>
    </div>
</template>

<script>
import { defineProps, getCurrentInstance } from 'vue';
import InputText from 'primevue/inputtext';
import Button from 'primevue/button';
import Calendar from 'primevue/calendar';
import Checkbox from 'primevue/checkbox';
import Dropdown from 'primevue/dropdown';
import { PutUsers, PostUsers, GetTariffs } from '../services/services.js';

export default {
    components: {
        InputText,
        Button,
        Checkbox,
        Calendar,
        Dropdown
    },
    props: {
        currentUserData: {
            type: Object,
            required: false,
            default: () => ({
                firstname: '',
                lastname: '',
                email: '',
                password: '',
                address: '',
                city: '',
                zipcode: null,
                country: '',
                birthdate: '',
                tariff: null,
                admincheck: null,
            })
        },
        addEdit: {
            type: String,
            required: true
        }
    },
    computed: {
        isEmailMatched() {
            const storedEmail = localStorage.getItem('email');
            return this.currentUserData.email === storedEmail;
        }
    },
    setup(props, { emit }) {
        function cancel() {
            emit('close');
        }

        return { cancel};
    },
    data() {
        return {
            userData: {
                firstname: '',
                lastname: '',
                email: '',
                password: '',
                address: '',
                city: '',
                zipcode: '',
                country: '',
                birthdate: '',
                tariff: null,
                admincheck: 0,
            },
            admincheck: false,
            tariffs: [] // Define tariffs in data
        };
    },
    methods: {
        async fetchUser() {
            this.userData = this.currentUserData;
            this.admincheck = this.userData.admincheck === 1;
        },
        async fetchTariffs() {
    try {
        const response = await GetTariffs();
        if (response && response.data) {
            // Map tariffs to label and value
            this.tariffs = response.data.result.map(tariff => ({
                label: tariff.name,
                value: tariff.tId,
            }));
            
            // Find and set the correct tariff for the user if it exists
            const selectedTariff = this.tariffs.find(tariff => tariff.value === this.userData.tariff);
            if (selectedTariff) {
                this.userData.tariff = selectedTariff;  // Assign the full object
            } else {
                this.userData.tariff = null;  // Handle cases where no match is found
            }
        }
    } catch (error) {
        console.error('Error fetching tariffs:', error);
    }
},
formatDate(date) {
        const d = new Date(date);
        const year = d.getFullYear().toString();
        const month = ('0' + (d.getMonth() + 1)).slice(-2);
        const day = ('0' + d.getDate()).slice(-2);
        return `${year}-${month}-${day}`;
    },
    async save() {
        this.userData.zipcode = parseInt(this.userData.zipcode);
        if (this.userData.birthdate) {
            // Convert Boolean admincheck to a number (0 or 1)
            this.userData.admincheck = this.admincheck ? 1 : 0;
            this.userData.birthdate = this.formatDate(this.userData.birthdate);
        }
        if (this.addEdit === 'Add') {
            console.log('Add', this.userData);
            await PostUsers(this.userData);
            this.cancel();
        } else if (this.addEdit === 'Edit') {
            console.log('Edit', this.userData);
            await PutUsers(this.userData);
            this.cancel();
        }
    }
    },
    mounted() {
        this.fetchUser();
        this.fetchTariffs();
    },
    unmounted() {
        this.userData = {
            firstname: '',
            lastname: '',
            email: '',
            password: '',
            address: '',
            city: '',
            zipcode: '',
            country: '',
            birthdate: '',
            tariff: null,
            admincheck: 0,
        };
    }
};
</script>

<style scoped>
.modalUser {
    display: flex;
    flex-direction: column;
}

.userData {
    display: flex;
    justify-content: space-between;
}

.leftInput, .middleInput, .rightInput {
    flex: 1;
    margin: 0 10px; /* Add margin for spacing */
}

/* Increase input text size */
.leftInput input,.middleInput input,.rightInput input {
    font-size: 1.25rem; /* Larger font size */
    padding: 10px; /* Larger padding for bigger input fields */
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
    font-size: 1.25rem; /* Increase font size */
    padding: 10px;      /* Increase padding */
    height: 50px;       /* Increase height */
    width: 100%;        /* Ensure the field takes full width */
}

.custom-calendar .p-calendar {
    width: 100%;        /* Ensure the calendar input takes full width */
}

.custom-calendar .p-calendar .p-datepicker {
    font-size: 1.25rem; /* Increase font size inside the calendar */
    padding: 10px;      /* Adjust padding */
    width: 350px;       /* Adjust width of the calendar dropdown */
}

/* Additional fix to ensure full width across the modal */
.userData .rightInput .p-calendar {
    width: 99%;
    height: 100%;
    font-size: 1.25rem;
}
.leftInput .p-dropdown {
    
    align-items: center; /* Center the text vertically */
    height: 43px;        /* Same height as other input fields */
    padding: 0 10px;     /* Adjust padding to align text properly */
    font-size: 1.25rem;  /* Same font size as the input fields */

}

</style>