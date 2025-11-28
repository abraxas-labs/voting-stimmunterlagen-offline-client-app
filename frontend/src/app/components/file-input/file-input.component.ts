/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, Input, EventEmitter, Output, ViewChild, ElementRef } from '@angular/core';
import { ElectronService } from '../../services/electron.service';

@Component({
  selector: 'app-file-input',
  templateUrl: './file-input.component.html',
  styleUrl: './file-input.component.scss',
  standalone: false,
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

  constructor(private readonly electronService: ElectronService) {}

  public async changePath(file?: File): Promise<void> {
    let path = '';

    if (file) {
      path = await this.electronService.getPathForFile(file);
    }

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
