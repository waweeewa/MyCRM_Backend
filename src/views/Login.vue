<template>
    <div class="login-wrapper">
        <div class="login-container" >
            <img src="../img/logo.png" alt="">
            <h1 style="margin-bottom: 2%;">Login</h1>
            <form @submit.prevent="login">
                <div class="form-group">
                    <label for="email">E-mail</label>
                    <div class="input-group">
                        <i class="fas fa-envelope input-icon"></i>
                        <InputText id="email" v-model="email" required class="input-field" />
                    </div>
                </div>
                <div class="form-group">
                    <label for="password">Password</label>
                    <div class="input-group">
                        <i class="fas fa-lock input-icon"></i>
                        <InputText id="password" v-model="password" type="password" required class="input-field" />
                    </div>
                </div>
                <Button label="Login" type="submit" class="login-button" />
            </form>
        </div>
    </div>
</template>


<script>
import { GetPartners, Login } from "../services/services";
import { toast } from 'vue3-toastify';
import 'vue3-toastify/dist/index.css';
import Button from 'primevue/button';
import InputText from 'primevue/inputtext';
import Password from 'primevue/password';

export default {
    components: {
        Button,
        InputText,
        Password
    },
    data() {
        return {
            email: '',
            password: ''
        };
    },
    methods: {
    login() {
        Login(this.email, this.password)
            .then(response => {
                if (response.data.item1 !== null) {
                    // Set a flag in the local storage
                    localStorage.setItem('isLoggedIn', 'true');
                    localStorage.setItem('userID', response.data.item1.userId);
                    localStorage.setItem('isAdmin', response.data.item1.userApproved);
                    localStorage.setItem('tariffId', response.data.item1.tariffId);
                    localStorage.setItem('email', this.email);
                    toast(response.data.item2[0].message, {
                        "theme": "colored",
                        "type": "success",
                        "dangerouslyHTMLString": true,
                        "duration": 1500
                    });

                    // Delay the navigation
                    setTimeout(() => {
                        this.$router.push({ name: 'home' });
                    }, 1500);
                } else {
                    toast(response.data.item2[0].message, {
                        "theme": "colored",
                        "type": "error",
                        "dangerouslyHTMLString": true,
                        "duration": 5000
                    });
                }
            });
    }
}
};
</script>

<style scoped>
body {
    grid-template-columns: none;
}
.login-wrapper {
    display: flex;
    padding-left: 350px;
    justify-content: center;
    align-items: center;
    height: 100vh; /* Full viewport height */
    
    grid-template-columns: none;
}

.login-container {
    background-color: #1e1e1e;
    padding: 40px;
    border-radius: 10px;
    width: 400%; /* Adjust as necessary */
    box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
    text-align: center;
    
    grid-template-columns: none;
}

.input-group {
    position: relative;
    margin-bottom: 15px; /* Add spacing between input fields */
}

.input-icon {
    position: absolute;
    top: 50%;
    left: 10px;
    transform: translateY(-50%);
    color: #888; /* Icon color */
}


.input-field {
    width: 100%;
    padding-left: 30px;  /* Adjust this value as per your needs */
    padding: 10px;
    border-radius: 4px;
    border: 1px solid #ccc;
    background-color: #2b2b2b;
    color: #fff;
}

.input-field:focus {
    outline: none;
    border-color: #1976d2;
}

.login-button {
    width: 100%;
    padding: 10px;
    background-color: #1976d2;
    border: none;
    border-radius: 4px;
    color: white;
    cursor: pointer;
    margin-top: 10px;
}

.login-button:hover {
    background-color: #1565c0;
}

label {
    display: block;
    margin-bottom: 5px;
    color: #fff;
}

h1 {
    color: #fff;
}


</style>