# Corelio - Detailed Functional Specifications
## Module-by-Module Requirements

**Document Version:** 1.0
**Date:** 2025-12-21

---

## Module 1: Multi-Tenant Authentication & Authorization

### 1.1 Tenant Registration

**Feature:** New tenant sign-up with organization details

**User Story:**
As a hardware store owner, I want to register my business so that I can start using Corelio for my operations.

**Acceptance Criteria:**
- ✓ Registration form captures: Business name, RFC, email, subdomain, password
- ✓ Subdomain must be unique (e.g., `ferreteria-lopez.corelio.com.mx`)
- ✓ RFC validation against SAT format (3-4 letters + 6 digits + 3 alphanumeric)
- ✓ Password must meet complexity requirements (min 8 chars, 1 uppercase, 1 number, 1 special)
- ✓ Email verification sent within 1 minute
- ✓ Default "Owner" user created automatically
- ✓ Default warehouse created ("Almacén Principal")
- ✓ Trial period activated (30 days)
- ✓ System generates welcome email with quick-start guide

**API Endpoints:**
- `POST /api/v1/tenants/register`
- `GET /api/v1/tenants/verify-email?token={token}`

**Validation Rules:**
```csharp
public class RegisterTenantValidator : AbstractValidator<RegisterTenantCommand>
{
    public RegisterTenantValidator()
    {
        RuleFor(x => x.BusinessName).NotEmpty().MaximumLength(200);
        RuleFor(x => x.RFC).NotEmpty().Matches(@"^[A-Z&Ñ]{3,4}[0-9]{6}[A-Z0-9]{3}$");
        RuleFor(x => x.Subdomain).NotEmpty().Matches(@"^[a-z0-9-]+$").MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches(@"[0-9]").WithMessage("Password must contain number")
            .Matches(@"[^a-zA-Z0-9]").WithMessage("Password must contain special character");
    }
}
```

### 1.2 User Authentication

**Feature:** Secure login with JWT tokens

**User Story:**
As a user, I want to securely log in to access my tenant's data.

**Acceptance Criteria:**
- ✓ Login with email + password
- ✓ JWT token issued with 1-hour expiration
- ✓ Refresh token issued with 7-day expiration
- ✓ JWT contains claims: sub (user ID), tenant_id, email, roles, permissions
- ✓ Failed login attempts tracked (max 5 attempts, 15-minute lockout)
- ✓ Login attempt logged in audit_logs table
- ✓ Support for "Remember Me" (extends refresh token to 30 days)
- ✓ Two-factor authentication for Owner role (via email code)

**API Endpoints:**
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `POST /api/v1/auth/logout`
- `POST /api/v1/auth/forgot-password`
- `POST /api/v1/auth/reset-password`

**JWT Structure:**
```json
{
  "sub": "uuid",
  "email": "user@example.com",
  "tenant_id": "uuid",
  "roles": ["Owner"],
  "permissions": ["*"],
  "exp": 1735689600,
  "iat": 1735686000
}
```

### 1.3 Role-Based Access Control

**Feature:** Permission-based authorization

**Predefined Roles:**

| Role | Permissions | Description |
|------|-------------|-------------|
| Owner | `*` (all) | Full system access |
| Administrator | users.*, settings.* | User and system management |
| Cashier | sales.create, sales.view, products.view, customers.view | POS operations only |
| InventoryManager | products.*, inventory.* | Product and stock management |
| Accountant | cfdi.*, reports.*, sales.view | Financial and tax compliance |
| Seller | sales.*, customers.*, quotes.* | Sales and customer management |

**Permission Structure:**
Format: `{module}.{action}`

Examples:
- `products.view`, `products.create`, `products.edit`, `products.delete`
- `sales.create`, `sales.view`, `sales.cancel`, `sales.discount`
- `cfdi.generate`, `cfdi.cancel`

**Authorization Logic:**
```csharp
[Authorize(Policy = "products.create")]
public class CreateProductCommand : IRequest<Result<Guid>>
{
    // Command properties
}
```

---

## Module 2: Point of Sale (POS)

### 2.1 Product Search

**Feature:** Fast product lookup with multiple search methods

