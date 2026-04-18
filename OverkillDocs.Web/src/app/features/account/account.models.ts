import { ParsedUserAgent } from "@core/utils/browser.utils"

export interface AccountDeletion {
    password: string
}

export interface AuthRequest {
    username: string
    password: string
    userAgent: string
}

export interface AuthResponse {
    readonly token: string
}

export interface PasswordChange {
    currentPassword: string
    newPassword: string
}

export interface Profile {
    avatar: string,
    name: string,
    username: string
}

export interface UserSession extends ParsedUserAgent {
    hashId: string
    userAgent: string
    isCurrent: boolean
}