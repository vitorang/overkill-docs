import { MatInputModule } from '@angular/material/input';
import { BackgroundImageComponent } from './components/background-image/background-image.component';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatOptionModule } from '@angular/material/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatCardModule } from '@angular/material/card';
import { BrandComponent } from './components/brand/brand.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatMenuModule } from '@angular/material/menu';
import { MainHeaderComponent } from './components/main-header/main-header.component';
import { HubMonitorComponent } from './components/hub-monitor/hub-monitor.component';

export const SHARED_NATIVE = [
    CommonModule,
    FormsModule,
    MatButtonModule,
    MatCardModule,
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
    BackgroundImageComponent,
    BrandComponent,
    HubMonitorComponent,
    MainHeaderComponent,
] as const;