/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
  selector: 'app-navigation',
  templateUrl: './navigation.component.html',
  styleUrls: ['./navigation.component.scss'],
  standalone: false,
})
export class NavigationComponent {
  @Input() set width(width) {
    this.displayWidth = width;
  }

  @Input() visible = true;
  @Output() visibleChange = new EventEmitter();
  public displayWidth: string;

  public closeNavigation(): void {
    this.visible = false;
  }
}
