<template>
    <div class="modalDevice">
        <div class="deviceData">
            <div class="leftInput">
                <InputText id="model" v-model="deviceData.model" placeholder="Model" />
            </div>
            <div class="rightInput">
                <InputText id="hwid" v-model="deviceData.hwid" placeholder="HWID" />
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
import { PostVerifiedDevices, PutVerifiedDevices} from '../services/services.js'

export default {
    components: {
        InputText,
        Button,
    },
    props: {
        deviceData: {
            type: Object,
            default: () => ({
                id: null,
                model: '',
                hwid: ''
            })
        },
        addEdit: {
            type: String,
            required: true
        }
    },
    setup(props, { emit }) {
        function cancel() {
            emit('close');
        }

        async function save() {
            if(props.addEdit === 'Add')
        {
            console.log('Add', props.deviceData);
            await PostVerifiedDevices(props.deviceData.model, props.deviceData.hwid);
            emit('refreshData');
            emit('close');
        }
            else if(props.addEdit === 'Edit'){
                console.log('Edit', props.deviceData);
                await PutVerifiedDevices(props.deviceData.id, props.deviceData.model, props.deviceData.hwid)
                emit('refreshData');
                emit('close');
            }
        }

        return { cancel, save };
    }
}
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
    margin: 0 10px; /* Add margin for spacing */
}

/* Increase input text size */
.leftInput input, .rightInput input {
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
</style>