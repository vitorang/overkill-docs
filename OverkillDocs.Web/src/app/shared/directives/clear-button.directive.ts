import { Directive } from '@angular/core';

@Directive({
    selector: 'button[okdClearButton]',
    host: {
        '[style]': 'buttonStyles',
        '[attr.type]': '"type"',
    },
})
export class ClearButtonDirective {
    protected buttonStyles: Partial<CSSStyleDeclaration> = {
        background: 'transparent',
        border: 'none',
        padding: '8px',
        margin: '0',
        color: 'inherit',
        font: 'inherit',
        cursor: 'pointer',
        pointerEvents: 'auto',
        display: 'inline-flex',
        alignItems: 'center',
        justifyContent: 'center',
        outline: 'none',
        textDecoration: 'none',
    };
}
