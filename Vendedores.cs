using System;
using System.Collections.Generic;

namespace ConcesionarioMotos;

// Este modulo valida vendedores y registra las ventas del concesionario.
public class Vendedores
{
    // Muestra las opciones del modulo de vendedores.
    public void Ejecutar(List<Moto> motos, List<Vendedor> vendedores)
    {
        int opcion = 0;

        do
        {
            LimpiarPantalla();
            Console.WriteLine("==========================================");
            Console.WriteLine("            MODULO VENDEDORES");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Registrar venta");
            Console.WriteLine("2. Regresar al menu principal");
            Console.WriteLine("------------------------------------------");

            opcion = LeerEntero("Seleccione una opcion: ", 1, 2);

            switch (opcion)
            {
                case 1:
                    RegistrarVenta(motos, vendedores);
                    Pausar();
                    break;

                case 2:
                    Console.WriteLine("Regresando al menu principal...");
                    break;
            }

        } while (opcion != 2);
    }

    // Valida el vendedor, la motocicleta, el stock y registra la venta.
    public void RegistrarVenta(List<Moto> motos, List<Vendedor> vendedores)
    {
        LimpiarPantalla();
        Console.WriteLine("==========================================");
        Console.WriteLine("              REGISTRAR VENTA");
        Console.WriteLine("==========================================");

        Console.WriteLine("VENDEDORES REGISTRADOS");
        foreach (Vendedor vendedor in vendedores)
        {
            Console.WriteLine("Codigo: " + vendedor.Codigo);
        }

        int codigoVendedor = LeerEntero("Ingrese el codigo del vendedor: ", 1, int.MaxValue);
        Vendedor? vendedorSeleccionado = BuscarVendedor(vendedores, codigoVendedor);

        if (vendedorSeleccionado == null)
        {
            Console.WriteLine("No existe un vendedor con ese codigo.");
            return;
        }

        Console.WriteLine();
        MostrarMotocicletas(motos);

        int idMoto = LeerEntero("Ingrese el ID de la motocicleta: ", 0, int.MaxValue);
        Moto? motoSeleccionada = BuscarMoto(motos, idMoto);

        if (motoSeleccionada == null)
        {
            Console.WriteLine("No existe una motocicleta con ese ID.");
            return;
        }

        int cantidad = LeerEntero("Ingrese la cantidad que desea comprar: ", 1, int.MaxValue);

        if (!motoSeleccionada.TieneStock(cantidad))
        {
            Console.WriteLine();
            Console.WriteLine("Venta rechazada. Stock insuficiente.");
            Console.WriteLine("Stock disponible: " + motoSeleccionada.Cantidad);
            return;
        }

        Console.WriteLine();
        Console.WriteLine("METODO DE PAGO");
        Console.WriteLine("1. Efectivo (2% de descuento)");
        Console.WriteLine("2. Tarjeta de credito (sin descuento)");

        int opcionPago = LeerEntero("Seleccione el metodo de pago: ", 1, 2);

        MetodoPago metodoPago;

        if (opcionPago == 1)
        {
            metodoPago = MetodoPago.Efectivo;
        }
        else
        {
            metodoPago = MetodoPago.TarjetaCredito;
        }

        // Calculamos los valores de la venta.
        decimal subtotal = motoSeleccionada.Precio * cantidad;
        decimal descuento = 0;

        if (metodoPago == MetodoPago.Efectivo)
        {
            descuento = subtotal * 0.02m;
        }

        decimal total = subtotal - descuento;

        // Creamos el objeto Venta con la informacion calculada.
        Venta nuevaVenta = new Venta(
            vendedorSeleccionado.Codigo,
            motoSeleccionada.Id,
            motoSeleccionada.Modelo,
            cantidad,
            motoSeleccionada.Precio,
            subtotal,
            descuento,
            total,
            metodoPago
        );

        try
        {
            // Actualizamos el inventario y guardamos la venta en el vendedor.
            motoSeleccionada.DescontarStock(cantidad);
            vendedorSeleccionado.RegistrarVenta(nuevaVenta);

            Console.WriteLine();
            Console.WriteLine("Venta registrada correctamente.");
            MostrarFactura(nuevaVenta);
            Console.WriteLine("Stock restante de " + motoSeleccionada.Modelo + ": " + motoSeleccionada.Cantidad);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("No fue posible registrar la venta: " + ex.Message);
        }
    }

