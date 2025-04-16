import axios from "axios";

const mainCall = "https://localhost:5001/pis/";

export function GetPartners(){
    return axios.get(mainCall + "Users");
}

export function Login(email, password){
    return axios.post(mainCall + "login", {
        email: email,
        password: password
    });
}

export function GetPartnerDataDashboard(id, year){
    return axios.get(mainCall + "Users/" + id + "/"+ year + "/dashboard");  //USE THIS LATER
}

export function GetPartnerDataDataDashboard(id, fromMonth, fromYear, toMonth, toYear, mode){
    return axios.get(mainCall + "Users/" + id + "/"+ fromMonth + "/" + fromYear + "/" + toMonth + "/" + toYear + "/" + mode + "/report");
}

export function GetPartnerDataDataDashboardPrice(id, fromMonth, fromYear, toMonth, toYear, mode){
    return axios.get(mainCall + "Users/" + id + "/"+ fromMonth + "/" + fromYear + "/" + toMonth + "/" + toYear + "/" + mode + "/reportTariffArchive");
}

export function fetchYearsForUser(id){
    return axios.get(mainCall + "Users/" + id + "/years");
}

export function PostVerifiedDevices(model, serialNum){
    return axios.post(mainCall + "Device/add", {
        model: model,
        serialNum: serialNum 
    });
}

export function PutVerifiedDevices(vdId, model, serialNum,){
    return axios.put(mainCall + "Device/put", {
        vdId: vdId,
        model: model,
        serialNum: serialNum 
    });
}

export function DeleteVerifiedDevices(vdId, model, serialNum,){
    return axios.delete(mainCall + "Device/delete", {
        headers: {
            'Content-Type': 'application/json'
        },
        data: JSON.stringify({
            vdId: vdId,
            model: model,
            serialNum: serialNum 
        })
    });
}
export function GetVerifiedDevices(){
    return axios.get(mainCall + "Device");
}

export function PostTariffs(name, price){
    return axios.post(mainCall + "Tariff/add", {
        name: name,
        price: parseFloat(price) 
    });
}

export function PutTariffs(month, year, price, tariffId) {
    return axios.put(mainCall + "Tariff/put/" + month + "/" + year + "/" + price + "/" + tariffId);
}

export function DeleteTariffs(tId, name, price,){
    return axios.delete(mainCall + "Tariff/delete", {
        headers: {
            'Content-Type': 'application/json'
        },
        data: JSON.stringify({
            tId: tId,
            name: name,
            price: parseFloat(price) 
        })
    });
}
export function GetTariffs(){
    return axios.get(mainCall + "Tariff");
}
export function GetInvoices(userId){
    return axios.get(mainCall + "Invoice/" + userId);
}
export function DeleteInvoices(userId, month, year) {
    return axios.delete(mainCall + "Invoice/delete/" + userId + "/" + month + "/" + year);
}
export function PostInvoices(userId, month, year, usedPower, tariffId, deviceId) {
    return axios.post(mainCall + "Invoice/post/"+ userId + "/" + month + "/" + year + "/" + usedPower + "/" + tariffId + "/" + deviceId);
}

export function PutInvoices(billId, userId, email, month, year, usedPower, paidAmount, deviceId ,tariffId) {
    return axios.put(mainCall + "Invoice/put/" + billId+"/"+ userId + "/" + email + "/" + month + "/" + year + "/" + usedPower + "/" + paidAmount + "/" + deviceId + "/" + tariffId);
}
export function DownloadBill(userId, month, year, loguserId) {
    return axios.get(mainCall + "Users/" + userId + "/bills/" + month + "/" + year + "/" +loguserId, {
        responseType: 'blob' // Ensure the response is treated as a Blob
    }).then((response) => {
        // Create a URL for the blob
        const url = window.URL.createObjectURL(new Blob([response.data]));
        // Create a temporary anchor element and trigger the download
        const link = document.createElement('a');
        link.href = url;
        link.setAttribute('download', `Bill-${userId}-${month}-${year}.pdf`); // Assuming the file is a PDF
        document.body.appendChild(link);
        link.click();

        // Clean up by revoking the URL and removing the anchor element
        window.URL.revokeObjectURL(url);
        link.parentNode.removeChild(link);
    });
}

export function GetUsers(){
    return axios.get(mainCall + "Users_db");
}

export function PostUsers(any){
    return axios.post(mainCall + "Users/add", {
            firstName: any.firstname,
            lastName: any.lastname,
            email: any.email,
            password: any.password,
            address: any.address,
            city: any.city,
            zipcode: any.zipcode,
            country: any.country,
            birthdate: any.birthdate,
            tariffId: any.tarriff,
            admincheck: any.admincheck,
    });
}

export function PutUsers(any){
    return axios.put(mainCall + "Users/put/" + any.uId, {
        uId: any.uId,
        firstName: any.firstname,
        lastName: any.lastname,
        email: any.email,
        password: any.password,
        address: any.address,
        city: any.city,
        zipcode: any.zipcode,
        country: any.country,
        birthdate: any.birthdate,
        tariffId: any.tarriff,
        admincheck: any.admincheck,
    });
}

export function GetUser(uId){
    return axios.get(mainCall + "Users/" + uId);
}

export function DeleteUsers(uId){
    return axios.delete(mainCall + "Users/delete/" + uId);
}

export function GetDevices(){
    return axios.get(mainCall + "UserDevices/");
}

export function PostDevices(any){
    return axios.post(mainCall + "UserDevice/post/", {
            email: any.email,
            name: any.name,
            from_date: any.from_date,
            to_date: any.to_date,
    });
}

export function UpdateDevice(deviceId, day_from, month_from, year_from, day_to, month_to, year_to) {
    return axios.put(mainCall + "Device/" + deviceId + "/" + day_from + "/" + month_from + "/" + year_from + "/" + day_to + "/" + month_to + "/" + year_to);
}

export function DeleteDevice(id){
    return axios.delete(mainCall + "UserDevice/delete/" + id);
}

export function AvailableDevices(id,month,year){
    return axios.get(mainCall + "Available/" + id + "/" + month + "/" + year);
}

export function GetInvoice(id, mond, year){
    return axios.get(mainCall + "Invoice/" + id + "/" + mond + "/" + year);
}

export function GetElectricitySummary(userId){
    return axios.get(mainCall + "electricity-summary/" + userId);
}