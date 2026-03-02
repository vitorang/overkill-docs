import { AbstractControl, FormGroup } from "@angular/forms";

export class FormUtils {
    static injectError(form: FormGroup, errors?: Record<string, string[]>): void {
        if (!errors) return;

        Object.keys(errors).forEach((fieldName) => {
            const controlName = fieldName.charAt(0).toLowerCase() + fieldName.slice(1);
            const control = form.get(controlName) || form.get(fieldName);

            if (control) {
                control.setErrors({
                    serverError: errors[fieldName][0]
                });
            }
        });

        form.markAllAsTouched();
    }

    static getFieldError(control: AbstractControl): string {
        if (!control.errors || !control.touched)
            return '';

        const firstKey = Object.keys(control.errors)[0];
        const errorValue = control.errors[firstKey];

        if (typeof errorValue === 'string')
            return errorValue;

        const messages: Record<string, string> = {
            required: 'Deve ser preenchido.',
            minlength: `Mínimo de ${errorValue?.requiredLength} caracteres.`,
        };

        return messages[firstKey] || 'Campo inválido.';
    }
}