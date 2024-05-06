import http from 'k6/http';
import { sleep, fail } from 'k6';

export let options = {
    insecureSkipTLSVerify: true,
    vus: 1,
    stages: [
        { duration: '15s', target: 5 },
        { duration: '15s', target: 10 },
        { duration: '15s', target: 20 },
        { duration: '15s', target: 20 }, // this seems to be the most it can handle
        { duration: '15s', target: 30 },
        { duration: '15s', target: 10 }
    ]
}

// register a new user (user name = k6-timestamp), 
// log in then log out
export default function () {
    let baseUrl = 'http://localhost:5100';

    let registerUrl = baseUrl + '/api/User/register';
    let payload = JSON.stringify({
        "name": "k6-" + Date.now(),
        "email": Date.now() + "@k6.com",
        "password": "k6LongPasswordWith$pecialChars!@#1234",
    });
    let params = {
        headers: {
            'Content-Type': 'application/json',
        },
    };

    let res = http.post(registerUrl, payload, params);
    if (res.status !== 201) {
        fail("register failed with status " + res.status);
        return;
    }

    sleep(1);

    let loginUrl = baseUrl + '/api/User/login';
    res = http.post(loginUrl, payload, params);
    if (res.status !== 200) {
        fail("login failed with status " + res.status);
        return;
    }

    sleep(1);

    let logoutUrl = baseUrl + '/api/User/logout';
    res = http.post(logoutUrl, null, params);
    if (res.status !== 200) {
        fail("logout failed with status " + res.status);
        return;
    }
}