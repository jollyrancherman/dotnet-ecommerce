import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Order } from 'src/app/shared/models/order';
import { OrderService } from '../order.service';
import { BreadcrumbService } from 'xng-breadcrumb';

@Component({
  selector: 'app-order-details',
  templateUrl: './order-details.component.html',
  styleUrls: ['./order-details.component.scss'],
})
export class OrderDetailsComponent implements OnInit {
  order?: Order;

  constructor(
    private orderService: OrderService,
    private route: ActivatedRoute,
    private bcService: BreadcrumbService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    id &&
      this.orderService.getOrderDetailed(+id).subscribe({
        next: (order) => {
          this.order = order;
          this.bcService.set(
            '@orderDetails',
            `Order# ${order.id} - ${order.status}`
          );
        },
      });
  }
}
