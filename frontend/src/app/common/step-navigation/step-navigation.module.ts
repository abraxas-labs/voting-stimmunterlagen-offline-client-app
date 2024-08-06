/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AbraStepNavigationItemComponent } from './step-navigation-item/step-navigation-item.component';
import { AbraStepNavigationComponent } from './step-navigation.component';

@NgModule({
  imports: [CommonModule],
  declarations: [AbraStepNavigationComponent, AbraStepNavigationItemComponent],
  exports: [AbraStepNavigationComponent, AbraStepNavigationItemComponent],
})
export class AbraStepNavigationModule {}
