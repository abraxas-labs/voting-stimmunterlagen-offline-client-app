/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Component, Input } from '@angular/core';
import { trigger, style, state, animate, transition } from '@angular/animations';

@Component({
  selector: 'app-fab',
  templateUrl: './fab.component.html',
  styleUrls: ['./fab.component.scss'],
  standalone: false,
  animations: [
    trigger('fabAnimation', [
      state(
        '*',
        style({
          transform: 'translateY(-50%) scale3d(1,1,1)',
        }),
      ),
      state(
        'void',
        style({
          transform: 'translateY(-50%) scale3d(0,0,1)',
        }),
      ),
      transition('* <=> void', [animate('.2s ease-in')]),
    ]),
  ],
  /* eslint-disable */
  host: {
    '[@fabAnimation]': '',
  },
  /* eslint-enable */
})
export class FabComponent {
  @Input()
  public icon: string;
}
