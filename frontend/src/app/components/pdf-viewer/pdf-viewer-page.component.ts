/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, Input, ElementRef, ViewChild, TemplateRef, HostBinding, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-pdf-viewer-page',
  templateUrl: './pdf-viewer-page.component.html',
  styleUrls: ['./pdf-viewer-page.component.scss'],
})
export class PdfViewerPageComponent {
  @Input()
  public set pdf(pdf: any) {
    this._init(pdf);
  }

  @Input()
  public index: number;

  @ViewChild('canvas', { read: ElementRef, static: true })
  public canvas: ElementRef;

  @Input()
  public headerTemplate: TemplateRef<any>;

  @HostBinding('class.checked')
  public checked: boolean | void;

  @Output()
  public checkedChange = new EventEmitter<boolean | void>();

  public check(checked: boolean): void {
    if (this.checked !== checked) {
      this.checked = checked;
      this.checkedChange.emit(checked || undefined);
    }
  }

  public constructor(private readonly elementRef: ElementRef) {}

  private _init(pdf: any): void {
    pdf.getPage(this.index + 1).then((page /*: PDFPageProxy */) => {
      let viewport = page.getViewport(1);
      viewport = page.getViewport(this.elementRef.nativeElement.offsetWidth / viewport.width);
      const canvas = this.canvas.nativeElement;
      const context = canvas.getContext('2d');
      canvas.height = viewport.height;
      canvas.width = viewport.width;
      const renderContext = {
        canvasContext: context,
        viewport: viewport,
      };
      page.render(renderContext);
    });
  }
}
