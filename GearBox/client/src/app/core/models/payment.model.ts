export interface PaymentIntentResponse {
  clientSecret: string;
}

export interface ShippingAddress {
  fullName:   string;
  address:    string;
  city:       string;
  postalCode: string;
  country:    string;
}
