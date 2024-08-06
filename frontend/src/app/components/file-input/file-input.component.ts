/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, Input, EventEmitter, Output, ViewChild, ElementRef } from '@angular/core';

@Component({
  selector: 'app-file-input',
  templateUrl: './file-input.component.html',
  styleUrl: './file-input.component.scss',
})
export class FileInputComponent {
  @Input()
  public path: string = '';

  @Input()
  public accept: string;

  @Input()
  public label: string;

  @Output()
  public pathChange: EventEmitter<string> = new EventEmitter<string>();

  @ViewChild('fileInput')
  public fileInputRef: ElementRef;

  public changePath(file?: File): void {
    const path = file?.path ?? '';

    if (path === this.path) {
      return;
    }

    this.path = path;

    if (!path) {
      this.fileInputRef.nativeElement.value = path;
    }

    this.pathChange.emit(this.path);
  }
}