**User Story:**
As a cashier, I want to quickly find products by scanning barcode or typing name/SKU so that I can complete sales faster.

**Search Methods:**
1. Barcode scan (EAN13, UPC, CODE128, QR)
2. SKU entry
3. Product name (autocomplete after 3 characters)
4. Product category browsing

**Performance Requirements:**
- Search results returned in <300ms (95th percentile)
- Autocomplete shows after 3 characters
- Results sorted by relevance (exact match first, then partial)
- Maximum 20 results displayed at once

**UI Requirements:**
- Search input always focused when POS loads
- Clear visual indication when barcode scanner active
- "No results" message with suggestion to add new product
- Recent searches saved (last 10)

**API Endpoint:**
- `GET /api/v1/pos/search?q={query}&limit=20`

**Response:**
```json
{
  "products": [
    {
      "id": "uuid",
      "sku": "MART-001",
      "barcode": "7501234567890",
      "name": "Martillo de Garra 16oz",
      "price": 150.00,
      "stock": 25,
      "imageUrl": "..."
    }
  ],
  "totalResults": 1,
  "searchTime": 120
}
```

### 2.2 Shopping Cart

**Feature:** Add/remove items with quantity and price adjustments

**User Story:**
As a cashier, I want to add multiple products to a cart and adjust quantities/prices before completing the sale.

**Acceptance Criteria:**
- ✓ Add product to cart (quantity defaults to 1)
- ✓ Increment/decrement quantity with + / - buttons
- ✓ Remove item from cart
- ✓ Apply discount (percentage or fixed amount) per item or entire sale
- ✓ Calculate subtotal, tax (IVA 16%), and total in real-time
- ✓ Display cart total prominently
- ✓ Support keyboard shortcuts:
  - `+` = Add 1 to current item quantity
  - `-` = Subtract 1 from current item quantity
  - `Delete` = Remove current item
  - `F5` = Apply discount to current item
  - `F6` = Apply discount to entire sale

**Validation Rules:**
- Cannot add more quantity than available stock (unless config allows negative inventory)
- Discount cannot exceed 100%
- Minimum sale total: $0.01 MXN

**State Management:**
```csharp
public class CartState
{
    public List<CartItem> Items { get; set; } = [];
    public decimal SubTotal => Items.Sum(i => i.LineTotal);
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount => (SubTotal - DiscountAmount) * 0.16m;
    public decimal Total => SubTotal - DiscountAmount + TaxAmount;
}

public class CartItem
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Quantity { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal LineTotal => (UnitPrice * Quantity) * (1 - DiscountPercentage / 100);
}
```

### 2.3 Payment Processing

**Feature:** Multiple payment methods with change calculation

**User Story:**
As a cashier, I want to accept multiple payment methods and calculate change accurately.

**Supported Payment Methods:**
1. Cash - with change calculation
2. Card (Visa, Mastercard, AmEx) - requires authorization code
3. Bank Transfer - requires reference number
4. Check - requires check number and bank
5. Mixed Payment - combination of above

**Payment Flow:**
1. Cart finalized → 2. Select payment method → 3. Enter payment details →
4. Confirm amount → 5. Process payment → 6. Print receipt → 7. Open cash drawer (if cash)

**Cash Payment Logic:**
```csharp
public class CashPayment
{
    public decimal TotalDue { get; set; }
    public decimal AmountReceived { get; set; }
    public decimal Change => AmountReceived - TotalDue;

    public bool IsValid => AmountReceived >= TotalDue;
}
```

**Keyboard Shortcuts:**
- `F12` = Go to payment screen
- `ESC` = Cancel and return to cart
- `Enter` = Confirm payment

**Receipt Generation:**
- Thermal printer (58mm/80mm) - ESC/POS commands
- PDF receipt for email
- Include: Business name, RFC, sale date, items, subtotal, tax, total, payment method

### 2.4 Quick Customer Creation

**Feature:** Add customer during checkout without leaving POS

**User Story:**
As a cashier, I want to quickly add a new customer during checkout if they request an invoice.

**Required Fields:**
- Name (First + Last or Business Name)
- RFC (validated) - optional for ticket sales
- Email (for invoice delivery)

