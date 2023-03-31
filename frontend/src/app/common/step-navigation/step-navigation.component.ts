import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-step-navigation',
  templateUrl: './step-navigation.component.html',
})
export class AbraStepNavigationComponent {
  @Input() direction = 'horizontal';
}
