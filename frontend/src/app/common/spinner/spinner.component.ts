/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-spinner',
  templateUrl: './spinner.component.html',
  styleUrls: ['./spinner.component.scss'],
  standalone: false,
})
export class SpinnerComponent {
  @Input()
  public size = '5rem';
  @Input()
  public progress: number | undefined;
  @Input()
  public showLabel = false;
  @Input()
  public bright = false;

  private circumference: number = Math.PI * 20 * 2;

  /**
   * Returns a valid value for progress.
   * If the progress is not defined, 0 is returned;
   * If the progress is greater than 1, 1 is returned;
   * If the progress is below 0, 0 is returned;
   *
   * @readonly
   * @memberof SpinnerComponent
   */
  public get validProgress(): number {
    return Math.min(Math.max(this.progress || 0, 0), 1);
  }

  public get infinite(): boolean {
    return this.progress === undefined;
  }

  public get dashProgress(): string | undefined {
    if (this.infinite) {
      return undefined;
    }
    const angle = this.validProgress * this.circumference;
    return `${angle}, ${this.circumference - angle}`;
  }

  public get label(): string {
    return `${Math.round(this.validProgress * 100)}%`;
  }
}
