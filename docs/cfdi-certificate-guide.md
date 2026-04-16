# Guía de Configuración del Certificado CSD para CFDI 4.0

**Versión:** Sprint 10  
**Regulación:** SAT México — CFDI 4.0  
**Última actualización:** 2026-04-16

> El **Certificado de Sello Digital (CSD)** es un archivo emitido por el SAT que permite a su empresa firmar digitalmente las facturas electrónicas CFDI. Sin este certificado, no es posible generar facturas válidas.

---

## Tabla de Contenidos

1. [¿Qué es el CSD?](#1-qué-es-el-csd)
2. [Pre-requisitos](#2-pre-requisitos)
3. [Obtención del CSD desde el Portal SAT](#3-obtención-del-csd-desde-el-portal-sat)
4. [Conversión a formato PFX](#4-conversión-a-formato-pfx)
5. [Configuración en Corelio](#5-configuración-en-corelio)
6. [Verificación](#6-verificación)
7. [Renovación del CSD](#7-renovación-del-csd)
8. [Resolución de Problemas](#8-resolución-de-problemas)

---

## 1. ¿Qué es el CSD?

El **Certificado de Sello Digital (CSD)** es un par de archivos emitidos por el SAT a cada contribuyente para firmar electrónicamente sus comprobantes fiscales (CFDI):

| Archivo | Extensión | Descripción |
|---------|-----------|-------------|
| Certificado público | `.cer` | Contiene el número de certificado y la clave pública. Se incluye en cada XML |
| Clave privada | `.key` | Clave privada para firmar el XML. Protegida con contraseña |

**Características del CSD:**
- Vigencia: **4 años** a partir de su emisión
- Es **único por RFC** — una empresa con varios establecimientos usa el mismo CSD
- Se debe **renovar antes de que expire** para no interrumpir la facturación
- Es **diferente** de la e.firma (FIEL) — no los confunda

**Corelio requiere el CSD en formato `.pfx`** (un solo archivo que contiene el certificado y la clave privada, protegido por contraseña). Si tiene los archivos `.cer` y `.key` por separado, siga la sección 4 para convertirlos.

---

## 2. Pre-requisitos

Antes de obtener su CSD, necesita:

### e.firma (FIEL) vigente
La e.firma es el certificado de identidad digital del SAT. Sin ella no puede acceder al portal para obtener el CSD.

- **¿Cómo obtenerla?** Agende una cita en su módulo del SAT más cercano con:
  - Identificación oficial (INE)
  - Correo electrónico activo
  - USB para guardar los archivos
- **Vigencia:** 4 años

### Acceso al portal del SAT
- Correo electrónico registrado ante el SAT
- Contraseña del SAT o Contraseña Clave Privada de la e.firma

---

## 3. Obtención del CSD desde el Portal SAT

### 3.1 Acceder al Portal CertiSAT Web

1. Abra su navegador y navegue a:  
   **`https://cfdiau.sat.gob.mx/nidp/wsfed/ep`**

2. Haga clic en **"Personas Morales"** (si es empresa) o **"Personas Físicas"** (si es persona física con actividad empresarial).

3. En el menú de acceso, seleccione una de las opciones:
   - **Con e.firma:** Use sus archivos `.cer` y `.key` de la e.firma + contraseña
   - **Con Contraseña SAT:** Ingrese RFC + Contraseña SAT

4. Ingrese sus credenciales y haga clic en **"Enviar"**.

### 3.2 Generar el CSD

Una vez dentro del portal:

1. En el menú principal, haga clic en **"Solicitar CSD"** o busque la sección **"Generación de Sellos Digitales"**.

2. Haga clic en **"Nueva Solicitud"**.

3. El portal le pedirá que genere un par de claves. Existen dos métodos:
   - **Descarga del programa (recomendado para empresas):** Descargue la herramienta SOLCEDI o CERTIFICA del SAT, genere la solicitud localmente y cargue el archivo `.req` al portal.
   - **Generación en línea:** El portal genera el par de claves directamente en el navegador.

4. Si usa la herramienta local:
   - Abra CERTIFICA (disponible en el portal del SAT).
   - Seleccione **"Requerimiento de Sellos Digitales"**.
   - Ingrese: RFC, nombre o razón social, contraseña para la clave privada (guárdela, la necesitará siempre).
   - Haga clic en **"Generar"**. Se crean dos archivos: `.req` (requerimiento) y `.key` (clave privada).
   - Cargue el archivo `.req` al portal del SAT.

5. El SAT procesa la solicitud y le permite **descargar el certificado** en formato `.cer`.

6. Descargue el archivo `.cer` y guárdelo junto con su archivo `.key` en un lugar seguro.

> **Importante:** La contraseña de la clave privada **no puede recuperarse**. Si la pierde, deberá revocar el CSD y solicitar uno nuevo.

### 3.3 Verificar el Certificado Descargado

Para confirmar que el certificado descargado corresponde a su RFC:

1. Abra el portal del SAT y busque **"Consultar CSD Vigentes"**.
2. Verifique que el número de certificado del `.cer` descargado aparezca como **"Vigente"**.

---

## 4. Conversión a Formato PFX

Corelio requiere el CSD en formato `.pfx` (también conocido como PKCS#12), que combina el certificado y la clave privada en un solo archivo protegido por contraseña.

### Método: OpenSSL (recomendado)

OpenSSL es la herramienta estándar para la conversión. Está disponible en:
- **Linux/Mac:** Ya viene instalado en la mayoría de distribuciones
- **Windows:** Descargue desde [slproweb.com/products/Win32OpenSSL.html](https://slproweb.com/products/Win32OpenSSL.html) o use WSL

**Pasos:**

1. Abra una terminal (cmd, PowerShell, o Terminal de Mac/Linux).

2. Navegue a la carpeta donde tiene sus archivos `.cer` y `.key`:
   ```bash
   cd /ruta/a/mis/archivos-csd
   ```

3. Ejecute el siguiente comando (sustituyendo los nombres de archivo):
   ```bash
   openssl pkcs12 -export \
     -in certificado.cer \
     -inkey clave_privada.key \
     -out mi_empresa_csd.pfx \
     -password pass:MiContraseñaSegura123
   ```

   | Parámetro | Descripción |
   |-----------|-------------|
   | `-in certificado.cer` | Su archivo .cer descargado del SAT |
   | `-inkey clave_privada.key` | Su archivo .key (clave privada) |
   | `-out mi_empresa_csd.pfx` | Nombre del archivo .pfx de salida |
   | `-password pass:...` | Contraseña para proteger el .pfx (puede ser diferente a la del .key) |

4. Se le pedirá la **contraseña de la clave privada** (la que ingresó al generar el CSD con CERTIFICA). Ingrésela cuando se solicite.

5. El archivo `.pfx` se creará en la misma carpeta. Guárdelo en un lugar seguro.

> **Seguridad:** Almacene el `.pfx` y su contraseña en un administrador de contraseñas o bóveda digital. No los envíe por correo electrónico sin cifrar.

---

## 5. Configuración en Corelio

### 5.1 Acceder a la Configuración CFDI

1. Inicie sesión en Corelio con una cuenta de **Administrador**.
2. En el menú lateral, navegue a **Configuración → Facturación CFDI** (`/settings/cfdi`).

### 5.2 Completar los Datos del Emisor

Complete todos los campos del formulario de configuración:

| Campo | Descripción | Ejemplo |
|-------|-------------|---------|
| RFC del emisor | RFC exacto de su empresa (13 dígitos para personas morales, 13 para físicas) | `FLO850101ABC` |
| Razón social | Nombre fiscal **en mayúsculas**, exactamente como aparece en el SAT | `FERRETERÍA LÓPEZ S.A. DE C.V.` |
| Régimen fiscal | Clave del régimen (ver tabla abajo) | `601` |
| Código postal | CP del lugar de expedición (su establecimiento principal) | `06600` |
| Serie de folios | Una letra o número para identificar la serie | `A` |
| Folio inicial | Número desde el que comenzará la numeración | `1` |

**Regímenes fiscales más comunes:**

| Clave | Régimen |
|-------|---------|
| 601 | General de Ley Personas Morales |
| 612 | Personas Físicas con Actividades Empresariales y Profesionales |
| 621 | Incorporación Fiscal |
| 626 | Simplificado de Confianza (RESICO) |
| 625 | Régimen de las Actividades Empresariales con ingresos a través de Plataformas Tecnológicas |

> **Nota:** El régimen fiscal debe coincidir exactamente con el registrado ante el SAT para su RFC. Una discrepancia causará errores de validación en el PAC.

### 5.3 Cargar el Certificado CSD

1. Desplácese hacia la sección **"Certificado CSD"** en la página de configuración.
2. Haga clic en **"Seleccionar Archivo"** y elija el archivo `.pfx` generado en la sección 4.
3. En el campo **"Contraseña del Certificado"**, ingrese la contraseña del `.pfx`.
4. Haga clic en **"Cargar Certificado"**.

**Resultado exitoso:**
```
✅ Certificado válido
   RFC: FLO850101ABC
   Vigente hasta: 15/03/2028
   Número de certificado: 00001000000504465028
```

Si aparece un error, consulte la sección 8 (Resolución de Problemas).

### 5.4 Guardar la Configuración

1. Haga clic en **"Guardar Configuración"** (botón azul al final del formulario).
2. Aparecerá el mensaje: _"Configuración guardada correctamente"_.

---

## 6. Verificación

Después de configurar el CSD, realice una prueba de extremo a extremo:

### 6.1 Prueba de Generación y Timbrado

1. Cree una venta de prueba en el POS con un cliente que tenga RFC válido (ej. `XAXX010101000` para cliente genérico, o un cliente real con RFC).
2. Navegue a **Facturación → Facturas** (`/facturas`).
3. Haga clic en **"Nueva Factura"** y seleccione la venta de prueba.
4. Haga clic en **"Generar Factura"** — se crea en estado Borrador.
5. Abra el borrador y haga clic en **"Timbrar con PAC"**.
6. Si el proceso es exitoso:
   - La factura cambia a estado **"Timbrada"**
   - Se muestra un UUID (ej. `6B7A2C3D-E4F5-6789-ABCD-EF0123456789`)
7. Descargue el **XML** y el **PDF** para confirmar que son correctos.

### 6.2 Verificar la Factura en el Portal SAT

Para confirmar que la factura fue reconocida por el SAT:

1. Navegue al portal de verificación del SAT:  
   **`https://verificacfdi.facturaelectronica.sat.gob.mx`**
2. Ingrese el UUID (Folio Fiscal) de la factura timbrada.
3. Ingrese el RFC del emisor y del receptor.
4. El portal confirmará si la factura es válida.

---

## 7. Renovación del CSD

Los CSD tienen una vigencia de **4 años**. Se recomienda renovarlos **30 días antes de que expiren** para evitar interrupciones en la facturación.

### 7.1 Cómo Saber Cuándo Expira

- La fecha de vigencia se muestra en la página **Configuración → Facturación CFDI** bajo el resumen del certificado cargado.
- También puede consultarla en el portal del SAT en **"Consultar CSD Vigentes"**.

### 7.2 Proceso de Renovación

1. Siga los pasos de la [sección 3](#3-obtención-del-csd-desde-el-portal-sat) para solicitar un nuevo CSD al SAT.
2. Convierta el nuevo CSD a `.pfx` siguiendo la [sección 4](#4-conversión-a-formato-pfx).
3. En Corelio, vaya a **Configuración → Facturación CFDI**.
4. Cargue el nuevo certificado `.pfx` (sobreescribe el anterior).
5. Las facturas emitidas con el CSD anterior **siguen siendo válidas** — el SAT conserva los registros históricos.

> **Importante:** Si el CSD expira antes de renovarlo, Corelio no podrá timbrar nuevas facturas hasta que cargue el nuevo certificado. Las facturas ya timbradas no se ven afectadas.

---

## 8. Resolución de Problemas

### Error: "Contraseña incorrecta del certificado"

**Causa:** La contraseña del `.pfx` ingresada en Corelio no coincide con la que se usó al crear el `.pfx`.

**Solución:**
1. Confirme la contraseña que ingresó al ejecutar el comando `openssl pkcs12 -export ... -password pass:...`.
2. Vuelva a ingresar la contraseña en el campo **"Contraseña del Certificado"** en Corelio.
3. Si ya no recuerda la contraseña, regenere el `.pfx` con una nueva contraseña usando los archivos `.cer` y `.key` originales.

---

### Error: "El RFC del certificado no coincide con el RFC del emisor"

**Causa:** El archivo `.pfx` cargado corresponde a un RFC diferente al configurado en el campo "RFC del emisor".

**Solución:**
1. Verifique el RFC del certificado — se muestra en el mensaje de error.
2. Corrija el campo **"RFC del emisor"** en el formulario para que coincida.
3. O cargue el certificado `.pfx` correcto para su empresa.

---

### Error: "Certificado vencido"

**Causa:** El CSD ya expiró (vigencia de 4 años).

**Solución:**
1. Solicite un nuevo CSD al SAT (ver [sección 3](#3-obtención-del-csd-desde-el-portal-sat)).
2. Conviértalo a `.pfx` (ver [sección 4](#4-conversión-a-formato-pfx)).
3. Cargue el nuevo certificado en Corelio (ver [sección 5.3](#53-cargar-el-certificado-csd)).

---

### Error: "Error al timbrar — XML inválido"

**Causa:** Los datos del emisor (razón social, régimen fiscal) no coinciden con los registros del SAT.

**Solución:**
1. Acceda al portal del SAT y consulte la razón social **exacta** registrada para su RFC.
2. Actualice el campo **"Razón social"** en Corelio (mayúsculas, acentos, puntuación exacta).
3. Verifique que el **régimen fiscal** coincide con el registrado en el SAT.
4. Guarde y vuelva a intentar el timbrado.

---

### Error: "El archivo .cer y .key no corresponden al mismo certificado"

**Causa:** Al crear el `.pfx`, se usaron archivos que no son par (`.cer` de un certificado y `.key` de otro).

**Solución:**
1. Descargue nuevamente el `.cer` del portal del SAT.
2. Use el `.key` que se generó en el mismo momento que ese `.cer`.
3. Asegúrese de que ambos archivos tienen el mismo número de certificado en el nombre.
4. Regenere el `.pfx` con el par correcto.

---

### No aparece la opción "Cargar Certificado" en la pantalla

**Causa:** Su cuenta no tiene el permiso `settings.cfdi`.

**Solución:** Contacte a su administrador para que asigne este permiso a su rol, o inicie sesión con una cuenta Administrador.

---

## Recursos Adicionales

- **Portal CertiSAT Web:** `https://cfdiau.sat.gob.mx`
- **Verificación de CFDI:** `https://verificacfdi.facturaelectronica.sat.gob.mx`
- **Catálogo SAT de Regímenes Fiscales:** `http://www.sat.gob.mx/sitio_internet/cfd/4/catCFDI.xsd`
- **Soporte SAT:** MarcaSAT 55 627-22-728 (horario: Lunes-Viernes 8:00-20:30, Sábado 9:00-13:00)
- **Guía de Administrador de Corelio:** [admin-guide.md](admin-guide.md)

---

*Corelio ERP — Guía de Certificado CSD*  
*Conforme a CFDI 4.0 — SAT México*
