import http from 'k6/http';
import { sleep, fail } from 'k6';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

export let options = {
    insecureSkipTLSVerify: true,
    stages: [
        { duration: '10s', target: 80 },
        { duration: '30s', target: 100 },
        { duration: '30s', target: 150 },
        { duration: '30s', target: 200 },
        { duration: '30s', target: 300 }, // this api can handle everything.
    ]
}

// get the admin cookie, that can be used in request headers like:
// 'Cookie': `token.AspNetCore.Cookies=${adminCookie}`
function loginAsAdmin() {
    let loginUrl = 'http://localhost:5100/api/User/login';

    let payload = JSON.stringify({
        email: 'admin@admin.com',
        password: 'ADMIN_admin'
    });
    let params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };
    let res = http.post(loginUrl, payload, params);
    if (res.status !== 200) {
        fail("login failed with status " + res.status);
    }
    let adminCookie = res.cookies['.AspNetCore.Cookies'][0].value;
    return adminCookie;
}

export default function () {
    let baseUrl = 'http://localhost:5100/api/Order';
    let adminCookie = loginAsAdmin();

    // Create a new order
    let payload = JSON.stringify({
        name: "k6-" + Date.now()
    });
    let params = {
        headers: {
            'Content-Type': 'application/json',
            'Cookie': `token.AspNetCore.Cookies=${adminCookie}`
        },
    };
    let res = http.post(baseUrl, payload, params);
    if (res.status !== 200) {
        fail(`Order creation failed with status code: ${res.status}`);
    }

    sleep(1);


    // List all orders
    res = http.get(baseUrl);
    if (res.status !== 200) {
        fail(`Order listing failed with status code: ${res.status}`);
    }

    let randomOrderId = res.json()[Math.floor(Math.random() * res.json().length)].id;

    sleep(1);


    // Get an order by id
    res = http.get(`${baseUrl}/${randomOrderId}`);
    if (res.status !== 200) {
        fail(`Order retrieval failed with status code: ${res.status}`);
    }

    sleep(1);


    // Update an order
    payload = JSON.stringify({
        name: "k6-" + Date.now()
    });
    res = http.put(`${baseUrl}/${randomOrderId}`, payload, params);
    if (res.status !== 204) {
        fail(`Order update failed with status code: ${res.status}`);
    }

    sleep(1);


    // Create pdf for an order
    res = http.get(`${baseUrl}/${randomOrderId}/pdf`);
    if (res.status !== 200) {
        fail(`PDF creation failed with status code: ${res.status}`);
    }

}


export function handleSummary(data) {
    return {
        "../order_test_summary.html": htmlReport(data, { title: "Orders API" })
    };
}