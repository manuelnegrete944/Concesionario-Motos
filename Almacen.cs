using System;
using System.Collections.Generic;

namespace ConcesionarioMotos;


public class Almacen
{
    
    public void Ejecutar(List<Moto> motos, List<Vendedor> vendedores)
    {
        int opcion = 0;

        do
        {
            LimpiarPantalla();
            Console.WriteLine("==========================================");
            Console.WriteLine("             MODULO ALMACEN");
            Console.WriteLine("==========================================");
            Console.WriteLine("1. Consultar inventario");
            Console.WriteLine("2. Ingresar nuevo lote");
            Console.WriteLine("3. Reporte individual de vendedor");
            Console.WriteLine("4. Reporte general consolidado");
            Console.WriteLine("5. Regresar al menu principal");
            Console.WriteLine("------------------------------------------");

            opcion = LeerEntero("Seleccione una opcion: ", 1, 5);

            switch (opcion)
            {
                case 1:
                    MostrarInventario(motos);
                    Pausar();
                    break;

                case 2:
                    IngresarNuevoLote(motos);
                    Pausar();
                    break;

                case 3:
                    MostrarReporteIndividual(motos, vendedores);
                    Pausar();
                    break;

                case 4:
                    MostrarReporteGeneral(vendedores);
                    Pausar();
                    break;

                case 5:
                    Console.WriteLine("Regresando al menu principal...");
                    break;
            }

        } while (opcion != 5);
    }

    
    public void MostrarInventario(List<Moto> motos)
    {
        LimpiarPantalla();
        Console.WriteLine("==============================================================");
        Console.WriteLine("                    INVENTARIO DE MOTOS");
        Console.WriteLine("==============================================================");
        ImprimirInventario(motos);
    }

    
    private void ImprimirInventario(List<Moto> motos)
    {
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

    
    public void IngresarNuevoLote(List<Moto> motos)
    {
        LimpiarPantalla();
        Console.WriteLine("==========================================");
        Console.WriteLine("             INGRESAR NUEVO LOTE");
        Console.WriteLine("==========================================");

        ImprimirInventario(motos);

        int idMoto = LeerEntero("Ingrese el ID de la motocicleta: ", 0, int.MaxValue);
        Moto? motoSeleccionada = BuscarMoto(motos, idMoto);

        if (motoSeleccionada == null)
        {
            Console.WriteLine("No existe una motocicleta con ese ID.");
            return;
        }

        int cantidad = LeerEntero("Ingrese la cantidad que desea agregar: ", 1, int.MaxValue);

        try
        {
            motoSeleccionada.AgregarStock(cantidad);

            Console.WriteLine();
            Console.WriteLine("Lote ingresado correctamente.");
            Console.WriteLine("Motocicleta: " + motoSeleccionada.Modelo);
            Console.WriteLine("Unidades agregadas: " + cantidad);
            Console.WriteLine("Nuevo stock: " + motoSeleccionada.Cantidad);
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine("No fue posible ingresar el lote: " + ex.Message);
        }
    }

    
    public void MostrarReporteIndividual(List<Moto> motos, List<Vendedor> vendedores)
    {
        LimpiarPantalla();
        Console.WriteLine("==========================================");
        Console.WriteLine("          REPORTE INDIVIDUAL");
        Console.WriteLine("==========================================");

        Console.WriteLine("Escriba el codigo y presione ENTER para consultar sus ventas.");
        Console.Write("Codigos disponibles: ");
        foreach (Vendedor vendedor in vendedores)
            Console.Write(vendedor.Codigo + " ");
        Console.WriteLine();

        int codigo = LeerEntero("Ingrese el codigo del vendedor: ", 1, int.MaxValue);
        Vendedor? vendedorSeleccionado = BuscarVendedor(vendedores, codigo);

        if (vendedorSeleccionado == null)
        {
            Console.WriteLine("No existe un vendedor con ese codigo.");
            return;
        }

        Console.WriteLine();
        Console.WriteLine("Vendedor: " + vendedorSeleccionado.Codigo);
        if (vendedorSeleccionado.Ventas.Count == 0)
            Console.WriteLine("Este vendedor aun no tiene ventas. Se muestran todos los modelos en cero.");
        Console.WriteLine("------------------------------------------");
        Console.WriteLine("{0,-32} {1,10}", "MOTOCICLETA", "UNIDADES");
        Console.WriteLine("------------------------------------------");

        int totalUnidades = 0;
        decimal totalRecaudado = 0;

        foreach (Moto moto in motos)
        {
            int unidadesMoto = 0;

            foreach (Venta venta in vendedorSeleccionado.Ventas)
            {
                if (venta.IdMoto == moto.Id)
                {
                    unidadesMoto += venta.Cantidad;
                }
            }

            totalUnidades += unidadesMoto;
            Console.WriteLine("{0,-32} {1,10}", moto.Modelo, unidadesMoto);
        }

        foreach (Venta venta in vendedorSeleccionado.Ventas)
        {
            totalRecaudado += venta.Total;
        }

        Console.WriteLine("------------------------------------------");
        Console.WriteLine("Total de unidades vendidas: " + totalUnidades);
        Console.WriteLine("Total recaudado: $" + totalRecaudado.ToString("N0"));
        Console.WriteLine("==========================================");
    }

    
    public void MostrarReporteGeneral(List<Vendedor> vendedores)
    {
        LimpiarPantalla();
        Console.WriteLine("==============================================================");
        Console.WriteLine("                 RESUMEN GENERAL DE VENDEDORES");
        Console.WriteLine("==============================================================");
        Console.WriteLine("{0,-14} {1,18} {2,22}", "COD. VENDEDOR", "UNIDADES VENDIDAS", "TOTAL RECAUDADO");
        Console.WriteLine("--------------------------------------------------------------");

        int totalGeneralUnidades = 0;
        decimal totalGeneralRecaudado = 0;

        foreach (Vendedor vendedor in vendedores)
        {
            int unidadesVendedor = 0;
            decimal recaudadoVendedor = 0;

            foreach (Venta venta in vendedor.Ventas)
            {
                unidadesVendedor += venta.Cantidad;
                recaudadoVendedor += venta.Total;
            }

            totalGeneralUnidades += unidadesVendedor;
            totalGeneralRecaudado += recaudadoVendedor;

            Console.WriteLine(
                "{0,-14} {1,18} ${2,21:N0}",
                vendedor.Codigo,
                unidadesVendedor,
                recaudadoVendedor
            );
        }

        Console.WriteLine("--------------------------------------------------------------");
        Console.WriteLine(
            "{0,-14} {1,18} ${2,21:N0}",
            "TOTALES",
            totalGeneralUnidades,
            totalGeneralRecaudado
        );
        Console.WriteLine("==============================================================");
    }

    
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

    // Lee y valida un número entero.
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

    
    private void PausarReporte()
    {
        string? respuesta;
        do
        {
            Console.WriteLine();
            Console.WriteLine("Reporte finalizado. Escriba VOLVER y presione ENTER para regresar al almacen.");
            respuesta = Console.ReadLine();
            if (respuesta == null)
                throw new System.IO.EndOfStreamException("Se cerro la entrada; el reporte permanece visible.");
        } while (!string.Equals(respuesta.Trim(), "VOLVER", StringComparison.OrdinalIgnoreCase));
    }

    
    private void Pausar()
    {
        Console.WriteLine();
        Console.WriteLine("Presione ENTER para continuar...");
        Console.ReadLine();
    }

    
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
            
            Console.WriteLine();
        }
    }
}



