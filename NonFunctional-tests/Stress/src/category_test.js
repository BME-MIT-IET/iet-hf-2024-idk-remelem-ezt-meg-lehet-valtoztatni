import http from 'k6/http';
import { sleep, fail } from 'k6';
import { htmlReport } from "https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js";

export let options = {
    insecureSkipTLSVerify: true,
    stages: [
        { duration: '30s', target: 10 },
        { duration: '30s', target: 20 }, // this seems to be the most it can handle
        { duration: '30s', target: 25 },
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
    let baseUrl = 'http://localhost:5100/api/Category';
    let adminCookie = loginAsAdmin();

    // Create a new category
    let myCategoryName = "k6-" + Date.now();
    let payload = JSON.stringify({
        name: myCategoryName
    });
    let params = {
        headers: {
            'Content-Type': 'application/json',
            'Cookie': `token.AspNetCore.Cookies=${adminCookie}`
        },
    };
    let res = http.post(baseUrl, payload, params);
    if (res.status !== 204) {
        fail(`Category creation failed with status code: ${res.status}`);
    }

    sleep(1);


    // List all categories
    res = http.get(baseUrl);
    if (res.status !== 200) {
        fail(`Category listing failed with status code: ${res.status}`);
    }
    let myCategoryId = null;
    let categories = res.json();
    for (let i = 0; i < categories.length; i++) {
        if (categories[i].name === myCategoryName) {
            myCategoryId = categories[i].id;
            break;
        }
    }

    sleep(1);


    // Get a category by id
    res = http.get(`${baseUrl}/${myCategoryId}`);
    if (res.status !== 200) {
        fail(`Category retrieval failed with status code: ${res.status}`);
    }

    sleep(1);


    // Update a category
    payload = JSON.stringify({
        name: "k6-" + Date.now()
    });
    res = http.put(`${baseUrl}/${myCategoryId}`, payload, params);
    if (res.status !== 200) {
        fail(`Category update failed with status code: ${res.status}`);
    }

    sleep(1);


    // Delete a category
    res = http.del(`${baseUrl}/${myCategoryId}`, null, params);
    if (res.status !== 202) {
        fail(`Category deletion failed with status code: ${res.status}`);
    }

}


export function handleSummary(data) {
    return {
        "../category_test_summary.html": htmlReport(data, { title: "Categories API" })
    };
}