**Optional Fields:**
- Phone
- CURP (for individuals)
- Tax Regime
- CFDI Use Code (defaults to G03 - General Expenses)

**UI Requirements:**
- Modal dialog opens from POS screen
- Form validates RFC format in real-time
- Auto-save on modal close
- Customer immediately available for sale

---

## Module 3: Inventory Management

### 3.1 Stock Tracking

**Feature:** Real-time inventory levels per warehouse

**User Story:**
As an inventory manager, I want to see current stock levels for all products across warehouses.

**Data Display:**
- Product list with current stock quantity
- Warehouse breakdown per product
- Available quantity (total - reserved)
- Low stock indicators (below minimum level)
- Stock value (quantity × average cost)

**Stock Calculation:**
```
Available Quantity = Current Quantity - Reserved Quantity
```

Reserved quantity includes:
- Pending quotes
- Partially fulfilled orders

**Color Coding:**
- 🔴 Red: Stock = 0 (out of stock)
- 🟡 Yellow: Stock <= Minimum Level (low stock alert)
- 🟢 Green: Stock > Minimum Level (adequate stock)

### 3.2 Stock Adjustments

**Feature:** Manual inventory corrections

**User Story:**
As an inventory manager, I want to adjust stock levels to correct discrepancies found during physical counts.

**Adjustment Types:**
1. Increase (found extra stock)
2. Decrease (stock damaged, lost, stolen)

**Required Fields:**
- Product
- Warehouse
- Adjustment Type (increase/decrease)
- Quantity
- Reason Code (damaged, lost, stolen, found, correction, other)
- Notes (optional, max 500 chars)

**Validation:**
- Cannot decrease below 0 (unless config allows negative inventory)
- Adjustment must have reason code
- User must have `inventory.adjust` permission

**Audit Trail:**
- All adjustments logged in `inventory_transactions` table
- Includes: user, timestamp, before/after quantities, reason

**API Endpoint:**
- `POST /api/v1/inventory/adjustments`

---

## Module 4: Product Management

### 4.1 Product Creation

**Feature:** Add new products with complete details

**User Story:**
As an inventory manager, I want to add new products to the catalog with all necessary information.

**Required Fields:**
- Product Name
- SKU (auto-generated or manual)
- Sale Price
- Unit of Measure (PCS, KG, M, L, etc.)
- Category

**Optional Fields:**
- Barcode (EAN13, UPC, etc.)
- Cost Price
- Description
- Brand, Manufacturer, Model Number
- SAT Product Code (ClaveProdServ) - required for CFDI
- SAT Unit Code (ClaveUnidad) - required for CFDI
- Images (up to 5)
- Min/Max stock levels
- Reorder point and quantity

**Validation:**
- SKU must be unique per tenant
- Barcode must be unique per tenant (if provided)
- Sale price must be > 0
- Cost price must be >= 0
- SAT codes validated against SAT catalog

**Automatic Calculations:**
```csharp
public decimal ProfitMargin =>
    CostPrice > 0 ? ((SalePrice - CostPrice) / CostPrice) * 100 : 0;

public decimal MarkupPercentage =>
    CostPrice > 0 ? ((SalePrice - CostPrice) / SalePrice) * 100 : 0;
```

### 4.2 Product Categories

**Feature:** Hierarchical product organization

**User Story:**
As an inventory manager, I want to organize products into categories for easier navigation.

**Category Structure:**
- Up to 5 levels deep
- Example: Hardware → Tools → Hand Tools → Hammers → Claw Hammers

**Category Fields:**
- Name
- Description
- Parent Category (nullable)
- Sort Order
- Icon/Color (for UI)

**Operations:**
- Create category
- Move category (change parent)
- Delete category (only if no products assigned)

---

## Module 5: Customer Management

### 5.1 Customer Profiles

**Feature:** Complete customer information with CFDI preferences

**User Story:**
As a seller, I want to maintain detailed customer profiles for better service and accurate invoicing.

**Customer Types:**
1. Individual (Persona Física)
2. Business (Persona Moral)

**Fields (Individual):**
- First Name, Last Name
- RFC (optional for retail customers)
- CURP
- Email, Phone, Mobile

