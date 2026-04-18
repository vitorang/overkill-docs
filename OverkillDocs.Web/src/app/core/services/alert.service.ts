import { Injectable, inject } from '@angular/core';
import { MatSnackBar } from '@angular/material/snack-bar';

@Injectable({ providedIn: 'root' })
export class AlertService {
    private snackBar = inject(MatSnackBar);

    private show(message: string, type: 'error' | 'info'): void {
        this.snackBar.open(message, 'OK', {
            duration: 5000,
            panelClass: [`${type}-toast`],
            horizontalPosition: 'center',
            verticalPosition: 'bottom',
        });
    }

    info(message: string): void {
        this.show(message, 'info');
    }

    error(message: string | null | undefined): void {
        this.show(message || 'Ocorreu um erro', 'error');
    }
}
