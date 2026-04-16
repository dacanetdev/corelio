# Manual de Usuario — Corelio ERP

**Versión:** Sprint 10  
**Idioma:** Español (es-MX)  
**Última actualización:** 2026-04-16

> Corelio es un sistema ERP en la nube diseñado para ferreterías mexicanas. Permite gestionar productos, ventas, inventario y facturación electrónica CFDI 4.0 desde cualquier navegador.

---

## Tabla de Contenidos

1. [Mapa de la Aplicación](#1-mapa-de-la-aplicación)
2. [Roles y Permisos](#2-roles-y-permisos)
3. [Flujo 1 — Inicio de Sesión y Navegación](#3-flujo-1--inicio-de-sesión-y-navegación)
4. [Flujo 2 — Configuración de Niveles de Precios](#4-flujo-2--configuración-de-niveles-de-precios)
5. [Flujo 3 — Gestión de Productos](#5-flujo-3--gestión-de-productos)
6. [Flujo 4 — Venta en Punto de Venta (POS)](#6-flujo-4--venta-en-punto-de-venta-pos)
7. [Flujo 5 — Historial de Ventas y Cancelación](#7-flujo-5--historial-de-ventas-y-cancelación)
8. [Flujo 6 — Gestión de Inventario](#8-flujo-6--gestión-de-inventario)
9. [Flujo 7 — Facturación Electrónica CFDI](#9-flujo-7--facturación-electrónica-cfdi)
10. [Mensajes de Error Comunes](#10-mensajes-de-error-comunes)

---

## 1. Mapa de la Aplicación

```
Corelio
├── Inicio (/)
├── Punto de Venta (/pos)
├── Ventas
│   ├── Historial (/sales)
│   └── Cotizaciones (/cotizaciones)
├── Catálogo
│   └── Productos (/products)
├── Clientes (/customers)
├── Inventario (/inventory)
├── Facturación
│   └── Facturas CFDI (/facturas)
└── Configuración
    ├── Precios (/settings/pricing)
    └── Facturación CFDI (/settings/cfdi)
```

---

## 2. Roles y Permisos

Corelio maneja tres roles predeterminados. El administrador puede crear roles personalizados.

| Módulo | Cajero | Gerente | Administrador |
|--------|:------:|:-------:|:-------------:|
| Punto de Venta (POS) | ✅ | ✅ | ✅ |
| Historial de Ventas | ✅ | ✅ | ✅ |
| Cancelar Ventas | — | ✅ | ✅ |
| Gestión de Productos | — | ✅ | ✅ |
| Gestión de Clientes | ✅ | ✅ | ✅ |
| Ajuste de Inventario | — | ✅ | ✅ |
| Facturación CFDI | — | — | ✅ |
| Configuración de Precios | — | — | ✅ |
| Gestión de Usuarios | — | — | ✅ |

> **Nota:** Si ve el mensaje _"Acceso Denegado"_ al intentar entrar a una sección, significa que su rol no tiene permiso para ese módulo. Contacte al administrador de su empresa.

---

## 3. Flujo 1 — Inicio de Sesión y Navegación

**Usuarios:** Todos los roles  
**Ruta:** `/auth/login`

### 3.1 Iniciar Sesión

1. Abra su navegador y vaya a la URL de Corelio proporcionada por su administrador.
2. Ingrese su **correo electrónico** y **contraseña** en los campos correspondientes.
3. Haga clic en **"Iniciar Sesión"**.
4. Si las credenciales son correctas, será redirigido al panel principal. Verá el nombre de su empresa en la barra superior.

**Si olvidó su contraseña:**
1. En la pantalla de inicio de sesión, haga clic en **"¿Olvidaste tu contraseña?"**.
2. Ingrese su correo electrónico y haga clic en **"Enviar instrucciones"**.
3. Revise su correo y siga el enlace recibido (válido por 1 hora).
4. Ingrese su nueva contraseña (mínimo 8 caracteres, incluya mayúsculas y números).

### 3.2 Navegar en la Aplicación

- El **menú lateral izquierdo** contiene todos los módulos disponibles para su rol.
- La **barra superior** muestra el nombre del tenant, nombre de usuario y el menú de cuenta.
- Para **cerrar sesión**: haga clic en su nombre de usuario (esquina superior derecha) → **"Cerrar Sesión"**.

### 3.3 Errores Comunes

| Error | Causa | Solución |
|-------|-------|----------|
| "Correo o contraseña incorrectos" | Credenciales inválidas | Verifique mayúsculas/minúsculas; use recuperación de contraseña |
| "Su sesión ha expirado" | Token de sesión vencido (8 horas) | Inicie sesión nuevamente |
| "Acceso Denegado" | Su rol no tiene permiso | Contacte al administrador |

---

## 4. Flujo 2 — Configuración de Niveles de Precios

**Usuarios:** Administrador  
**Ruta:** `/settings/pricing`

Corelio permite configurar múltiples **niveles de precio** para aplicar diferentes tarifas según el tipo de cliente (menudeo, mayoreo, distribuidor) o descuentos comerciales.

### 4.1 Ver y Editar Niveles de Precio

1. En el menú lateral, navegue a **Configuración → Precios**.
2. Verá los niveles de precio actuales divididos en dos categorías:
   - **Niveles de Descuento** (porcentajes de descuento fijo sobre el precio de venta)
   - **Niveles de Margen** (precio calculado a partir del costo + margen)
3. Para **renombrar** un nivel: haga clic en el campo de nombre, edite y presione Enter.
4. Para **cambiar el porcentaje** de descuento o margen: edite el valor en el campo correspondiente.
5. Haga clic en **"Guardar Configuración"** para aplicar los cambios.

### 4.2 Agregar Nuevos Niveles

1. En la sección correspondiente (Descuentos o Márgenes), haga clic en **"Agregar Nivel"** (ícono `+`).
2. Ingrese el nombre y el porcentaje del nuevo nivel.
3. Haga clic en **"Guardar Configuración"**.

> **Nota:** Los cambios en los niveles de precio se reflejan inmediatamente en los cálculos del módulo de **Precios de Productos** (`/pricing`).

### 4.3 Aplicar Precios a Productos (Bulk Update)

1. Navegue a **Configuración → Precios → Actualización Masiva**.
2. Seleccione la categoría o productos que desea actualizar.
3. Elija el nivel de precio y el porcentaje de ajuste.
4. Haga clic en **"Aplicar"** — el sistema recalculará y guardará los precios de todos los productos seleccionados.

---

## 5. Flujo 3 — Gestión de Productos

**Usuarios:** Gerente, Administrador  
**Ruta:** `/products`

### 5.1 Consultar el Catálogo de Productos

1. En el menú lateral, haga clic en **Catálogo → Productos**.
2. Verá la lista paginada de productos con nombre, SKU, precio de venta y estado (Activo/Inactivo).
3. Use la **barra de búsqueda** para filtrar por nombre, SKU o código de barras.
4. Use el filtro de **Categoría** para ver productos de un grupo específico.

### 5.2 Crear un Nuevo Producto

1. En la lista de productos, haga clic en **"Nuevo Producto"** (botón azul, esquina superior derecha).
2. Complete el formulario:

   | Campo | Descripción | Requerido |
   |-------|-------------|:---------:|
   | SKU | Código único del producto | ✅ |
   | Nombre | Nombre del producto (visible en POS) | ✅ |
   | Categoría | Grupo al que pertenece | ✅ |
   | Código de Barras | Para búsqueda rápida en POS | — |
   | Costo | Precio de compra (base para márgenes) | ✅ |
   | Precio de Venta | Precio al menudeo (incluye IVA si aplica) | ✅ |
   | IVA 16% | Indica si el precio lleva IVA | — |
   | Clave SAT | Código de producto/servicio del catálogo SAT | Para CFDI |
   | Unidad SAT | Clave de unidad de medida SAT (ej. `H87` = pieza) | Para CFDI |
   | Descripción | Descripción larga del producto | — |

3. Haga clic en **"Guardar"**. El producto queda activo e inmediatamente disponible en el POS.

### 5.3 Editar un Producto

1. En la lista, haga clic en el **nombre del producto** o en el ícono de editar (lápiz).
2. Modifique los campos necesarios.
3. Haga clic en **"Guardar"**.

### 5.4 Configurar Precios Multi-Nivel

1. Abra el producto que desea configurar.
2. Vaya a la pestaña **"Precios"**.
3. Para cada nivel de precio configurado (Menudeo, Mayoreo, Distribuidor):
   - Ingrese el **margen** como porcentaje, **o** ingrese el **precio directo**.
   - El sistema calculará automáticamente el precio final con IVA.
4. Haga clic en **"Guardar Precios"**.

> Los precios por nivel se aplican automáticamente en el POS cuando el cajero selecciona el nivel de cliente correspondiente.

### 5.5 Desactivar un Producto

1. Abra el producto.
2. Desactive el interruptor **"Activo"**.
3. Guarde los cambios. El producto ya no aparecerá en búsquedas del POS ni en el catálogo.

---

## 6. Flujo 4 — Venta en Punto de Venta (POS)

**Usuarios:** Cajero, Gerente, Administrador  
**Ruta:** `/pos`

El módulo POS está diseñado para ventas rápidas. La pantalla se divide en tres paneles:
- **Izquierda:** Buscador de productos
- **Centro:** Carrito de compra
- **Derecha:** Panel de pago

### 6.1 Realizar una Venta

1. Navegue al **Punto de Venta** desde el menú lateral.
2. **Buscar un producto:**
   - Escriba el nombre, SKU o código de barras en la barra de búsqueda (panel izquierdo).
   - Los resultados aparecen en tiempo real. Haga clic en el producto para agregarlo al carrito, **o** use un lector de código de barras (el cursor debe estar en la barra de búsqueda).
3. **Ajustar cantidades:**
   - En el carrito (panel central), use los botones `+` y `-` para cambiar la cantidad, o edite el campo de cantidad directamente.
   - Para **eliminar** un artículo del carrito: haga clic en el ícono de basura (🗑) en la fila del producto.
4. **Revisar totales:**
   - El panel derecho muestra: Subtotal, IVA (16%) y Total.
5. **Seleccionar método de pago:**
   - Elija **Efectivo**, **Tarjeta** u otro método disponible.
   - Para pago en efectivo: ingrese el monto recibido. El sistema calcula el cambio automáticamente.
6. Haga clic en **"Cobrar"** para completar la venta.
7. Aparecerá una confirmación con el **número de folio** de la venta.
8. Haga clic en **"Imprimir/Descargar Ticket"** para obtener el comprobante en PDF.

### 6.2 Guardar como Cotización

Si el cliente desea el presupuesto pero no pagará de inmediato:

1. En lugar de hacer clic en "Cobrar", haga clic en **"Guardar como Cotización"**.
2. La cotización queda guardada en **Ventas → Cotizaciones** (`/cotizaciones`).
3. Cuando el cliente regrese a pagar, abra la cotización y haga clic en **"Convertir a Venta"** — el carrito se precarga automáticamente en el POS.

### 6.3 Asociar Cliente a la Venta

Para generar CFDI, la venta debe tener un cliente con RFC válido:

1. En el panel de pago (derecho), haga clic en **"Buscar Cliente"**.
2. Escriba el nombre o RFC del cliente.
3. Seleccione el cliente de los resultados.
4. El nombre del cliente aparecerá en el panel de pago antes de cobrar.

---

## 7. Flujo 5 — Historial de Ventas y Cancelación

**Usuarios:** Cajero (solo consulta), Gerente, Administrador  
**Ruta:** `/sales`

### 7.1 Ver el Historial de Ventas

1. En el menú lateral, navegue a **Ventas → Historial**.
2. Verá la lista de todas las ventas del tenant, ordenadas por fecha (más recientes primero).
3. **Filtros disponibles:**
   - **Rango de fechas:** Seleccione fecha inicio y fecha fin.
   - **Estado:** Completada, Cancelada, Borrador.
   - **Folio:** Busque por número de folio específico.
4. Haga clic en cualquier venta para ver su **detalle completo** (artículos, pagos, cliente, totales).

### 7.2 Descargar el Ticket de una Venta

1. Abra el detalle de la venta.
2. Haga clic en **"Descargar Ticket"** (botón PDF).
3. El ticket se descarga automáticamente.

### 7.3 Cancelar una Venta

> **Permisos requeridos:** Gerente o Administrador.  
> **Importante:** Al cancelar una venta completada, el stock de los productos vendidos se **restaura automáticamente** al inventario.

1. Abra el detalle de la venta que desea cancelar.
2. Haga clic en **"Cancelar Venta"**.
3. Confirme la cancelación en el diálogo que aparece.
4. La venta cambia a estado **"Cancelada"** y el inventario se actualiza.

> **Nota:** Las ventas canceladas no pueden reactivarse. Si necesita revertir, deberá crear una nueva venta.

---

## 8. Flujo 6 — Gestión de Inventario

**Usuarios:** Gerente, Administrador  
**Ruta:** `/inventory`

### 8.1 Ver el Inventario Actual

1. En el menú lateral, navegue a **Inventario**.
2. Verá la lista de todos los productos con su **stock actual** en el almacén principal.
3. Use el buscador para filtrar por nombre de producto o SKU.
4. El color del stock indica el estado:
   - **Verde:** Stock saludable
   - **Amarillo:** Stock bajo (cerca del mínimo configurado)
   - **Rojo:** Sin stock o por debajo del mínimo

### 8.2 Ajustar Stock Manualmente

Use esta función para registrar recepciones de mercancía, mermas, daños o diferencias de inventario.

1. Haga clic en el producto que desea ajustar.
2. Haga clic en **"Ajustar Stock"** (botón en el detalle del producto).
3. Ingrese la **cantidad** del ajuste:
   - Valor **positivo** (ej. `+10`): incrementa el stock (recepción de mercancía)
   - Valor **negativo** (ej. `-5`): reduce el stock (merma, daño, robo)
4. Seleccione o escriba el **motivo** del ajuste:
   - Recepción de mercancía
   - Merma por daño
   - Diferencia de inventario
   - Ajuste por conteo físico
   - Otro (especifique en el campo de notas)
5. Haga clic en **"Guardar Ajuste"**.
6. El stock se actualiza inmediatamente y el movimiento queda registrado.

### 8.3 Ver el Historial de Movimientos

1. En el detalle del producto de inventario, haga clic en **"Ver Movimientos"** (o navegue a `/inventory/{id}/history`).
2. Verá todos los movimientos de stock del producto:
   - Ventas completadas (reducción automática)
   - Cancelaciones (incremento automático)
   - Ajustes manuales (con motivo)
3. Cada movimiento muestra: fecha, tipo, cantidad, motivo y usuario que lo realizó.

---

## 9. Flujo 7 — Facturación Electrónica CFDI

**Usuarios:** Administrador  
**Ruta:** `/facturas`  
**Pre-requisito:** Certificado CSD configurado en `/settings/cfdi` (ver [Guía de Certificado CSD](cfdi-certificate-guide.md))

### 9.1 Verificar la Configuración CFDI

Antes de generar facturas, confirme que la configuración del emisor esté completa:

1. Navegue a **Configuración → Facturación CFDI** (`/settings/cfdi`).
2. Verifique que los siguientes campos estén completos:
   - RFC del emisor (ej. `FDE010101ABC`)
   - Razón social
   - Régimen fiscal (clave SAT, ej. `601` = General de Ley)
   - Código postal del lugar de expedición
   - Serie de folio (ej. `A`)
   - Certificado CSD (.pfx) cargado y válido
3. Si falta algún dato, complete el formulario y haga clic en **"Guardar Configuración"**.

### 9.2 Generar una Factura CFDI

1. Navegue a **Facturación → Facturas** (`/facturas`).
2. Haga clic en **"Nueva Factura"**.
3. En el diálogo de generación:
   - Seleccione la **venta completada** para la que desea generar la factura. La venta debe tener un cliente con RFC válido.
   - Verifique los datos del receptor (RFC, régimen fiscal, uso del CFDI).
   - Seleccione el **uso del CFDI** (ej. `G03` = Gastos en general).
4. Haga clic en **"Generar Factura"**.
5. La factura se crea en estado **"Borrador"** con su folio asignado (ej. A-00001).

### 9.3 Timbrar la Factura (Enviar al PAC)

El timbrado envía la factura al Proveedor Autorizado de Certificación (PAC) para obtener el **Folio Fiscal (UUID)** del SAT.

1. Abra el detalle de la factura en estado "Borrador".
2. Revise todos los conceptos y datos.
3. Haga clic en **"Timbrar con PAC"**.
4. Confirme la acción en el diálogo.
5. Si el timbrado es exitoso:
   - La factura cambia a estado **"Timbrada"**.
   - Se muestra el UUID del SAT (36 caracteres, ej. `6B7A2C3D-...`).
6. Descargue el **XML** y el **PDF** con los botones correspondientes.

> **Nota:** Una vez timbrada, la factura es un documento fiscal oficial. Para invalidarla, debe cancelarla ante el SAT (ver sección 9.4).

### 9.4 Cancelar una Factura CFDI

> La cancelación ante el SAT solo es posible dentro de las **72 horas** posteriores al timbrado.

1. Abra el detalle de la factura timbrada.
2. Haga clic en **"Cancelar Factura"**.
3. En el diálogo, seleccione el **motivo de cancelación SAT**:

   | Clave | Motivo |
   |-------|--------|
   | 01 | Comprobante emitido con errores con relación |
   | 02 | Comprobante emitido con errores sin relación |
   | 03 | No se llevó a cabo la operación |
   | 04 | Operación nominativa relacionada en la factura global |

4. Confirme la cancelación.
5. La factura cambia a estado **"Cancelada"**.

### 9.5 Descargar el XML y el PDF

Desde el detalle de cualquier factura timbrada:
- **"Descargar XML"** → Descarga el XML firmado con sello del PAC (para registros contables).
- **"Descargar PDF"** → Descarga el PDF con código QR SAT (para entregar al cliente).

---

## 10. Mensajes de Error Comunes

| Mensaje | Causa | Solución |
|---------|-------|----------|
| "Sin stock suficiente" | El inventario no tiene las unidades requeridas | Ajuste el stock o reduzca la cantidad en el carrito |
| "El cliente no tiene RFC válido" | El receptor no tiene RFC para CFDI | Actualice el RFC del cliente en `/customers/{id}` |
| "Error al timbrar: certificado vencido" | El CSD del emisor expiró (vigencia 4 años) | Renueve el CSD con el SAT y vuelva a cargarlo |
| "Error al timbrar: RFC emisor no coincide" | El RFC del certificado no coincide con el configurado | Verifique RFC en `/settings/cfdi` y el CSD cargado |
| "La factura ya no puede cancelarse (72h)" | Han pasado más de 72 horas desde el timbrado | Contacte directamente al SAT para cancelación extemporánea |
| "El producto ya no está disponible" | El producto fue desactivado | Reactive el producto en `/products` o retire del carrito |
| "Su sesión ha expirado" | El token JWT venció tras 8 horas de inactividad | Inicie sesión nuevamente |
| "Error de red" | Pérdida de conexión a internet | Verifique su conexión y reintente |

---

## Soporte

Para reportar problemas o solicitar asistencia:
- **Correo:** soporte@corelio.com.mx
- **Teléfono:** +52 (55) 0000-0000
- **Horario:** Lunes a Viernes, 9:00 am — 6:00 pm (hora CDMX)

---

*Corelio ERP — Sistema de gestión para ferreterías mexicanas*  
*Desarrollado conforme a CFDI 4.0 (SAT México)*
