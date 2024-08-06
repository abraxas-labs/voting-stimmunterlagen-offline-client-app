/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { HostListener, Directive } from '@angular/core';

@Directive({
  selector: '[appDisableDrop]',
})
export class DisableDropDirective {
  @HostListener('window:dragover', ['$event'])
  public disableDragOver(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
  }

  @HostListener('window:drop', ['$event'])
  public disableDrop(event: Event): void {
    event.preventDefault();
    event.stopPropagation();
  }
}
