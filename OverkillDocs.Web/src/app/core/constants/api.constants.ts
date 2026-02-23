import { environment } from "../../../environments/environment";

const url = environment.apiUrl;
export const API = {
    ACCOUNT: {
        LOGIN: `${url}/account/login`,
        LOGOUT: `${url}/account/logout`,
        REGISTER: `${url}/account/register`
    }
} as const;