# GearBox — Online prodavnica opreme za outdoor aktivnosti

## Ideja

GearBox je e-commerce platforma za prodaju outdoor opreme (kampovanje, planinarenje, trčanje, biciklizam). Korisnik može da pregleda katalog, filtrira proizvode, dodaje u korpu, naruči sa Stripe plaćanjem, i prati status narudžbine. Admin može da upravlja proizvodima, kategorijama i narudžbinama.

Projekat je dovoljno realan da pokaže arhitekturne odluke, a dovoljno mali da bude završiv za jednu osobu.

---

## Tech Stack

| Sloj | Tehnologija |
|------|-------------|
| Backend API | .NET 10 Web API |
| Arhitektura | Clean Architecture (Core → Infrastructure → API) |
| ORM | Entity Framework Core (Code First) |
| Baza | SQL Server (LocalDB za dev, opciono PostgreSQL) |
| Autentifikacija | ASP.NET Identity + JWT |
| Plaćanje | Stripe (Checkout Session + Webhooks) |
| Frontend | Angular 18+ (standalone components) |
| State mgmt | Signals + BehaviorSubject gde treba |
| UI | Angular Material ili Tailwind CSS |
| Caching | In-memory caching za katalog (opciono Redis) |

---

## Domain model

### Entiteti

**Product**
- Id, Name, Description, Price (decimal), PictureUrl
- FK: BrandId, CategoryId
- Quantity (stock)
- IsActive (soft delete)

**Category**
- Id, Name, Description
- ParentCategoryId (nullable — za podkategorije: Kampovanje → Šatori, Vreće za spavanje...)

**Brand**
- Id, Name

**AppUser** (Identity)
- Id, Email, DisplayName
- Roles: Customer, Admin
- Addresses (1:N)

**Address**
- Id, FirstName, LastName, Street, City, PostalCode, Country
- IsDefault (bool)

**Basket** (korpa — ne čuva se u SQL-u, čuva se u memoriji ili Redis-u sa userId/guestId kao ključ)
- Id (string — GUID za guest korisnike, UserId za ulogovane)
- Items: List<BasketItem> (ProductId, ProductName, Price, Quantity, PictureUrl)

**Order**
- Id, BuyerEmail, OrderDate
- ShipToAddress (owned entity)
- OrderItems (1:N)
- Subtotal, DeliveryFee, Total (computed)
- Status: Pending → PaymentReceived → Shipped → Delivered / PaymentFailed
- PaymentIntentId (Stripe)

**OrderItem**
- Id, ProductOrdered (owned: ProductId, ProductName, PictureUrl)
- Price, Quantity

**DeliveryMethod**
- Id, ShortName, Description, DeliveryTime, Price
- Primeri: "Standardna 3-5 dana — 500 RSD", "Ekspres 1 dan — 1500 RSD"

---

## Funkcionalnosti — Backend

### 1. Catalog API

```
GET    /api/products?search=&brandId=&categoryId=&sort=&pageIndex=&pageSize=
GET    /api/products/{id}
GET    /api/categories
GET    /api/brands
```

- **Paginacija** — PagedList<T> sa TotalCount, PageSize, PageIndex
- **Sortiranje** — po imenu (asc/desc), po ceni (asc/desc), po datumu
- **Filtriranje** — po brand-u, kategoriji, pretrazi (Name.Contains)
- **Specification pattern** — umesto hardkodovanih LINQ upita, koristi klase poput `ProductsWithBrandsAndCategoriesSpec`
- **Caching** — keširanje liste kategorija i brendova (ne menjaju se često)

### 2. Basket API

```
GET    /api/basket?id={basketId}
POST   /api/basket          (upsert — kreira ili ažurira)
DELETE /api/basket?id={basketId}
```

- Basket se čuva van SQL-a (in-memory Dictionary<string, Basket> za početak, opciono Redis)
- Kad se korisnik uloguje, spoji se guest basket sa user basket-om

### 3. Identity / Auth

```
POST   /api/account/register
POST   /api/account/login
GET    /api/account            (trenutni korisnik iz tokena)
GET    /api/account/address    (default adresa)
PUT    /api/account/address
```

- ASP.NET Identity sa JWT bearer token-om
- Refresh token (opciono ali dobar za portfolio)
- Role-based: Customer (default), Admin
- Validacija: FluentValidation za registraciju/login DTO

### 4. Orders API

```
POST   /api/orders              (kreira narudžbinu iz basket-a)
GET    /api/orders               (moje narudžbine)
GET    /api/orders/{id}
GET    /api/orders/delivery-methods
```

- Kreiranje: uzme basket items, proveri cene iz baze (ne veruje klijentu!), kreira Order
- Samo ulogovani korisnici
- Admin: GET /api/admin/orders (sve narudžbine), PUT /api/admin/orders/{id}/status

### 5. Payments (Stripe)

