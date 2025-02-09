<template>
    <div class="modalTariff">
        <div class="tariffData">
            <div class="leftInput">
                <InputText id="tariffModel" v-model="tariffData.tariffModel" placeholder="Tariff Model" />
            </div>
            <div class="rightInput">
                <InputText id="price" v-model="tariffData.price" placeholder="Price" />
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
import { PostTariffs, PutTariffs} from '../services/services.js'

export default {
    components: {
        InputText,
        Button,
    },
    props: {
        tariffData: {
            type: Object,
            default: () => ({
                id: null,
                tariffModel: null,
                price: null
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
            console.log('Add', props.tariffData);
            await PostTariffs(props.tariffData.tariffModel, props.tariffData.price);
            emit('refreshData');
            emit('close');
        }
            else if(props.addEdit === 'Edit'){
                console.log('Edit', props.tariffData);
                await PutTariffs(props.tariffData.id, props.tariffData.tariffModel, props.tariffData.price)
                emit('refreshData');
                emit('close');
            }
        }

        return { cancel, save };
    }
}
</script>

<style scoped>
.modalTariff {
    display: flex;
    flex-direction: column;
}

.tariffData {
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