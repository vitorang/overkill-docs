import { UAParser } from 'ua-parser-js';

export interface ParsedUserAgent {
    browserName: string;
    deviceIcon: string;
}

const DEVICE_ICONS: Record<string, string> = {
    'mobile': 'smartphone',
    'tablet': 'tablet',
    'smarttv': 'tv',
    'console': 'videogame_asset',
    'wearable': 'watch',
    'embedded': 'memory',
    'xr': 'vrpano',
    'desktop': 'desktop_windows',
};


export function parseUserAgent(userAgent: string): ParsedUserAgent {
    const res = new UAParser(userAgent).getResult();
    const type = res.device.type ?? 'desktop';

    return {
        browserName: res.browser.name || userAgent,
        deviceIcon: DEVICE_ICONS[type] || 'help_outline'
    };
}
