import { HttpErrorResponse } from '@angular/common/http';
import { Component, effect, inject, input, output } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { ProblemDetails } from '@core/models/problem-details.model';
import { AlertService } from '@shared/services/alert.service';
import { apiHandler } from '@core/utils/api-handler.utils';
import { FormUtils } from '@core/utils/form.utils';
import { Profile } from '@features/account/account.models';
import { AccountSettingsService } from '@features/account/services/account-settings.service';
import { SHARED } from '@shared/index';

type ProfileForm = FormGroup<{
    [K in keyof Profile]: FormControl<Profile[K]>;
}>;

@Component({
    selector: 'okd-profile-form',
    imports: [SHARED],
    templateUrl: './profile-form.component.html',
    styleUrl: './profile-form.component.scss',
})
export class ProfileFormComponent {
    value = input.required<Profile>();
    saved = output<Profile>();

    private alertService = inject(AlertService);
    private accountSettingsService = inject(AccountSettingsService);
    protected profileHandler = apiHandler();

    private formBuilder = inject(NonNullableFormBuilder);
    protected formGroup: ProfileForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(15)]],
        username: [{ value: '', disabled: true }],
        avatar: [{ value: '', disabled: true }],
    });

    constructor() {
        effect(() => this.formGroup.patchValue(this.value()));
    }

    protected onSubmit(): void {
        if (!this.formGroup.valid || this.profileHandler.loading()) return;

        const profile: Profile = this.formGroup.getRawValue();

        this.profileHandler.execute(
            this.accountSettingsService.updateProfile(profile),
            (result) => this.saved.emit(result),
            (err: HttpErrorResponse) => {
                const problem = err.error as ProblemDetails | undefined;
                if (problem?.errors) FormUtils.injectError(this.formGroup, problem.errors);
                else this.alertService.error(problem?.detail);
            },
        );
    }

    protected get nameError(): string {
        return FormUtils.getFieldError(this.formGroup.controls.name);
    }
}
