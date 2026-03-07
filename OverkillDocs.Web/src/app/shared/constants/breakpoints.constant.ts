// Ao editar, atualize _breakpoints.scss

export const Breakpoints = {
    small: { min: '0px', max: '599.98px' },
    medium: { min: '600px', max: '1199.98px' },
    large: { min: '1200px', max: null },
    smallMedium: { min: '0px', max: '1199.98px' },
    mediumLarge: { min: '600px', max: null }
} as const;

export const BreakpointQueries = {
    small: `(max-width: ${Breakpoints.small.max})`,
    medium: `(min-width: ${Breakpoints.medium.min}) and (max-width: ${Breakpoints.medium.max})`,
    large: `(min-width: ${Breakpoints.large.min})`,

    smallMedium: `(max-width: ${Breakpoints.smallMedium.max})`,
    mediumLarge: `(min-width: ${Breakpoints.mediumLarge.min})`
} as const;