import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ImportStatus } from '../../models/import-status';

@Component({
  selector: 'app-select-files',
  templateUrl: './select-files.component.html',
  styleUrls: ['./select-files.component.scss'],
})
export class SelectFilesComponent {
  public canDrop: boolean | null;
  @Input()
  public icon: string;

  @Input()
  public description: string;

  public files: File[];
  @Output()
  public fileChanged = new EventEmitter<File[]>();

  @Input()
  public status: ImportStatus;
  public STATUS = ImportStatus;

  public dragEnter(event: Event): void {
    if (this.status === ImportStatus.Loading) {
      return undefined;
    }
    this.canDrop = true;
    event.preventDefault();
    event.stopPropagation();
  }

  public dragOver(event: Event): void {
    if (this.status === ImportStatus.Loading) {
      return undefined;
    }
    event.preventDefault();
    event.stopPropagation();
  }

  public dragLeave(event: Event): void {
    this.canDrop = null;
    event.preventDefault();
    event.stopPropagation();
  }

  public drop(event: DragEvent): void {
    if (this.status === ImportStatus.Loading) {
      return;
    }
    this.canDrop = null;

    if (!event.dataTransfer) {
      return;
    }

    const fileList = event.dataTransfer.files;
    const files = this.convert(fileList);
    this.files = files;
    this.fileChanged.emit(files);
    event.preventDefault();
    event.stopPropagation();
  }

  public convert(fileList: FileList): File[] {
    const files: File[] = [];
    for (let i = 0; i < fileList.length; i++) {
      const file = fileList.item(i);

      if (!file) {
        continue;
      }

      files.push(file);
    }
    return files;
  }
}
