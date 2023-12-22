import { Component } from '@angular/core';
import { BasketService } from '../basket/basket.service';
import { BasketItem } from '../shared/models/basket';
import { AccountService } from '../account/account.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.scss'],
})
export class NavBarComponent {
  constructor(
    public basketService: BasketService,
    public accountService: AccountService
  ) {}

  getCount(items: BasketItem[]): number {
    return items.reduce((acc, item) => acc + item.quantity, 0);
  }

  logout() {
    this.accountService.logout();
  }
}
