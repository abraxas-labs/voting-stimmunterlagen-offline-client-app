/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, HostBinding, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-step-navigation-item',
  templateUrl: './step-navigation-item.component.html',
  styleUrls: ['./step-navigation-item.component.scss'],
  standalone: false,
})
export class AbraStepNavigationItemComponent implements OnInit {
  @Input() public active = false;
  @HostBinding('attr.disabled')
  @Input()
  public disabled = false;
  @HostBinding('attr.role') htmlRole = 'button';
  @HostBinding('attr.type') htmlType = 'button';
  @HostBinding('attr.tabindex') tabIndex = 0;

  public ngOnInit(): void {
    if (this.disabled) {
      this.tabIndex = -1;
    }
  }
}
