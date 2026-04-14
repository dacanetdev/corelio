# Corelio UAT Test Script

**Tenant:** Ferretería Demo S.A. de C.V.  
**Environment:** Staging (https://staging.corelio.com.mx)  
**Prepared by:** Development Team  
**Version:** Sprint 10

> **Pre-requisite:** Run the UAT data seeder before executing these tests:
> ```bash
> dotnet run --project src/Presentation/Corelio.WebAPI -- --seed-uat
> ```
> This creates 8 categories, 52 products, 5 customers, and 30 days of historical sales for the demo tenant.

---

## Test Accounts

| User | Email | Password | Role |
|------|-------|----------|------|
| Admin | admin@demo.corelio.app | Admin123! | Administrator |
| Manager | manager@demo.corelio.app | Admin123! | Manager |
| Cashier | cashier@demo.corelio.app | Admin123! | Cashier |

---

## Workflow 1: Registro e Inicio de Sesión

**Objetivo:** Verificar que el sistema de autenticación funciona correctamente.  
**Usuario:** admin@demo.corelio.app  

| # | Paso | Acción | Resultado Esperado | ✓/✗ |
|---|------|--------|--------------------|-----|
| 1 | Abrir la aplicación | Navegar a la URL de staging | Se muestra la pantalla de inicio de sesión | |
| 2 | Ingresar credenciales | Email: `admin@demo.corelio.app`, Contraseña: `Admin123!` | Los campos se llenan correctamente | |
| 3 | Hacer clic en "Iniciar Sesión" | Clic en el botón de acceso | Redirige al dashboard principal | |
| 4 | Verificar el dashboard | Observar la pantalla principal | Se muestra el nombre del tenant "Ferretería Demo S.A. de C.V." y el nombre del usuario | |
| 5 | Cerrar sesión | Clic en menú de usuario → "Cerrar Sesión" | Redirige a la pantalla de inicio de sesión | |
| 6 | Verificar acceso con rol Cajero | Iniciar sesión con `cashier@demo.corelio.app` | Accede con menú reducido (sin módulos de configuración) | |
| 7 | Verificar restricción de acceso | Navegar manualmente a `/ajustes/precios` | Se muestra mensaje de acceso denegado o redirige al inicio | |

**Resultado Global:** ☐ Aprobado  ☐ Fallido  
**Notas:**

---

## Workflow 2: Configuración de Niveles de Precios

**Objetivo:** Verificar que el administrador puede configurar los niveles de precio y descuento.  
**Usuario:** admin@demo.corelio.app  

| # | Paso | Acción | Resultado Esperado | ✓/✗ |
|---|------|--------|--------------------|-----|
| 1 | Navegar a precios | Menú → Configuración → **Precios** (o navegar a `/ajustes/precios`) | Se muestra la pantalla de configuración de precios | |
| 2 | Verificar niveles cargados | Observar los niveles de precio existentes | Se muestran 3 niveles de descuento (Descuento 1, 2, 3) y 3 niveles de margen (Menudeo, Mayoreo, Distribuidor) | |
| 3 | Modificar nombre de nivel | Cambiar "Descuento 1" a "Descuento Comercial" | El campo acepta el nuevo nombre | |
| 4 | Guardar cambios | Clic en "Guardar" | Mensaje de confirmación "Configuración guardada" | |
| 5 | Verificar persistencia | Recargar la página | El nombre "Descuento Comercial" permanece guardado | |
| 6 | Agregar nivel adicional | Incrementar el número de niveles de descuento a 4 | Aparece un nuevo nivel "Descuento 4" | |
| 7 | Guardar y verificar | Guardar y recargar | Se muestran 4 niveles de descuento | |

**Resultado Global:** ☐ Aprobado  ☐ Fallido  
**Notas:**

---

## Workflow 3: Alta de Producto con Precios Multi-Nivel

**Objetivo:** Verificar que se puede crear un producto y configurar sus precios por nivel.  
**Usuario:** manager@demo.corelio.app  

| # | Paso | Acción | Resultado Esperado | ✓/✗ |
|---|------|--------|--------------------|-----|
| 1 | Navegar a productos | Menú → **Catálogo** → **Productos** | Se muestra la lista de productos (debe haber ~52 del seeder) | |
| 2 | Iniciar creación | Clic en "Nuevo Producto" o "+" | Se abre el formulario de creación de producto | |
| 3 | Llenar datos básicos | SKU: `TEST-001`, Nombre: `Producto de Prueba UAT`, Categoría: `Ferretería General` | Los campos se llenan sin errores | |
| 4 | Ingresar precios | Costo: `$50.00`, Precio de Venta: `$95.00` | Los campos aceptan los valores decimales | |
| 5 | Habilitar IVA | Verificar que IVA 16% esté activado | El precio con IVA calculado se muestra automáticamente ($110.20) | |
| 6 | Agregar código SAT | Código SAT: `27111700`, Unidad SAT: `H87` | Los campos se llenan correctamente | |
| 7 | Guardar producto | Clic en "Guardar" | Mensaje de confirmación y regresa a la lista | |
| 8 | Verificar en lista | Buscar "Producto de Prueba" en el buscador | El producto aparece en los resultados | |
| 9 | Editar precios por nivel | Abrir el producto → pestaña "Precios" | Se muestran los 3 niveles de precio (Menudeo, Mayoreo, Distribuidor) | |
| 10 | Configurar Menudeo | Ingresar margen 30% para Menudeo | El precio calculado se actualiza ($71.43 + IVA = $82.86) | |
| 11 | Guardar precios | Clic en "Guardar Precios" | Mensaje de confirmación | |

**Resultado Global:** ☐ Aprobado  ☐ Fallido  
**Notas:**

---

## Workflow 4: Venta en POS con Pago en Efectivo y Descarga de Ticket

**Objetivo:** Verificar que el módulo POS funciona de extremo a extremo.  
**Usuario:** cashier@demo.corelio.app  

| # | Paso | Acción | Resultado Esperado | ✓/✗ |
|---|------|--------|--------------------|-----|
| 1 | Navegar al POS | Menú → **Punto de Venta** (o navegar a `/pos`) | Se muestra la pantalla de POS de 3 paneles | |
| 2 | Buscar producto | Escribir "Martillo" en el buscador de productos | Se muestra "Martillo de uña 16 oz" ($185.00 + IVA) | |
| 3 | Agregar al carrito | Clic en el producto o presionar Enter | El producto aparece en el carrito con cantidad 1 | |
| 4 | Agregar segundo producto | Buscar "Cinta métrica" y agregar | Aparece en el carrito con cantidad 1 | |
| 5 | Modificar cantidad | Cambiar cantidad del martillo a 2 | El subtotal se actualiza correctamente ($370.00 + IVA) | |
| 6 | Verificar totales | Observar el panel de pago | Subtotal, IVA (16%) y Total se muestran correctamente | |
| 7 | Seleccionar pago en efectivo | Seleccionar método "Efectivo" | El campo de monto recibido aparece | |
| 8 | Ingresar monto recibido | Ingresar $700.00 | Se calcula y muestra el cambio ($700.00 - Total) | |
| 9 | Completar venta | Clic en "Cobrar" o "Completar Venta" | Aparece confirmación de venta completada con número de folio | |
| 10 | Descargar ticket | Clic en "Imprimir Ticket" o "Descargar" | Se descarga o muestra el ticket en PDF con los datos correctos | |
| 11 | Verificar folio en historial | Navegar a Ventas → Historial | La venta recién creada aparece en la lista | |

**Resultado Global:** ☐ Aprobado  ☐ Fallido  
**Notas:**

---

## Workflow 5: Historial de Ventas y Cancelación

**Objetivo:** Verificar que el historial de ventas es accesible y se puede cancelar una venta.  
**Usuario:** manager@demo.corelio.app  

| # | Paso | Acción | Resultado Esperado | ✓/✗ |
|---|------|--------|--------------------|-----|
| 1 | Navegar al historial | Menú → **Ventas** → **Historial** | Se muestra la lista de ventas con paginación | |
| 2 | Verificar ventas históricas | Observar la lista | Se muestran ~30 ventas de los últimos 30 días (del seeder) | |
| 3 | Filtrar por fecha | Seleccionar rango de últimos 7 días | Solo se muestran ventas del período seleccionado | |
| 4 | Ver detalle de venta | Clic en cualquier venta de la lista | Se muestra el detalle completo (cliente, artículos, totales, pagos) | |
| 5 | Regresar a la lista | Clic en "Regresar" o el botón de retroceso | Regresa a la lista de ventas | |
| 6 | Seleccionar venta a cancelar | Abrir una venta con estado "Completada" | Se muestra el detalle con el botón "Cancelar Venta" | |
| 7 | Iniciar cancelación | Clic en "Cancelar Venta" | Aparece diálogo de confirmación | |
| 8 | Confirmar cancelación | Confirmar la cancelación en el diálogo | La venta cambia a estado "Cancelada" | |
| 9 | Verificar estado | Observar el estado en la lista | La venta muestra estado "Cancelada" con fecha de cancelación | |

**Resultado Global:** ☐ Aprobado  ☐ Fallido  
**Notas:**

---

## Workflow 6: Ajuste de Inventario Manual

**Objetivo:** Verificar que se puede ajustar el inventario con un motivo.  
**Usuario:** manager@demo.corelio.app  

| # | Paso | Acción | Resultado Esperado | ✓/✗ |
|---|------|--------|--------------------|-----|
| 1 | Navegar a inventario | Menú → **Inventario** | Se muestra la lista de artículos del inventario | |
| 2 | Verificar stock inicial | Buscar "Martillo de uña" | Se muestra el stock actual (150 del seeder) | |
| 3 | Abrir ajuste | Clic en el producto → "Ajustar Stock" | Se abre el formulario de ajuste de inventario | |
| 4 | Ingresar ajuste positivo | Cantidad: `+10`, Motivo: `Recepción de mercancía` | Los campos se llenan correctamente | |
| 5 | Guardar ajuste | Clic en "Guardar Ajuste" | Mensaje de confirmación; el stock actualiza a 160 | |
| 6 | Verificar en lista | Regresar a la lista de inventario | El stock del martillo muestra 160 unidades | |
| 7 | Ajuste negativo | Abrir ajuste nuevamente y ingresar `-5`, Motivo: `Merma por daño` | El stock se reduce a 155 unidades | |
| 8 | Verificar historial de movimientos | Clic en "Ver Movimientos" del producto | Se muestran los dos ajustes realizados con fecha, cantidad y motivo | |

**Resultado Global:** ☐ Aprobado  ☐ Fallido  
**Notas:**

---

## Workflow 7: Generación y Timbre de Factura CFDI

**Objetivo:** Verificar que se puede generar y timbrar una factura CFDI para una venta completada.  
**Usuario:** admin@demo.corelio.app  

**Pre-requisito:** Configurar los datos del emisor CFDI en `/ajustes/cfdi` (RFC, nombre, régimen fiscal, serie "A").

| # | Paso | Acción | Resultado Esperado | ✓/✗ |
|---|------|--------|--------------------|-----|
| 1 | Verificar configuración CFDI | Navegar a Configuración → **Facturación CFDI** | Se muestran los datos del emisor (RFC: FDE010101ABC) | |
| 2 | Navegar a facturas | Menú → **Facturación** → **Facturas** | Se muestra la lista de facturas | |
| 3 | Generar nueva factura | Clic en "Nueva Factura" | Se abre el diálogo de generación con selector de venta | |
| 4 | Seleccionar venta | Seleccionar una venta completada con cliente (p.ej. "Juan García López") | Los datos del receptor se llenan automáticamente (RFC: GALJ800101HDF) | |
| 5 | Verificar datos CFDI | Observar: RFC receptor, régimen fiscal (605), uso CFDI (G03), forma de pago | Los datos son correctos | |
| 6 | Generar factura en borrador | Clic en "Generar Factura" | La factura se crea en estado "Borrador" con folio A-00001 | |
| 7 | Verificar borrador | Ver detalle de la factura | Se muestran todos los conceptos (productos vendidos) con clave SAT y montos | |
| 8 | Timbrar factura | Clic en "Timbrar con PAC" | Aparece diálogo de confirmación | |
| 9 | Confirmar timbre | Confirmar en el diálogo | La factura cambia a estado "Timbrada" con UUID asignado | |
| 10 | Verificar UUID | Observar el UUID en el detalle | Se muestra un UUID de 36 caracteres (del mock PAC) | |
| 11 | Descargar XML | Clic en "Descargar XML" | Se descarga el archivo XML firmado | |
| 12 | Descargar PDF | Clic en "Descargar PDF" | Se descarga el PDF de la factura con código QR | |
| 13 | Cancelar factura (prueba) | Clic en "Cancelar Factura" | Aparece diálogo con motivos de cancelación SAT (01, 02, 03, 04) | |
| 14 | Seleccionar motivo y cancelar | Seleccionar "01 - Comprobante emitido con errores con relación" y confirmar | La factura cambia a estado "Cancelada" | |

**Resultado Global:** ☐ Aprobado  ☐ Fallido  
**Notas:**

---

## Resumen de Resultados UAT

| # | Workflow | Resultado | Bugs Encontrados |
|---|----------|-----------|-----------------|
| 1 | Registro e Inicio de Sesión | ☐ Aprobado / ☐ Fallido | |
| 2 | Configuración de Niveles de Precios | ☐ Aprobado / ☐ Fallido | |
| 3 | Alta de Producto con Precios Multi-Nivel | ☐ Aprobado / ☐ Fallido | |
| 4 | Venta en POS con Pago en Efectivo | ☐ Aprobado / ☐ Fallido | |
| 5 | Historial de Ventas y Cancelación | ☐ Aprobado / ☐ Fallido | |
| 6 | Ajuste de Inventario Manual | ☐ Aprobado / ☐ Fallido | |
| 7 | Generación y Timbre de Factura CFDI | ☐ Aprobado / ☐ Fallido | |

### Criterio de Aceptación
- **✅ Listo para producción:** Todos los workflows aprobados y 0 bugs P0
- **⚠️ Requiere correcciones:** 1+ workflows fallidos o bugs P0 encontrados

### Definición de Severidad de Bugs
| Nivel | Descripción | Ejemplo |
|-------|-------------|---------|
| **P0 – Bloqueante** | Impide completar el workflow; sin workaround | Error al timbrar CFDI, no se puede completar venta |
| **P1 – Alta** | Funcionalidad incorrecta; workaround disponible | Monto de cambio calculado incorrectamente |
| **P2 – Media** | Problema cosmético o de usabilidad | Texto mal alineado, mensaje de error confuso |

---

## Firma de Aprobación

| Rol | Nombre | Firma | Fecha |
|-----|--------|-------|-------|
| Stakeholder / Dueño del Producto | | | |
| QA Lead | | | |
| Tech Lead | | | |

> **Nota:** Esta aprobación certifica que el MVP de Corelio está listo para su despliegue en producción y el onboarding del primer tenant piloto.
