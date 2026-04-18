import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatOptionModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { AvatarComponent } from '@shared/components/avatar/avatar.component';
import { BackgroundImageComponent } from '@shared/components/background-image/background-image.component';
import { BrandComponent } from '@shared/components/brand/brand.component';
import { RequestOverlayComponent } from '@shared/components/request-overlay/request-overlay.component';
import { MainHeaderComponent } from '@shared/components/main-header/main-header.component';
import { ClearButtonDirective } from '@shared/directives/clear-button.directive';
import { MatDialogModule } from '@angular/material/dialog';

export const SHARED_NATIVE = [
    CommonModule,
    FormsModule,
    MatBadgeModule,
    MatButtonModule,
    MatCardModule,
    MatDialogModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatMenuModule,
    MatOptionModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatToolbarModule,
    ReactiveFormsModule,
] as const;


export const SHARED_CUSTOM = [
    AvatarComponent,
    BackgroundImageComponent,
    BrandComponent,
    ClearButtonDirective,
    MainHeaderComponent,
    RequestOverlayComponent,
] as const;