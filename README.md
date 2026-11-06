# ConcesionarioMotos – Etapa 5.3

## Objetivo
Integrar el módulo de vendedores y ventas con el módulo Almacén de la etapa 5.2, manteniendo una estructura sencilla basada en clases y métodos.

## Estructura del proyecto
- `Program.cs`: crea los datos iniciales y llama al menú principal.
- `Modelos.cs`: contiene Moto, Vendedor, Venta y MetodoPago.
- `DatosIniciales.cs`: crea 15 motocicletas y 5 vendedores.
- `MenuPrincipal.cs`: conecta los módulos Almacén y Vendedores.
- `Almacen.cs`: consulta inventario, ingresa lotes y presenta reportes.
- `Vendedores.cs`: valida vendedores, registra ventas y muestra facturas.
- `ConcesionarioMotos.csproj`: configuración del proyecto .NET 8.

No se utiliza Gotoxy, Console.SetCursorPosition, ConsolaUI, ConcesionarioApp ni servicios independientes.

## Reglas de venta
- Solo se permite vender si el vendedor existe.
- La motocicleta debe existir y tener stock suficiente.
- La cantidad debe ser un entero mayor que cero.
- Efectivo: descuento del 2% sobre el subtotal.
- Tarjeta de crédito: no aplica descuento.
- La venta queda guardada en el historial del vendedor.
- El inventario disminuye después de registrar la venta.

## Compilar y ejecutar
Desde la carpeta que contiene `ConcesionarioMotos.csproj`:

```powershell
dotnet clean
dotnet restore
dotnet build
dotnet run
```

## Pruebas sugeridas
1. Abrir el módulo Almacén y consultar las 15 motocicletas.
2. Ingresar 2 unidades al ID 0 (Yamaha MT09): el stock debe pasar de 3 a 5.
3. Regresar al menú principal y abrir Vendedores.
4. Registrar una venta para vendedor 101, moto ID 0, cantidad 1, pago en efectivo:
   - Subtotal: $68.000.000
   - Descuento: $1.360.000
   - Total: $66.640.000
   - Stock: 4 si se ingresó el lote de 2 unidades antes de la venta; de lo contrario, 2.
5. Registrar otra venta para vendedor 101, moto ID 0, cantidad 1, pago con tarjeta:
   - Subtotal: $68.000.000
   - Descuento: $0
   - Total: $68.000.000
6. Consultar el reporte individual del vendedor 101: debe mostrar 2 unidades y $134.640.000 si se realizaron ambas ventas indicadas.
7. Consultar el reporte general: debe consolidar las ventas de los cinco vendedores.
8. Intentar vender una cantidad superior al stock disponible: la venta debe rechazarse sin modificar el inventario.
9. Ingresar un código de vendedor inexistente: el sistema debe rechazar el acceso a la operación.

## Alcance
Los datos se mantienen en memoria durante la ejecución. Al cerrar el programa, el inventario y las ventas vuelven a los valores iniciales. No se ha incorporado persistencia en archivos o base de datos.

## Correccion del reporte individual (5 de noviembre de 2026)
Se conservan exactamente los seis archivos C# y las listas compartidas. No se agregan servicios ni clases de interfaz.

### Diagnostico del original
El flujo es MenuPrincipal -> Almacen.Ejecutar -> opcion 3 -> MostrarReporteIndividual -> LeerEntero -> buscar vendedor -> imprimir modelos y totales -> Pausar -> menu.
El metodo original escribe encabezado y prompt inmediatamente despues de Console.Clear. No hay una rama normal que deje la pantalla vacia esperando el codigo sin imprimir antes. Los calculos del reporte ya eran correctos y no reinicializaban las ventas.

Si se envia un ENTER adicional despues del codigo (por doble pulsacion o texto pegado), la pausa original puede consumir esa linea vacia, regresar al menu y ejecutar Console.Clear, borrando el reporte. Esto explica un reporte que desaparece, pero no demuestra por si solo una pantalla permanentemente vacia: el menu deberia imprimirse despues. Una pantalla totalmente vacia requiere comprobar la terminal utilizada, la version ejecutada y la secuencia exacta. No se afirma haber reproducido ese sintoma visual.

Otro defecto confirmado por lectura: Console.ReadLine() devolviendo null se convertia en cadena vacia y el lector entraba en un ciclo indefinido de errores. Ahora se informa el cierre de entrada y se termina ordenadamente.

