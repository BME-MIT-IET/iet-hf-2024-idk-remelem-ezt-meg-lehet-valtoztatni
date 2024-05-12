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
    let baseUrl = 'http://localhost:5100/api/Product';
    let adminCookie = loginAsAdmin();

    // Create a new product
    let myProductName = "k6-" + Date.now();
    let payload = JSON.stringify([{
        name: myProductName,
        price: 100,
        categoryId: 1
    }]);
    let params = {
        headers: {
            'Content-Type': 'application/json',
            'Cookie': `token.AspNetCore.Cookies=${adminCookie}`
        },
    };
    let res = http.post(baseUrl, payload, params);
    if (res.status !== 201) {
        fail(`Product creation failed with status code: ${res.status}`);
    }
    let product = res.json();
    let productId = product[0].id;

    sleep(1);


    // Retrieve a product
    res = http.get(`${baseUrl}/${productId}`);
    if (res.status !== 200) {
        fail(`Product retrieval failed with status code: ${res.status}`);
    }

    sleep(1);


    // Update a product
    payload = JSON.stringify({
        name: "Updated-" + Date.now(),
        price: 150,
        categoryId: 1
    });
    res = http.put(`${baseUrl}/${productId}`, payload, params);
    if (res.status !== 204) {
        fail(`Product update failed with status code: ${res.status}`);
    }

    sleep(1);



    // Delete a product
    res = http.del(`${baseUrl}/${productId}`, null, params);
    if (res.status !== 202) {
        fail(`Product deletion failed with status code: ${res.status}`);
    }
}

export function handleSummary(data) {
    return {
        "../product_test_summary.html": htmlReport(data, { title: "Product API Stress Test" })
    };
}