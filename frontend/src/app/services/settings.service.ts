import { Injectable } from '@angular/core';

@Injectable()
export class SettingsService {
  private jobSizeValue: number;
  private numberOfPreviewValue: number;

  public get numberOfPreview(): number {
    if (this.numberOfPreviewValue) {
      return this.numberOfPreviewValue;
    }
    const tempValue = window.localStorage.getItem('numberOfPreview');
    if (tempValue) {
      this.numberOfPreviewValue = +tempValue;
      return this.numberOfPreviewValue;
    }

    this.numberOfPreviewValue = 10;
    return this.numberOfPreviewValue;
  }

  public set numberOfPreview(value: number) {
    this.numberOfPreviewValue = value;
    window.localStorage.setItem('numberOfPreview', value.toString());
  }

  public get jobSize(): number {
    if (this.jobSizeValue) {
      return this.jobSizeValue;
    }
    const tempValue = window.localStorage.getItem('jobSize');
    if (tempValue) {
      this.jobSizeValue = +tempValue;
      return this.jobSizeValue;
    }

    this.jobSizeValue = 50;
    return this.jobSizeValue;
  }

  public set jobSize(value: number) {
    this.jobSizeValue = value;
    window.localStorage.setItem('jobSize', value.toString());
  }
}