### Cambios
- Cada menu y operacion limpia la pantalla al entrar y dibuja inmediatamente su contenido. Nunca se limpia entre el resultado y su pausa. Se mantiene una sola ejecucion con las mismas listas de datos.
- Se muestran codigos disponibles e instrucciones antes de leer el codigo.
- Se muestran los 15 modelos incluso cuando tienen cero unidades; se explica si el vendedor no tiene ventas.
- La opcion individual pide escribir VOLVER y ENTER. Un ENTER adicional no cierra el reporte. La pausa pertenece al menu y se ejecuta tambien cuando el vendedor no existe.
- Se mantiene Console.ReadLine en todo el flujo; no se mezcla con ReadKey.
- Las consultas no modifican inventario ni ventas. Los datos siguen siendo solo de la sesion.

### Prueba manual del reporte
Ejecutar en una terminal interactiva (PowerShell, Windows Terminal o la consola de Visual Studio), no en el panel Salida.
1. Ejecutar dotnet run en esta carpeta y elegir 1 (Almacen), 3 (reporte).
2. Deben verse encabezado, codigos y prompt. Ingresar 101 y ENTER.
3. Sin ventas deben verse los 15 modelos en cero, vendedor 101 y ambos totales en cero.
4. Pulsar ENTER otra vez: el reporte sigue visible y se repite la instruccion VOLVER.
5. Escribir VOLVER y ENTER. Consultar 999: se muestra el rechazo y la misma pausa.
6. Consultar de nuevo e ingresar abc o una linea vacia: se informa el error y se vuelve a pedir el codigo.
7. Para las dos ventas del README, no agregar lote: menu principal 2; registrar venta 1; vendedor 101; moto 0; cantidad 1; pago 1; ENTER para continuar. Repetir con pago 2.
8. Volver con opcion 2, abrir Almacen con 1 y reporte con 3. Para 101: Yamaha MT09 = 2, otros modelos = 0, total unidades = 2, recaudado = $134.640.000 (los separadores dependen de la configuracion regional).
9. Escribir VOLVER y repetir la consulta: mismos valores. Consultar 102: cero ventas. Consultar reporte general: 2 unidades y el mismo recaudo. Inventario MT09: 1.
10. No cerrar el programa entre ventas y reportes: al reiniciar se cargan datos iniciales y se pierden las ventas de la sesion.

### Validacion realizada
dotnet build: correcto, cero advertencias y cero errores; destino net8.0.
Prueba automatizada por entrada de texto: dos ventas, descuento, reportes repetidos, vendedor sin ventas, codigo inexistente, entradas vacias/no numericas, ENTER adicional en pausa, consolidado y stock final. Todas las comprobaciones pasaron.
Tambien se probo fin de entrada esperando el codigo: muestra explicacion y termina sin ciclo infinito.
Estas pruebas verifican el programa con entrada redirigida; no se ha observado visualmente la terminal concreta donde se reporto el problema.

## Actualizacion: una sola pantalla por operacion
En terminal interactiva, LimpiarPantalla usa Console.Clear al abrir cada vista. Los resultados permanecen hasta la pausa: ENTER en inventario, lotes y ventas; VOLVER seguido de ENTER en reporte individual. Despues se limpia y aparece el menu correspondiente.
No se crean procesos ni ventanas por operacion. Limpiar la pantalla no elimina ventas ni inventario: solo cambia la vista.
La acumulacion de texto no altera los calculos; esta mejora evita confusion visual.
Si la salida esta redirigida (por ejemplo a un archivo o en las pruebas automaticas), no se intenta limpiar. Si la terminal rechaza Console.Clear con un error de entrada/salida, se continua mostrando contenido sin interrumpir la operacion.
Usar PowerShell/Windows Terminal con dotnet run, o Visual Studio con Ctrl+F5, no el panel Salida.

Prueba visual adicional:
1. Abrir Almacen: debe desaparecer el menu principal.
2. Abrir reporte individual: debe desaparecer el menu Almacen y aparecer inmediatamente titulo, codigos y prompt.
3. Ingresar 101: el reporte debe permanecer; un ENTER adicional no lo cierra.
4. Escribir VOLVER: debe desaparecer el reporte y aparecer Almacen.
5. Registrar una venta y volver a consultar: los datos deben conservarse.
Compilacion de esta actualizacion: correcta, cero errores y cero advertencias. Se repitio la prueba automatizada de ventas y reportes con entrada/salida redirigidas. La limpieza visual requiere la prueba manual anterior en la terminal del usuario.
