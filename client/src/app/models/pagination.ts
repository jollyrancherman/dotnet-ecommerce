import { Product } from './products';

export interface Pagination<T> {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: T;
}
