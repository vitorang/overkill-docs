export const SEGMENTS = {
    ACCOUNT: {
        ROOT: 'account',
        LOGIN: 'login',
        SETTINGS: 'settings'
    }
} as const;

const S = SEGMENTS;
export const PATHS = {
    ROOT: '/',
    ACCOUNT: {
        LOGIN: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.LOGIN}`,
        SETTINGS: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.SETTINGS}`
    }

} as const;