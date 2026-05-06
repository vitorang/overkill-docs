import { CdkPortal } from '@angular/cdk/portal';
import { Component, inject, AfterViewInit, OnDestroy, ViewChild } from '@angular/core';
import { MainHeaderService } from '@shared/services/main-header.service';
import { SHARED } from '@shared/index';
import { FillInputDirective } from '@shared/directives/fill-input.directive';
import { ToggleChatButtonComponent } from '@features/document/components/toggle-chat-button/toggle-chat-button.component';

@Component({
    selector: 'okd-document-index-page',
    imports: [SHARED, FillInputDirective, ToggleChatButtonComponent],
    templateUrl: './document-index-page.component.html',
    styleUrl: './document-index-page.component.scss',
})
export class DocumentIndexPageComponent implements AfterViewInit, OnDestroy {
    @ViewChild('leftPortal') leftPortal!: CdkPortal;
    @ViewChild('centerPortal') centerPortal!: CdkPortal;
    @ViewChild('rightPortal') rightPortal!: CdkPortal;

    private mainHeaderService = inject(MainHeaderService);

    ngAfterViewInit(): void {
        this.mainHeaderService.portal.set({
            left: this.leftPortal,
            center: this.centerPortal,
            right: this.rightPortal,
        });
    }

    ngOnDestroy(): void {
        this.mainHeaderService.portal.set(null);
    }
}
