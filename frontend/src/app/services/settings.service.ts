/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Injectable } from '@angular/core';

const numberOfPreviewKey = 'numberOfPreview';
const jobSizeKey = 'jobSize';
const javaRuntimePathKey = 'javaRuntimePath';
const postSignatureValidatorPathKey = 'postSignatureValidatorPath';

@Injectable()
export class SettingsService {
  private jobSizeValue: number;
  private numberOfPreviewValue: number;
  private javaRuntimePathValue: string;
  private postSignatureValidatorPathValue: string;

  public get numberOfPreview(): number {
    if (this.numberOfPreviewValue) {
      return this.numberOfPreviewValue;
    }
    const tempValue = window.localStorage.getItem(numberOfPreviewKey);
    if (tempValue) {
      this.numberOfPreviewValue = +tempValue;
      return this.numberOfPreviewValue;
    }

    this.numberOfPreviewValue = 10;
    return this.numberOfPreviewValue;
  }

  public set numberOfPreview(value: number) {
    this.numberOfPreviewValue = value;
    window.localStorage.setItem(numberOfPreviewKey, value.toString());
  }

  public get jobSize(): number {
    if (this.jobSizeValue) {
      return this.jobSizeValue;
    }
    const tempValue = window.localStorage.getItem(jobSizeKey);
    if (tempValue) {
      this.jobSizeValue = +tempValue;
      return this.jobSizeValue;
    }

    this.jobSizeValue = 50;
    return this.jobSizeValue;
  }

  public set jobSize(value: number) {
    this.jobSizeValue = value;
    window.localStorage.setItem(jobSizeKey, value.toString());
  }

  public get javaRuntimePath(): string {
    if (this.javaRuntimePathValue) {
      return this.javaRuntimePathValue;
    }

    const tempValue = window.localStorage.getItem(javaRuntimePathKey);
    if (tempValue) {
      this.javaRuntimePathValue = tempValue;
      return this.javaRuntimePathValue;
    }

    return '';
  }

  public set javaRuntimePath(value: string) {
    this.javaRuntimePathValue = value;
    window.localStorage.setItem(javaRuntimePathKey, value);
  }

  public get postSignatureValidatorPath(): string {
    if (this.postSignatureValidatorPathValue) {
      return this.postSignatureValidatorPathValue;
    }

    const tempValue = window.localStorage.getItem(postSignatureValidatorPathKey);
    if (tempValue) {
      this.postSignatureValidatorPathValue = tempValue;
      return this.postSignatureValidatorPathValue;
    }

    return '';
  }

  public set postSignatureValidatorPath(value: string) {
    this.postSignatureValidatorPathValue = value;
    window.localStorage.setItem(postSignatureValidatorPathKey, value);
  }
}