```
POST   /api/payments/{basketId}   (kreira ili ažurira PaymentIntent)
POST   /api/payments/webhook      (Stripe webhook)
```

**Flow:**
1. Korisnik klikne "Plati" → frontend pozove POST /api/payments/{basketId}
2. Backend kreira Stripe PaymentIntent, vrati clientSecret
3. Frontend koristi Stripe.js (confirmCardPayment sa clientSecret)
4. Stripe pošalje webhook → backend ažurira Order status

**Webhook handler:**
- `payment_intent.succeeded` → OrderStatus = PaymentReceived
- `payment_intent.payment_failed` → OrderStatus = PaymentFailed
- Verifikacija Stripe signature-a (webhook secret)

### 6. Admin API (zaštićen [Authorize(Roles = "Admin")])

```
POST   /api/admin/products        (kreiranje proizvoda)
PUT    /api/admin/products/{id}   (izmena)
DELETE /api/admin/products/{id}   (soft delete — IsActive = false)
PUT    /api/admin/orders/{id}/status
GET    /api/admin/orders
```

---

## Funkcionalnosti — Frontend (Angular)

### Stranice / Rute

| Ruta | Komponenta | Opis |
|------|-----------|------|
| `/` | HomeComponent | Hero baner, featured kategorije |
| `/shop` | ShopComponent | Katalog sa filterima i paginacijom |
| `/shop/:id` | ProductDetailComponent | Detalj proizvoda, dodavanje u korpu |
| `/basket` | BasketComponent | Pregled korpe, izmena količine |
| `/checkout` | CheckoutComponent | Stepper: adresa → dostava → plaćanje → pregled |
| `/checkout/success` | SuccessComponent | Potvrda narudžbine |
| `/account/login` | LoginComponent | Login forma |
| `/account/register` | RegisterComponent | Registracija |
| `/orders` | OrdersComponent | Moje narudžbine |
| `/orders/:id` | OrderDetailComponent | Detalj jedne narudžbine |
| `/admin` | AdminComponent (lazy) | CRUD za proizvode, pregled svih narudžbina |

### Ključne Angular teme za portfolio

**Signals za lokalno stanje:**
```typescript
// BasketComponent
items = signal<BasketItem[]>([]);
total = computed(() => this.items().reduce((sum, i) => sum + i.price * i.quantity, 0));
```

**BehaviorSubject za deljeno stanje (servis):**
```typescript
// BasketService — deli stanje između Header-a, Basket stranice, Checkout-a
private basketSource = new BehaviorSubject<Basket | null>(null);
basket$ = this.basketSource.asObservable();

basketTotalItems = computed(() => {
  // ili signal koji se ažurira kad se basket$ promeni
});
```

**Interceptor za JWT:**
```typescript
// auth.interceptor.ts — dodaje Authorization header
```

**Route guards:**
```typescript
// auth.guard.ts — štiti /checkout, /orders, /admin
// admin.guard.ts — štiti /admin (proverava role)
```

**Lazy loading:**
```typescript
{ path: 'admin', loadChildren: () => import('./admin/admin.routes') }
```

**Checkout stepper:**
- Korak 1: Adresa (reactive form sa validacijom)
- Korak 2: Izbor dostave (radio buttons za DeliveryMethod)
- Korak 3: Stripe card element (Stripe.js + Elements)
- Korak 4: Pregled pre potvrde

**Paginacija i filteri:**
- Custom `PaginationComponent` (reusable)
- Query parametri u URL-u (`/shop?brandId=2&sort=priceAsc&page=2`)
- `ShopService` koristi HttpParams za prosleđivanje

---

## Clean Architecture — Struktura foldera

```
src/
├── GearBox.Domain/               ← Entiteti, Interfaces, nema zavisnosti
│   ├── Entities/
│   │   ├── Product.cs
│   │   ├── Order.cs
│   │   ├── OrderItem.cs
│   │   └── ...
│   ├── Interfaces/
│   │   ├── IGenericRepository.cs
│   │   └── IBasketRepository.cs
│   └── Specifications/
│       ├── ISpecification.cs
│       ├── BaseSpecification.cs
│       └── ProductsWithBrandsAndCategoriesSpec.cs
│
├── GearBox.Application/          ← Use cases, DTOs, Mappings, Validacija
│   ├── DTOs/
│   ├── Mappings/                 ← AutoMapper profiles
│   ├── Validators/               ← FluentValidation
│   └── Services/
│       ├── IPaymentService.cs
│       └── IOrderService.cs
│
├── GearBox.Infrastructure/       ← EF, Identity, Stripe, spoljni servisi
│   ├── Data/
│   │   ├── GearBoxContext.cs
│   │   ├── Migrations/
│   │   └── SeedData/
│   ├── Repositories/
│   ├── Identity/
│   │   ├── AppUser.cs
│   │   └── AppIdentityDbContext.cs
│   └── Services/
│       ├── PaymentService.cs     ← Stripe logika
│       └── OrderService.cs
│
├── GearBox.API/                  ← Controllers, Middleware, Program.cs
│   ├── Controllers/
│   ├── Middleware/
│   │   └── ExceptionMiddleware.cs
│   ├── Extensions/               ← ServiceCollection extension metode
│   └── Helpers/
│       └── MappingProfiles.cs
│
└── client/                       ← Angular app
    └── src/app/
        ├── core/                 ← Interceptori, guards, nav
        ├── shared/               ← Reusable komponente, modeli
        ├── features/
        │   ├── shop/
        │   ├── basket/
        │   ├── checkout/
        │   ├── orders/
        │   ├── account/
        │   └── admin/
        └── app.routes.ts
```

