/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-vct-popup',
  templateUrl: './vct-popup.component.html',
  styleUrls: ['./vct-popup.component.scss'],
  standalone: false,
})
export class VctPopupComponent {
  @Input() visible = false;
  @Input() title = '';
  @Output() visibleChange = new EventEmitter();

  public closePopup(): void {
    this.visible = false;
    this.visibleChange.emit(false);
  }
}
