export interface AuthRequest
{
    username: string
    password: string
    userAgent: string
}

export interface AuthResponse
{
    readonly token: string
}

export enum AuthStorageMode
{
    LocalStorage = 'LOCAL_STORAGE',
    QueryString = 'QUERY_STRING'
}