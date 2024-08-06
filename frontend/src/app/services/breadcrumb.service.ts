/**
 * (c) Copyright by Abraxas Informatik AG
 *
 * For license information see LICENSE file.
 */

import { Injectable } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { Observable, ReplaySubject } from 'rxjs';
import { NavigationItem } from '../models/navigation-item';
import { filter } from 'rxjs/operators';
import { TranslationService } from './translation/translation.service';

const URL_SEGEMENT_SEPARATOR = '/';

@Injectable()
export class BreadcrumbService {
  private _items = new ReplaySubject<Array<NavigationItem>>(1);

  constructor(private readonly router: Router, private readonly translationService: TranslationService) {
    router.events.pipe(filter(event => event instanceof NavigationEnd)).subscribe((navigationEnd: NavigationEnd) => {
      this._createItems(navigationEnd.urlAfterRedirects);
    });
  }

  public get items$(): Observable<Array<NavigationItem>> {
    return this._items.asObservable();
  }

  public navigate(navigationItem: NavigationItem): void {
    this.router.navigateByUrl(navigationItem.path);
  }

  private _createItems(url: string): void {
    const items: NavigationItem[] = [];
    const urlSegments: string[] = [];
    url.split(URL_SEGEMENT_SEPARATOR).forEach(urlSegment => {
      urlSegments.push(urlSegment);
      const path = urlSegments.join('/');
      const title = this.translationService.translate(`ROUTES.${path}`);
      items.push(new NavigationItem(title, path));
    });
    this._items.next(items);
  }
}
