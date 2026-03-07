export const SEGMENTS = {
    ACCOUNT: {
        ROOT: 'account',
        LOGIN: 'login',
        SETTINGS: 'settings'
    },
    DOCUMENT: {
        ROOT: 'documents',
        EDITOR: ':id'
    }
} as const;

const S = SEGMENTS;
export const PATHS = {
    ROOT: '/',
    ACCOUNT: {
        LOGIN: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.LOGIN}`,
        SETTINGS: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.SETTINGS}`
    },
    DOCUMENT: {
        INDEX: `${S.DOCUMENT.ROOT}`,
        EDITOR: `${S.DOCUMENT.ROOT}/${S.DOCUMENT.EDITOR}`
    }
} as const;