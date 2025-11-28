/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, ElementRef, Input, OnDestroy, ViewChild } from '@angular/core';

@Component({
  selector: 'app-pdf-viewer',
  templateUrl: './pdf-viewer.component.html',
  standalone: false,
})
export class PdfViewerComponent implements OnDestroy {
  public url?: string;

  private _data?: Uint8Array;

  @ViewChild('iframe', { static: true }) iframe: ElementRef;

  @Input()
  public set data(d: Uint8Array | undefined) {
    if (this._data === d) {
      return;
    }

    this.disposeUrl();

    if (!d) {
      return;
    }

    const blob = new Blob([d as BlobPart], { type: 'application/pdf' });
    this.url = URL.createObjectURL(blob);
    this._data = d;
  }

  public ngOnDestroy(): void {
    this.disposeUrl();
  }

  private disposeUrl(): void {
    if (this.url) {
      URL.revokeObjectURL(this.url);
      delete this.url;
    }
  }
}
