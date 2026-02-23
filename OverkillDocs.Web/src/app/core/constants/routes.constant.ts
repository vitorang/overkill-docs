export const SEGMENTS = {
    ACCOUNT: {
        ROOT: 'account',
        LOGIN: 'login',
        REGISTER: 'register',
        SETTINGS: 'settings'
    }
} as const;

const S = SEGMENTS;
export const PATHS = {
    HOME: '/',
    ACCOUNT: {
        LOGIN: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.LOGIN}`,
        REGISTER: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.REGISTER}`,
        SETTINGS: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.SETTINGS}`
    }

} as const;