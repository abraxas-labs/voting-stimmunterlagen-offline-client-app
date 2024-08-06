/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, ElementRef, EventEmitter, Input, Output, TemplateRef, ViewChild } from '@angular/core';

@Component({
  selector: 'app-pdf-viewer',
  templateUrl: './pdf-viewer.component.html',
})
export class PdfViewerComponent {
  @Input()
  public pageHeaderTemplate: TemplateRef<any>;

  @Input()
  public set pdf(pdf: ArrayBuffer) {
    this.loadPDF(pdf);
  }

  @Output()
  public checkedChange = new EventEmitter<{ index: number; checked: boolean | void }>();

  @Output()
  public pdfLoaded = new EventEmitter<any>();

  @ViewChild('iframe', { static: true }) iframe: ElementRef;

  private loadPDF(innerSrc): void {
    if (!innerSrc) {
      return;
    }
    let fileUrl;
    if (innerSrc instanceof Blob) {
      fileUrl = encodeURIComponent(URL.createObjectURL(innerSrc));
    } else if (innerSrc instanceof Uint8Array) {
      const blob = new Blob([innerSrc], { type: 'application/pdf' });
      fileUrl = encodeURIComponent(URL.createObjectURL(blob));
    } else {
      throw new Error('pdf inner src is not a blob or uint8 array');
    }
    let viewerUrl = './assets/pdfjs-dist/web/viewer.html';
    viewerUrl += `?file=${fileUrl}`;
    this.iframe.nativeElement.src = viewerUrl;
  }
}
