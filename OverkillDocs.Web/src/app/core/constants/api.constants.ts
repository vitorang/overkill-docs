import { environment } from "@env/environment";

const url = environment.apiUrl;
export const API = {
    ACCOUNT: {
        CHANGE_PASSWORD: `${url}/account/change-password`,
        DELETE_ACCOUNT: `${url}/account/delete-account`,
        LOGIN: `${url}/account/login`,
        LOGOUT: `${url}/account/logout`,
        LOGOUT_BY_ID: (id: string) => `${url}/account/logout/${id}`,
        PROFILE: `${url}/account/profile`,
        REGISTER: `${url}/account/register`,
        SESSIONS: `${url}/account/sessions`,
    },
    USER: {
        CURRENT: `${url}/user/me`,
        BY_ID: (id: string) => `${url}/user/${id}`,
    },
    HUB: {
        MAIN: (authToken: string) => `${url}/hubs/main?auth_token=${authToken}`
    },
} as const;