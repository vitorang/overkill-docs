import { Validators } from '@angular/forms';

export const PASSWORD_VALIDATORS = [
    Validators.required,
    Validators.minLength(3),
    Validators.maxLength(15),
] as const;
