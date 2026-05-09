import { environment } from '@env/environment';

const url = environment.apiUrl;
export const API = {
    ACCOUNT: {
        CHANGE_PASSWORD: `${url}/account/change-password`,
        DELETE_ACCOUNT: `${url}/account/delete-account`,
        LOGIN: `${url}/account/login`,
        LOGOUT: `${url}/account/logout`,
        LOGOUT_BY_ID: (hashId: string) => `${url}/account/logout/${hashId}`,
        PROFILE: `${url}/account/profile`,
        REGISTER: `${url}/account/register`,
        SESSIONS: `${url}/account/sessions`,
    },
    DOCUMENTS: {
        ROOT: `${url}/documents`,
        BY_ID: (hashId: string) => `${url}/documents/${hashId}`,
        SEARCH: `${url}/documents/search`,
        FRAGMENT: {
            INDEX: (documentHashId: string) => `${url}/documents/${documentHashId}/fragments`,
            BY_ID: (documentHashId: string, fragmentHashId: string) =>
                `${url}/documents/${documentHashId}/fragments/${fragmentHashId}`,
        },
    },
    HUB: {
        MAIN: (authToken: string) => `${url}/hubs/main?auth_token=${authToken}`,
    },
    USER: {
        CURRENT: `${url}/user/me`,
        BY_ID: (id: string) => `${url}/user/${id}`,
    },
} as const;
