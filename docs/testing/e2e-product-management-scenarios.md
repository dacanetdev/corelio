# E2E Test Scenarios: Product Management (TD-3.1.C)

**Feature:** Product Management (`/productos`)  
**Application:** Corelio BlazorApp  
**Tester:** QA / Developer  
**Environment:** Staging or local Aspire (`http://localhost:5001`)

---

## Prerequisites

- Logged in as an admin user (role: `Admin` or `Manager`)
- At least one product category exists in the system
- Browser: Chrome or Edge (latest)

---

## Scenario 1 — Navigate to Product List

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Open the sidebar and click **Productos** | Navigates to `/productos` |
| 2 | Observe the page | Data grid loads with columns: SKU, Nombre, Categoría, Precio Venta, Activo |
| 3 | Observe the header | "Productos" title visible; **Nuevo Producto** button visible |
| 4 | Observe pagination | Page size selector and page indicator visible |

**Pass Criteria:** Grid loads without error, all columns visible in Spanish.

---

## Scenario 2 — Create a New Product

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | Click **Nuevo Producto** | Create product form opens (dialog or page at `/productos/nuevo`) |
| 2 | Leave all fields empty and click **Guardar** | Validation errors appear: "SKU es requerido", "Nombre es requerido", "Precio de Venta es requerido" |
| 3 | Enter SKU: `TEST-001`, Name: `Tornillo 1/4"`, SalePrice: `12.50`, CostPrice: `7.00` | Fields accept input |
| 4 | Select a category from the dropdown | Category selected |
| 5 | Toggle **IVA habilitado** ON | Tax rate field becomes visible/enabled |
| 6 | Click **Guardar** | Success notification appears; redirects to product list |
| 7 | Observe the list | New product `TEST-001 - Tornillo 1/4"` appears in the grid |

**Pass Criteria:** Product persisted; visible in list; all fields correctly saved.

---

## Scenario 3 — Edit an Existing Product

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | In the product list, click the **Edit** icon next to `TEST-001` | Edit form opens pre-populated with existing values |
| 2 | Verify all fields show the saved values | SKU: `TEST-001`, Name: `Tornillo 1/4"`, Price: `12.50` |
| 3 | Change Name to `Tornillo 1/4" Galvanizado` and Price to `14.99` | Fields accept new values |
| 4 | Click **Guardar** | Success notification; redirects to list |
| 5 | Observe the list | Updated name and price reflected in the row |

**Pass Criteria:** Changes persisted; old values replaced; no duplicate rows.

---

## Scenario 4 — Delete a Product

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | In the product list, click the **Delete** icon next to `TEST-001` | Confirmation dialog appears: "¿Eliminar producto?" |
| 2 | Click **Cancelar** | Dialog closes; product still visible in list |
| 3 | Click **Delete** icon again | Dialog appears again |
| 4 | Click **Eliminar** | Success notification; product removed from list |
| 5 | Search for `TEST-001` | No results found |

**Pass Criteria:** Soft delete works; product no longer appears in queries.

---

## Scenario 5 — Search Products

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | In the search box at the top of the product list, type `Tornillo` | Grid filters in real-time (or on submit) to show only products matching "Tornillo" |
| 2 | Clear the search box | All products shown again |
| 3 | Type a SKU fragment (e.g., `CEM`) | Grid shows only products with SKU containing "CEM" |
| 4 | Type a non-existent term (e.g., `XXXXXXXXXXX`) | Grid shows "No se encontraron productos" empty state |

**Pass Criteria:** Search works on Name, SKU, and Barcode fields; empty state shows in Spanish.

---

## Scenario 6 — Filter by Category

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | In the category filter dropdown, select an existing category | Grid filters to show only products in that category |
| 2 | Verify the count matches expectation | Only products of the selected category shown |
| 3 | Select **Todas las categorías** (All) | All products shown again |

**Pass Criteria:** Category filter correctly isolates products.

---

## Scenario 7 — Pagination

| Step | Action | Expected Result |
|------|--------|-----------------|
| 1 | If less than 10 products exist, add 15 products via the form | 15 products visible |
| 2 | Observe pagination: page 1 of 2 (with page size 10) | First 10 products shown |
| 3 | Click **Next Page** | Next 5 products shown |
| 4 | Click **Previous Page** | Returns to first 10 products |
| 5 | Change page size to 25 | All products visible on one page |

**Pass Criteria:** Pagination controls work; correct page size applied.

---

## Regression Check

After running all scenarios, verify:

- [ ] Product list still loads correctly
- [ ] No JavaScript console errors
- [ ] No HTTP 500 errors in the browser network tab
- [ ] All labels and messages are in Spanish (es-MX)
- [ ] Currency values display as `$1,234.56` format (MXN)

---

## Known Limitations / Notes

- Soft-deleted products are hidden from the UI but remain in the database for audit purposes
- SKU must be unique per tenant; duplicate SKU will show a validation error
- IVA (16%) is automatically calculated when `IVA habilitado` is toggled ON
