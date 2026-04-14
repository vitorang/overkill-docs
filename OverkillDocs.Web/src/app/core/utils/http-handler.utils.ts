import { HttpErrorResponse } from "@angular/common/http";
import { computed, DestroyRef, inject, signal } from "@angular/core";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";
import { finalize, Observable } from "rxjs";

type RequestState = 'IDLE' | 'LOADING' | 'SUCCESS' | 'ERROR';

export class HttpHandler {
    private state = signal<RequestState>('IDLE');

    readonly idle = computed(() => this.state() === 'IDLE');
    readonly loading = computed(() => this.state() === 'LOADING');
    readonly success = computed(() => this.state() === 'SUCCESS');
    readonly error = computed(() => this.state() === 'ERROR');

    constructor(private destroyRef: DestroyRef) { }

    execute<T>(
        observable: Observable<T>,
        onSuccess?: (result: T) => void,
        onError?: (error: HttpErrorResponse) => void
    ): void {
        if (this.state() === 'LOADING')
            throw 'Outra requisição já está em execução.';

        const next = (result: T) => {
            this.state.set('SUCCESS');
            if (onSuccess)
                onSuccess(result);
        };

        const error = (error: HttpErrorResponse) => {
            this.state.set('ERROR');
            if (onError)
                onError(error);
        }

        this.state.set('LOADING');
        observable.pipe(
            takeUntilDestroyed(this.destroyRef),
            finalize(() => {
                if (this.state() === 'LOADING')
                    this.state.set('IDLE');
            })
        ).subscribe({ next, error });
    }
}

export function httpHandler(): HttpHandler {
    const destroyRef = inject(DestroyRef);
    return new HttpHandler(destroyRef);
}
