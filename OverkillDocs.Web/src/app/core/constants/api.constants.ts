import { environment } from "../../../environments/environment";

const url = environment.apiUrl;
export const API = {
    ACCOUNT: {
        LOGIN: `${url}/account/login`,
        LOGOUT: `${url}/account/logout`,
        REGISTER: `${url}/account/register`,
    },
    USER: {
        CURRENT: `${url}/user/me`,
        BY_ID: (id: string) => `${url}/user/${id}`,
    },
    HUB: {
        MAIN: (authToken: string) => `${url}/hubs/main?auth_token=${authToken}`
    },
} as const;