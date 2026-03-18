import { DOCUMENT } from '@angular/common';
import { inject, Injectable } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { fromEvent, map } from 'rxjs';


@Injectable({ providedIn: 'root' })
export class BrowserService {
    private document = inject(DOCUMENT);

    public documentIsVisible = toSignal(
        fromEvent(this.document, 'visibilitychange').pipe(
            map(() => !this.document.hidden)
        ),
        { initialValue: !this.document.hidden }
    );
}