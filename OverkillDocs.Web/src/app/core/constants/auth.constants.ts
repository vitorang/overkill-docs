export const AUTH = {
    TOKEN: 'authToken',
} as const;

export enum AuthStorageMode {
    LocalStorage = 'LOCAL_STORAGE',
    SessionStorage = 'SESSION_STORAGE'
}