---

## Seed Data

Kreiraj ~20 proizvoda u 4-5 kategorija sa realnim imenima:

**Kategorije:** Kampovanje, Planinarenje, Trčanje, Biciklizam

**Primer proizvoda:**
- "Naturehike Cloud Up 2 šator" — Kampovanje — 12.500 RSD
- "Osprey Atmos 65 ranac" — Planinarenje — 28.000 RSD
- "Salomon Speedcross 6" — Trčanje — 16.500 RSD
- "Garmin Edge 540 bike computer" — Biciklizam — 35.000 RSD

**Brendovi:** Salomon, Osprey, The North Face, Garmin, Naturehike

Slike: koristi placeholder slike sa Unsplash-a ili generiši jednostavne product placeholder-e.

---

## Error Handling

**Backend:**
- `ExceptionMiddleware` — hvata sve izuzetke, vraća uniformni JSON format
- `ApiResponse` klasa: StatusCode, Message
- `ApiValidationErrorResponse`: nasleđuje ApiResponse, dodaje Errors[]
- Swagger dokumentacija sa primerima grešaka

**Frontend:**
- HTTP Error Interceptor — hvata 400/401/404/500, prikazuje toast ili redirect
- Toast notifikacije za uspeh/greška (npr. `ngx-toastr`)
- Loading interceptor — globalni spinner za HTTP pozive

---

## Faze razvoja (predlog redosleda)

### Faza 1 — Osnova (1-2 nedelje)
- [ ] Kreirati solution sa Clean Architecture strukturom
- [ ] Domain entiteti + EF konfiguracija + migracije
- [ ] Generic Repository + Specification pattern
- [ ] Catalog API (Products, Brands, Categories)
- [ ] Seed data
- [ ] Swagger
- [ ] Angular shell: routing, core module, shop stranica (lista + filteri)
- [ ] Product detail stranica

### Faza 2 — Basket + Identity (1 nedelja)
- [ ] Basket API (in-memory ili Redis)
- [ ] BasketService u Angularu (BehaviorSubject)
- [ ] Header sa basket icon-om i brojem stavki
- [ ] Identity setup (register, login, JWT)
- [ ] Auth interceptor + guards u Angularu
- [ ] Account stranice (login, register)

### Faza 3 — Orders + Stripe (1-2 nedelje)
- [ ] Order entiteti + repozitorijum
- [ ] Kreiranje narudžbine iz basket-a
- [ ] Stripe PaymentIntent integracija
- [ ] Webhook handler
- [ ] Angular checkout stepper (adresa → dostava → Stripe → pregled)
- [ ] Orders stranica (lista mojih narudžbina)

### Faza 4 — Admin + Polish (1 nedelja)
- [ ] Admin panel: CRUD proizvoda
- [ ] Admin: pregled i ažuriranje statusa narudžbina
- [ ] Error handling (middleware + interceptori)
- [ ] Loading indikatori
- [ ] Responsivan dizajn
- [ ] README.md sa screenshot-ovima i uputstvom za pokretanje

---

## README.md koji ostavlja utisak

Na kraju projekta, napravi README koji sadrži:
1. Screenshot-ove ključnih stranica (shop, checkout, admin)
2. Arhitekturni dijagram (Clean Architecture slojevi)
3. Tech stack listu
4. Uputstvo za lokalno pokretanje (git clone → dotnet run → ng serve)
5. Stripe test kartice za demo
6. Napomenu o čemu si razmišljao pri arhitekturnim odlukama

---

## Bonus ideje (ako želiš da proširuješ)

- **Wishlist** — korisnik može da sačuva proizvode za kasnije
- **Product reviews** — ocena 1-5 + komentar, prosečna ocena na proizvodu
- **Email potvrda** — SendGrid za order confirmation email
- **Docker** — Dockerfile + docker-compose (API + SQL Server + Redis + Angular)
- **CI/CD** — GitHub Actions: build → test → objavi na neku besplatnu platformu
- **Kuponi** — Stripe Promotion Codes integracija (imaš iskustvo sa ovim iz skinet-a)