    // Muestra las motocicletas disponibles para la venta.
    private void MostrarMotocicletas(List<Moto> motos)
    {
        Console.WriteLine("==============================================================");
        Console.WriteLine("                 MOTOCICLETAS DISPONIBLES");
        Console.WriteLine("==============================================================");
        Console.WriteLine("{0,-5} {1,-32} {2,8} {3,16}", "ID", "MODELO", "STOCK", "PRECIO");
        Console.WriteLine("--------------------------------------------------------------");

        foreach (Moto moto in motos)
        {
            Console.WriteLine(
                "{0,-5} {1,-32} {2,8} {3,16:N0}",
                moto.Id,
                moto.Modelo,
                moto.Cantidad,
                moto.Precio
            );
        }

        Console.WriteLine("==============================================================");
    }

    // Muestra la factura de una venta registrada.
    public void MostrarFactura(Venta venta)
    {
        string metodoPagoTexto;

        if (venta.MetodoPago == MetodoPago.Efectivo)
        {
            metodoPagoTexto = "Efectivo";
        }
        else
        {
            metodoPagoTexto = "Tarjeta de credito";
        }

        string descuentoTexto;

        if (venta.Descuento > 0)
        {
            descuentoTexto = "-$" + venta.Descuento.ToString("N0");
        }
        else
        {
            descuentoTexto = "$0";
        }

        Console.WriteLine();
        Console.WriteLine("==========================================");
        Console.WriteLine("               FACTURA DE VENTA");
        Console.WriteLine("==========================================");
        Console.WriteLine("Vendedor ID:       " + venta.CodigoVendedor);
        Console.WriteLine("Producto:          " + venta.ModeloMoto);
        Console.WriteLine("Cantidad:          " + venta.Cantidad);
        Console.WriteLine("Precio unitario:   $" + venta.PrecioUnitario.ToString("N0"));
        Console.WriteLine("Subtotal:          $" + venta.Subtotal.ToString("N0"));
        Console.WriteLine("Descuento:          " + descuentoTexto);
        Console.WriteLine("Metodo de pago:    " + metodoPagoTexto);
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("TOTAL A PAGAR:     $" + venta.Total.ToString("N0"));
        Console.WriteLine("Fecha:             " + venta.Fecha.ToString("dd/MM/yyyy HH:mm"));
        Console.WriteLine("==========================================");
    }

    // Busca un vendedor por su codigo.
    private Vendedor? BuscarVendedor(List<Vendedor> vendedores, int codigo)
    {
        foreach (Vendedor vendedor in vendedores)
        {
            if (vendedor.Codigo == codigo)
            {
                return vendedor;
            }
        }

        return null;
    }

    // Busca una motocicleta por su ID.
    private Moto? BuscarMoto(List<Moto> motos, int id)
    {
        foreach (Moto moto in motos)
        {
            if (moto.Id == id)
            {
                return moto;
            }
        }

        return null;
    }

    // Lee un entero y comprueba que se encuentre dentro del rango.
    private int LeerEntero(string mensaje, int minimo, int maximo)
    {
        int numero;
        bool valido;

        do
        {
            Console.Write(mensaje);
            string? entrada = Console.ReadLine();
            if (entrada == null)
                throw new System.IO.EndOfStreamException("Se cerro la entrada. Ejecute el programa en una terminal interactiva.");

            valido = int.TryParse(entrada, out numero);

            if (!valido)
            {
                Console.WriteLine("Debe ingresar un numero entero.");
            }
            else if (numero < minimo || numero > maximo)
            {
                Console.WriteLine("El valor debe estar entre " + minimo + " y " + maximo + ".");
                valido = false;
            }

        } while (!valido);

        return numero;
    }

    // Pausa la pantalla hasta que el usuario presione ENTER.
    private void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Presione ENTER para continuar...");
        Console.ReadLine();
    }

    // Limpia solo al entrar a una vista y solo en una terminal interactiva.
    private void LimpiarPantalla()
    {
        if (Console.IsOutputRedirected)
            return;

        try
        {
            Console.Clear();
        }
        catch (System.IO.IOException)
        {
            // Si la terminal no permite limpiar, conserva el flujo y sus mensajes.
            Console.WriteLine();
        }
    }
}


