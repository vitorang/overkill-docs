import { Injectable } from '@angular/core';
import { botttsNeutral } from '@dicebear/collection';
import { createAvatar } from '@dicebear/core';
import { Options } from '@dicebear/bottts-neutral';

export type AvatarCustomization = 'color' | 'eyes' | 'mouth';
type AvatarSplit = [
    string,
    string,
    NonNullable<Options['eyes']>[number],
    NonNullable<Options['mouth']>[number],
];

const SEPARATOR = ';';
const AVATAR_PREFIX = 'botttsNeutral';
const COLORS = ['DC143C', 'FF69B4', 'FF8C00', 'FFD700', '9370DB', '228B22', '4169E1', '808080'];

const EYES = [
    'bulging',
    'dizzy',
    'eva',
    'frame1',
    'frame2',
    'glow',
    'happy',
    'hearts',
    'robocop',
    'round',
    'roundFrame01',
    'roundFrame02',
    'sensor',
    'shade01',
];

const MOUTHS = [
    'bite',
    'diagram',
    'grill01',
    'grill02',
    'grill03',
    'smile01',
    'smile02',
    'square01',
    'square02',
];

@Injectable({ providedIn: 'root' })
export class AvatarService {
    private generateNumber(text: string) {
        let hash = 0;
        for (let i = 0; i < text.length; i++) {
            const char = text.charCodeAt(i);
            hash = char + (hash << 6) + (hash << 16) - hash;
        }
        return Math.abs(hash);
    }

    private xorshift(seed: number) {
        seed ^= seed << 13;
        seed ^= seed >> 17;
        seed ^= seed << 5;

        return seed >>> 0;
    }

    private pickOne(values: string[], seed: number) {
        const i = this.xorshift(seed) % values.length;
        return values[i];
    }

    private splitAvatarCode(avatarCode: string): AvatarSplit {
        let split = avatarCode.split(SEPARATOR) as AvatarSplit;
        if (split.length !== 4 || split[0] !== AVATAR_PREFIX) {
            console.warn(`Avatar '${avatarCode}' inválido.`);
            split = this.generateAvatarCode('-').split(SEPARATOR) as AvatarSplit;
        }

        return split;
    }

    generateAvatarCode(seed: string): string {
        const rngSeed = this.generateNumber(seed);
        const color = this.pickOne(COLORS, rngSeed);
        const eyes = this.pickOne(EYES, rngSeed + 1);
        const mouth = this.pickOne(MOUTHS, rngSeed + 2);

        return [AVATAR_PREFIX, color, eyes, mouth].join(SEPARATOR);
    }

    generateSvg(avatarCode: string, seed: string, radius: number): string {
        if (!avatarCode) avatarCode = this.generateAvatarCode(seed);

        const [_prefix, color, eye, mouth] = this.splitAvatarCode(avatarCode);

        return createAvatar(botttsNeutral, {
            backgroundColor: [color],
            eyes: [eye],
            mouth: [mouth],
            radius,
        }).toString();
    }

    listCodeVariations(avatarCode: string, variation: AvatarCustomization): string[] {
        const [_prefix, color, eye, mouth] = this.splitAvatarCode(avatarCode);
        const colors = variation === 'color' ? COLORS : [color];
        const eyes = variation === 'eyes' ? EYES : [eye];
        const mouths = variation === 'mouth' ? MOUTHS : [mouth];

        const codes: string[] = [];
        for (const color of colors)
            for (const eye of eyes)
                for (const mouth of mouths)
                    codes.push([AVATAR_PREFIX, color, eye, mouth].join(SEPARATOR));

        return codes;
    }
}
