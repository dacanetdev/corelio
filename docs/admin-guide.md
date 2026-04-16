# Guía de Administrador — Corelio ERP

**Versión:** Sprint 10  
**Audiencia:** Administradores del tenant  
**Última actualización:** 2026-04-16

> Esta guía cubre las funciones exclusivas del rol **Administrador**: registro de tenant, gestión de usuarios y roles, configuración de precios, y configuración de facturación CFDI.

---

## Tabla de Contenidos

1. [Registro del Tenant](#1-registro-del-tenant)
2. [Gestión de Usuarios](#2-gestión-de-usuarios)
3. [Configuración de Roles y Permisos](#3-configuración-de-roles-y-permisos)
4. [Configuración de Precios](#4-configuración-de-precios)
5. [Configuración de Facturación CFDI](#5-configuración-de-facturación-cfdi)
6. [Seguridad y Gestión de Sesiones](#6-seguridad-y-gestión-de-sesiones)
7. [Referencia de Permisos](#7-referencia-de-permisos)

---

## 1. Registro del Tenant

Un **tenant** es la instancia de Corelio para su empresa. Cada empresa opera en un entorno completamente aislado de otros tenants.

### 1.1 Crear un Nuevo Tenant

El registro se realiza una sola vez por empresa. El primer usuario registrado es automáticamente asignado como **Administrador**.

1. Navegue a la URL de Corelio y haga clic en **"Registrar empresa"** (en la pantalla de inicio de sesión).
2. Complete el formulario de registro:

   | Campo | Descripción | Ejemplo |
   |-------|-------------|---------|
   | Nombre de la empresa | Razón social completa | `Ferretería López S.A. de C.V.` |
   | RFC | RFC de la empresa (13 dígitos) | `FLO850101ABC` |
   | Su nombre completo | Nombre del administrador principal | `Carlos López Martínez` |
   | Correo electrónico | Correo del administrador | `admin@ferreteria.com` |
   | Contraseña | Mínimo 8 caracteres, incluya mayúsculas y números | |

3. Haga clic en **"Registrar"**.
4. Recibirá un correo de confirmación. Haga clic en el enlace para verificar su cuenta.
5. Inicie sesión con las credenciales registradas.

> **Importante:** El RFC del tenant se usa como RFC del emisor CFDI por defecto. Asegúrese de ingresarlo correctamente.

### 1.2 Configuración Inicial Recomendada

Tras el registro, complete estos pasos antes de usar el sistema:

- [ ] Configurar categorías de productos
- [ ] Configurar niveles de precio (ver sección 4)
- [ ] Cargar el catálogo de productos
- [ ] Crear usuarios para su equipo (ver sección 2)
- [ ] Configurar los datos CFDI y cargar el certificado CSD (ver sección 5)

---

## 2. Gestión de Usuarios

### 2.1 Ver Usuarios Existentes

1. En el menú lateral, navegue a **Configuración → Usuarios** (si está disponible en su versión).
2. Verá la lista de todos los usuarios del tenant con su nombre, correo y rol asignado.

### 2.2 Crear un Nuevo Usuario

1. Haga clic en **"Nuevo Usuario"**.
2. Complete el formulario:

   | Campo | Descripción |
   |-------|-------------|
   | Nombre completo | Nombre y apellido del usuario |
   | Correo electrónico | Se usa como nombre de usuario para iniciar sesión |
   | Contraseña temporal | El usuario deberá cambiarla en su primer inicio de sesión |
   | Rol | Asigne un rol de la lista (ver sección 3) |

3. Haga clic en **"Crear Usuario"**.
4. Comparta las credenciales temporales con el usuario de forma segura.

### 2.3 Editar un Usuario

1. En la lista de usuarios, haga clic en el nombre del usuario.
2. Modifique los campos necesarios (nombre, correo, rol).
3. Haga clic en **"Guardar"**.

### 2.4 Desactivar un Usuario

Cuando un empleado deja la empresa, desactívelo en lugar de eliminarlo para conservar el historial de sus transacciones:

1. Abra el perfil del usuario.
2. Desactive el interruptor **"Activo"**.
3. Guarde los cambios.
4. El usuario no podrá iniciar sesión. Sus transacciones históricas se conservan.

### 2.5 Restablecer Contraseña de un Usuario

Si un usuario olvida su contraseña, puede restablecerla desde la pantalla de inicio de sesión (opción "¿Olvidaste tu contraseña?"), o el administrador puede hacerlo desde el perfil del usuario:

1. Abra el perfil del usuario.
2. Haga clic en **"Restablecer Contraseña"**.
3. El usuario recibirá un correo con instrucciones.

---

## 3. Configuración de Roles y Permisos

### 3.1 Roles Predeterminados

Corelio incluye tres roles predefinidos que no pueden eliminarse:

| Rol | Descripción | Casos de Uso |
|-----|-------------|--------------|
| **Administrador** | Acceso total al sistema | Dueño, gerente general |
| **Gerente** | Ventas, inventario y productos. Sin configuración avanzada | Gerente de sucursal |
| **Cajero** | Solo POS, consulta de ventas y clientes | Personal de mostrador |

### 3.2 Crear un Rol Personalizado

Si los roles predeterminados no se ajustan a sus necesidades, puede crear roles personalizados:

1. Navegue a **Configuración → Roles**.
2. Haga clic en **"Nuevo Rol"**.
3. Ingrese el nombre del rol (ej. `Almacenista`).
4. Seleccione los **permisos** que tendrá este rol (ver sección 7 para la lista completa).
5. Haga clic en **"Guardar Rol"**.
6. Asigne el nuevo rol a los usuarios correspondientes.

### 3.3 Editar Permisos de un Rol

1. Navegue a **Configuración → Roles**.
2. Haga clic en el rol que desea modificar.
3. Active o desactive los permisos necesarios.
4. Haga clic en **"Guardar"**.

> **Nota:** Los cambios de permisos se aplican la próxima vez que el usuario inicie sesión (el token JWT actual no se invalida automáticamente).

---

## 4. Configuración de Precios

**Ruta:** `/settings/pricing`

Corelio permite definir múltiples niveles de precio para adaptarse a diferentes tipos de clientes o políticas comerciales.

### 4.1 Niveles de Precio

Existen dos tipos de niveles:

**Niveles de Descuento** — aplican un porcentaje de descuento sobre el precio de venta estándar:
- Ej: Descuento 1 = 5%, Descuento 2 = 10%, Descuento 3 = 15%

**Niveles de Margen** — calculan el precio a partir del costo + un margen de ganancia:
- Ej: Menudeo = 40%, Mayoreo = 25%, Distribuidor = 15%

### 4.2 Configurar Niveles

1. Navegue a **Configuración → Precios**.
2. En cada nivel puede:
   - **Renombrar** el nivel (campo de texto editable)
   - **Cambiar el porcentaje** de descuento o margen
   - **Agregar un nuevo nivel** (botón `+`)
   - **Eliminar un nivel** (solo si no está asignado a ningún producto)
3. Haga clic en **"Guardar Configuración"** al terminar.

### 4.3 Asignar Precios por Nivel a Productos

Después de configurar los niveles, asigne los precios individuales en cada producto:

1. Navegue a **Catálogo → Productos**.
2. Abra el producto y vaya a la pestaña **"Precios"**.
3. Para cada nivel, ingrese el margen o precio directo.
4. Guarde los precios.

Alternativamente, use la **actualización masiva** para aplicar el mismo margen a múltiples productos:

1. Navegue a **Configuración → Precios → Actualización Masiva**.
2. Seleccione los productos por categoría o individualmente.
3. Ingrese el porcentaje de ajuste y el nivel de precio a actualizar.
4. Haga clic en **"Aplicar"**.

> **Efecto en POS:** En el módulo POS, el cajero puede seleccionar el nivel de precio del cliente antes de cerrar la venta. Los precios se ajustan automáticamente.

---

## 5. Configuración de Facturación CFDI

**Ruta:** `/settings/cfdi`

Para emitir facturas electrónicas CFDI 4.0 válidas ante el SAT, debe configurar los datos del emisor y cargar el Certificado de Sello Digital (CSD).

> Para instrucciones detalladas sobre cómo obtener y cargar el CSD, consulte la [Guía de Certificado CSD](cfdi-certificate-guide.md).

### 5.1 Datos del Emisor

Complete todos los campos antes de emitir facturas:

| Campo | Descripción | Ejemplo |
|-------|-------------|---------|
| RFC del emisor | RFC de la empresa (13 caracteres) | `FLO850101ABC` |
| Razón social | Nombre fiscal exacto como aparece en el SAT | `FERRETERÍA LÓPEZ S.A. DE C.V.` |
| Régimen fiscal | Clave del régimen (catálogo SAT) | `601` (General de Ley) |
| Código postal | Código postal del lugar de expedición | `06600` |
| Serie de folios | Letra o número para identificar la serie | `A` |
| Folio inicial | Número a partir del cual iniciar | `1` |

**Regímenes fiscales comunes:**

| Clave | Descripción |
|-------|-------------|
| 601 | General de Ley Personas Morales |
| 612 | Personas Físicas con Actividades Empresariales |
| 621 | Incorporación Fiscal |
| 626 | Simplificado de Confianza (RESICO) |

### 5.2 Cargar el Certificado CSD

1. En la sección **"Certificado CSD"** de la página de configuración CFDI, haga clic en **"Seleccionar Archivo"**.
2. Seleccione el archivo `.pfx` de su CSD.
3. Ingrese la **contraseña** del certificado en el campo correspondiente.
4. Haga clic en **"Cargar Certificado"**.
5. Si es válido, verá:
   - RFC del certificado (debe coincidir con el RFC del emisor)
   - Fecha de vigencia (los CSD tienen vigencia de 4 años)
   - Estado: **"Certificado válido ✅"**

> **Seguridad:** El certificado se almacena de forma cifrada. La contraseña nunca se guarda en texto plano.

### 5.3 Verificar la Configuración

Tras guardar, genere una factura de prueba:

1. Cree una venta de prueba en el POS con un cliente que tenga RFC.
2. Genere la factura CFDI desde **Facturación → Facturas**.
3. Intente timbrarla con el PAC.
4. Si el timbrado es exitoso, la configuración está correcta.

---

## 6. Seguridad y Gestión de Sesiones

### 6.1 Política de Contraseñas

Las contraseñas deben cumplir:
- Mínimo **8 caracteres**
- Al menos **1 letra mayúscula**
- Al menos **1 número**
- Se recomienda incluir caracteres especiales (`!@#$%`)

### 6.2 Duración de Sesión

- Los tokens de acceso (JWT) tienen una vigencia de **8 horas**.
- Los tokens de refresco tienen una vigencia de **30 días**.
- Al cerrar sesión, el token de refresco se invalida inmediatamente.

### 6.3 Seguridad Multi-Tenant

- Cada tenant opera en un entorno completamente aislado a nivel de base de datos.
- No es posible acceder a datos de otro tenant, incluso con las mismas credenciales.
- Todos los registros incluyen auditoría: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`.

### 6.4 Recomendaciones de Seguridad

- **No comparta credenciales** — cada empleado debe tener su propio usuario.
- **Desactive usuarios inmediatamente** cuando un empleado deje la empresa.
- **Use contraseñas únicas** para Corelio (no reutilice contraseñas de otros sistemas).
- **Revise el historial de transacciones** regularmente para detectar movimientos inusuales.
- **Renueve el CSD** con anticipación antes de que expire (fecha visible en `/settings/cfdi`).

---

## 7. Referencia de Permisos

Tabla completa de permisos disponibles para asignar a roles personalizados:

| Módulo | Permiso | Descripción |
|--------|---------|-------------|
| **Productos** | `products.view` | Ver catálogo de productos |
| | `products.create` | Crear nuevos productos |
| | `products.update` | Editar productos existentes |
| | `products.delete` | Desactivar productos |
| **Clientes** | `customers.view` | Ver lista de clientes |
| | `customers.create` | Crear nuevos clientes |
| | `customers.update` | Editar clientes existentes |
| | `customers.delete` | Desactivar clientes |
| **Ventas** | `sales.view` | Ver historial de ventas |
| | `sales.create` | Crear ventas (POS) |
| | `sales.cancel` | Cancelar ventas |
| **Inventario** | `inventory.view` | Ver inventario actual |
| | `inventory.adjust` | Realizar ajustes de stock |
| **Facturación** | `cfdi.view` | Ver facturas CFDI |
| | `cfdi.generate` | Generar y timbrar facturas |
| | `cfdi.cancel` | Cancelar facturas timbradas |
| **Precios** | `pricing.view` | Ver configuración de precios |
| | `pricing.manage` | Modificar precios y niveles |
| **Configuración** | `settings.view` | Ver configuración del tenant |
| | `settings.update` | Modificar configuración general |
| | `settings.cfdi` | Gestionar configuración CFDI y CSD |
| **Usuarios** | `users.view` | Ver lista de usuarios |
| | `users.create` | Crear usuarios |
| | `users.update` | Editar usuarios |
| | `users.delete` | Desactivar usuarios |
| **Reportes** | `reports.view` | Ver reportes y estadísticas |
| | `reports.export` | Exportar reportes |
| **Roles** | `roles.view` | Ver roles del tenant |
| | `roles.create` | Crear roles personalizados |
| | `roles.update` | Editar roles |
| | `roles.delete` | Eliminar roles (solo roles sin usuarios) |

---

## Soporte Técnico

Para problemas de configuración avanzada o incidentes críticos:

- **Correo:** soporte@corelio.com.mx
- **Teléfono:** +52 (55) 0000-0000
- **Horario:** Lunes a Viernes, 9:00 am — 6:00 pm (hora CDMX)
- **Portal de soporte:** (disponible próximamente)

---

*Corelio ERP — Guía de Administrador*  
*Conforme a CFDI 4.0 (SAT México)*
