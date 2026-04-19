import { Component, inject, OnInit, signal } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AvatarCustomization, AvatarService } from '@core/services/avatar.service';
import { ClearButtonDirective } from '@shared/directives/clear-button.directive';
import { SHARED } from '@shared/index';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';

export interface AvatarCustomizationDialogData {
    avatarCode: string;
    customization: AvatarCustomization;
    seed: string;
}

@Component({
    selector: 'okd-avatar-customization-dialog',
    imports: [SHARED, ClearButtonDirective, AvatarComponent],
    templateUrl: './avatar-customization-dialog.component.html',
    styleUrl: './avatar-customization-dialog.component.scss',
})
export class AvatarCustomizationDialogComponent implements OnInit {
    protected data = inject<AvatarCustomizationDialogData>(MAT_DIALOG_DATA);

    private avatarService = inject(AvatarService);
    protected variations = signal<string[]>([]);

    ngOnInit(): void {
        const { avatarCode, customization, seed } = this.data;
        const code = avatarCode || this.avatarService.generateAvatarCode(seed);
        this.variations.set(this.avatarService.listCodeVariations(code, customization));
    }
}
