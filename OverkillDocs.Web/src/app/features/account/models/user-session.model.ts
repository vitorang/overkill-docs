import { ParsedUserAgent } from "../../../core/utils/browser.utils"

export interface UserSession extends ParsedUserAgent {
    hashId: string
    userAgent: string
    isCurrent: boolean
}