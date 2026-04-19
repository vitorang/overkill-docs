import { HttpErrorResponse } from '@angular/common/http';
import { Component, effect, inject, input, output, signal } from '@angular/core';
import { FormControl, FormGroup, NonNullableFormBuilder, Validators } from '@angular/forms';
import { ProblemDetails } from '@core/models/problem-details.model';
import { AlertService } from '@shared/services/alert.service';
import { apiHandler } from '@core/utils/api-handler.utils';
import { FormUtils } from '@core/utils/form.utils';
import { Profile } from '@features/account/account.models';
import { AccountSettingsService } from '@features/account/services/account-settings.service';
import { SHARED } from '@shared/index';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { AvatarCustomization } from '@core/services/avatar.service';
import {
    AvatarCustomizationDialogComponent,
    AvatarCustomizationDialogData,
} from '@features/account/components/avatar-customization-dialog/avatar-customization-dialog.component';
import { MatDialog } from '@angular/material/dialog';

type ProfileForm = FormGroup<{
    [K in keyof Profile]: FormControl<Profile[K]>;
}>;

@Component({
    selector: 'okd-profile-form',
    imports: [SHARED, AvatarComponent],
    templateUrl: './profile-form.component.html',
    styleUrl: './profile-form.component.scss',
})
export class ProfileFormComponent {
    value = input.required<Profile>();
    saved = output<Profile>();

    private alertService = inject(AlertService);
    private accountSettingsService = inject(AccountSettingsService);
    private dialog = inject(MatDialog);
    protected profileHandler = apiHandler();

    private formBuilder = inject(NonNullableFormBuilder);
    protected formGroup: ProfileForm = this.formBuilder.group({
        name: ['', [Validators.required, Validators.maxLength(15)]],
        username: [{ value: '', disabled: true }],
        avatar: [{ value: '', disabled: true }],
        hashId: [{ value: '', disabled: true }],
    });

    protected avatar = signal('');

    constructor() {
        effect(() => {
            this.formGroup.patchValue(this.value());
            this.avatar.set(this.formGroup.getRawValue().avatar);
        });
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

    protected showAvatarCustomizationDialog(customization: AvatarCustomization): void {
        const dialogRef = this.dialog.open(AvatarCustomizationDialogComponent, {
            width: '450px',
            autoFocus: 'input',
            data: {
                avatarCode: this.avatar(),
                customization,
                seed: this.value().hashId,
            } as AvatarCustomizationDialogData,
        });

        dialogRef.afterClosed().subscribe((result: string | null) => {
            if (result) {
                this.avatar.set(result);
                this.formGroup.patchValue({ avatar: result });
            }
        });
    }
}
