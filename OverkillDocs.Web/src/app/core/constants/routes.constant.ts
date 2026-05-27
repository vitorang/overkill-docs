export const SEGMENTS = {
    HOME: 'documents',
    ACCOUNT: {
        ROOT: 'account',
        LOGIN: 'login',
        SETTINGS: 'settings',
    },
    DOCUMENT: {
        ROOT: 'documents',
        VIEWER: ':documentHashId',
    },
} as const;

const S = SEGMENTS;
export const PATHS = {
    ROOT: '',
    ACCOUNT: {
        LOGIN: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.LOGIN}`,
        SETTINGS: `/${S.ACCOUNT.ROOT}/${S.ACCOUNT.SETTINGS}`,
    },
    DOCUMENT: {
        INDEX: `/${S.DOCUMENT.ROOT}`,
        EDITOR: `/${S.DOCUMENT.ROOT}/${S.DOCUMENT.VIEWER}`,
    },
} as const;
