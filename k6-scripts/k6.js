import {sleep, check} from 'k6';
import http from 'k6/http';
import { randomIntBetween } from 'https://jslib.k6.io/k6-utils/1.2.0/index.js';
export const options = {
    stages: [
        {duration: '10s', target: 5},
        {duration: '5m', target: 5000},
        {duration: '10m', target: 10000},
        {duration: '1m', target: 0}
    ],
    thresholds: {
        http_req_failed: ['rate<0.01'], // http errors should be less than 1%
        http_req_duration: ['p(99)<300'], // 99% of requests must complete below 1.5s
    },
};

export function setup() {
    const url = `${__ENV.BASE_URL}/customers/all`;
    const response = http.get(url);

    if (response.status === 200) {
        const customerIds = JSON.parse(response.body); // Assuming the response body is a JSON array of customer IDs
        return customerIds
    }
    
    return [];
}

export default function (customerIds) {
    const randomIndex = randomIntBetween(0, customerIds.length - 1);
    
    const customerId = customerIds[randomIndex];
    const query = `${__ENV.BASE_URL}/customers/${customerId}`;
    const response = http.get(query);

    sleep(1)
    check(response, {
        'status 200 local': (res) => res.status === 200,
    });
}