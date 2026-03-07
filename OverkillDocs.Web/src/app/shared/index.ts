import { MatInputModule } from '@angular/material/input';
import { BackgroundImageComponent } from './components/background-image/background-image.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatOptionModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { BrandComponent } from './components/brand/brand.component';
import { NavigationRailComponent } from './components/navigation-rail/navigation-rail.component';

export const SHARED_NATIVE = [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatFormFieldModule,
    MatIconModule,
    MatInputModule,
    MatOptionModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    ReactiveFormsModule,
] as const;


export const SHARED_CUSTOM = [
    BackgroundImageComponent,
    BrandComponent,
    NavigationRailComponent,
] as const;