import { Directive, HostListener, input } from '@angular/core';

@Directive({
    selector: 'input[okdFillInput]',
    standalone: true,
    host: {
        '[style]': 'styles',
        '[attr.type]': '"text"',
        '[style.maxWidth]': 'maxWidth()',
    },
})
export class FillInputDirective {
    maxWidth = input<string>('none');
    private borderColor = '#ccc';
    private focusedBorderColor = 'cyan';

    protected styles: Partial<CSSStyleDeclaration> = {
        boxSizing: 'border-box',
        width: '100%',
        height: '32px',
        padding: '0 10px',
        fontSize: '14px',
        fontFamily: 'inherit',
        border: '1px solid',
        borderColor: this.borderColor,
        borderRadius: '4px',
        outline: 'none',
        transition: 'all 0.2s ease-in-out',
        color: 'inherit',
        backgroundColor: 'rgba(128, 128, 128, 0.15)',
    };

    @HostListener('focus')
    onFocus(): void {
        this.styles = {
            ...this.styles,
            borderColor: this.focusedBorderColor,
            boxShadow: `0 0 0 0.5px ${this.focusedBorderColor}`,
        };
    }

    @HostListener('blur')
    onBlur(): void {
        this.styles = {
            ...this.styles,
            borderColor: this.borderColor,
            boxShadow: 'none',
        };
    }
}