**Fields (Business):**
- Business Name
- Trade Name
- RFC (mandatory)
- Tax Regime
- Email, Phone, Website

**CFDI Preferences:**
- CFDI Use Code (G01, G02, G03, etc.)
- Preferred Payment Method (PUE or PPD)
- Preferred Payment Form (01-Cash, 03-Transfer, 04-Card)
- Invoice Email

**Credit Settings:**
- Credit Limit
- Credit Days
- Current Balance (auto-calculated from unpaid invoices)

### 5.2 Customer Search

**Feature:** Fast customer lookup

**Search By:**
- Name (fuzzy matching)
- RFC
- Phone Number
- Email

**Search Performance:**
- Results in <200ms
- Autocomplete after 3 characters
- Display up to 50 results

---

## Module 6: CFDI Compliance

### 6.1 Invoice Generation

**Feature:** Create CFDI 4.0 compliant invoices from sales

**User Story:**
As an accountant, I want to generate SAT-compliant invoices for customers who require them.

**Workflow:**
1. Select completed sale
2. Verify customer has complete CFDI data (RFC, Tax Regime, CFDI Use)
3. Auto-populate invoice from sale data
4. Review line items (ensure SAT codes present)
5. Generate XML
6. Validate XML against SAT schema
7. Sign with CSD certificate
8. Send to PAC for stamping
9. Receive UUID and SAT seal
10. Generate PDF with QR code
11. Email to customer

**CFDI Fields (Auto-Populated from Sale):**
- Series (from tenant config, e.g., "A")
- Folio (auto-increment per series)
- Issue Date (current datetime)
- Payment Method (PUE or PPD from customer)
- Payment Form (01, 03, 04 from customer)
- Currency (MXN)
- Subtotal, Discount, Taxes, Total

**SAT Validation Rules:**
- All amounts must have 2 decimal places
- RFC format valid for issuer and receiver
- SAT product code (8 digits) required per item
- SAT unit code (2-3 chars) required per item
- Tax calculations must be exact

**Error Handling:**
- PAC errors: Retry up to 3 times with exponential backoff
- Validation errors: Display clear message with field causing error
- Network errors: Queue for retry when connection restored

### 6.2 Invoice Cancellation

**Feature:** Cancel invoices with SAT-approved reasons

**User Story:**
As an accountant, I want to cancel incorrectly issued invoices per SAT regulations.

**SAT Cancellation Reasons:**
- 01: Comprobantes emitidos con errores con relación
- 02: Comprobantes emitidos con errores sin relación
- 03: No se llevó a cabo la operación
- 04: Operación nominativa relacionada en una factura global

**Workflow:**
1. Select stamped invoice (status = "stamped")
2. Choose cancellation reason
3. Request cancellation from PAC
4. PAC sends cancellation request to SAT
5. SAT approves or rejects
6. Update invoice status (cancelled or cancellation_rejected)
7. If approved, send cancellation notification to customer

**Time Limit:**
- Invoices can only be cancelled within 72 hours of stamping (SAT rule)
- System prevents cancellation after 72 hours

---

## Module 7: Reporting & Analytics

### 7.1 Daily Sales Report

**Feature:** Summary of sales by day, cashier, payment method

**Data Displayed:**
- Total sales amount
- Number of transactions
- Average ticket
- Sales by payment method breakdown
- Sales by cashier (if multiple cashiers)
- Top 10 selling products
- Hourly sales distribution

**Filters:**
- Date range
- Warehouse
- Cashier
- Payment method

**Export Options:**
- PDF
- Excel (XLSX)
- CSV

### 7.2 Inventory Valuation Report

**Feature:** Current inventory value by warehouse

**Data Displayed:**
- Product list with:
  - Current quantity
  - Average cost
  - Extended value (quantity × cost)
- Total inventory value
- Value by category breakdown
- Low stock alerts

**Valuation Method:**
- Weighted Average Cost (WAC)

**Formula:**
```
New Average Cost = (Old Quantity × Old Cost + Purchase Quantity × Purchase Cost)
                   / (Old Quantity + Purchase Quantity)
```

---

**Last Updated:** 2025-12-21
