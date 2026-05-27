export interface CartItem {
  productId: number;
  name: string;
  price: number;
  quantity: number;
  pictureUrl: string;
  brand: string;
}

export interface ShoppingCart {
  userId: string;
  items: CartItem[];
  total: number;
  itemCount: number;
}
