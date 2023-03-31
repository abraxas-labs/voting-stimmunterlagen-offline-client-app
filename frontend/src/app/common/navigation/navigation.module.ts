import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationComponent } from './navigation.component';
import { NavigationItemComponent } from './navigation-item/navigation-item.component';

@NgModule({
  imports: [CommonModule],
  declarations: [NavigationComponent, NavigationItemComponent],
  exports: [NavigationComponent, NavigationItemComponent],
})
export class AbraNavigationModule {}
