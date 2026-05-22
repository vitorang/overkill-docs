import { environment } from '@env/environment';

const url = environment.apiUrl;
export const API = {
    ACCOUNT: {
        CHANGE_PASSWORD: `${url}/Account/ChangePassword`,
        DELETE_ACCOUNT: `${url}/Account/DeleteAccount`,
        LOGIN: `${url}/Account/Login`,
        LOGOUT: `${url}/Account/Logout`,
        LOGOUT_BY_ID: (hashId: string) => `${url}/Account/Logout/${hashId}`,
        PROFILE: `${url}/Account/Profile`,
        REGISTER: `${url}/Account/Register`,
        SESSIONS: `${url}/Account/Sessions`,
    },
    DOCUMENTS: {
        ROOT: `${url}/Documents`,
        BY_ID: (hashId: string) => `${url}/Documents/${hashId}`,
    },
    DOCUMENT_FRAGMENTS: {
        INDEX: `${url}/DocumentFragments`,
        BY_ID: (hashId: string) => `${url}/DocumentFragments/${hashId}`,
    },
    HUB: {
        MAIN: (authToken: string) => `${url}/Hubs/Main?auth_token=${authToken}`,
    },
    USER: {
        CURRENT: `${url}/User/Me`,
        BY_ID: (id: string) => `${url}/User/${id}`,
    },
} as const;
