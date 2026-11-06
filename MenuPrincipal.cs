using System;
using System.Collections.Generic;

namespace ConcesionarioMotos;


public class MenuPrincipal
{
    public void Ejecutar(List<Moto> motos, List<Vendedor> vendedores)
    {
        Almacen almacen = new Almacen();
        Vendedores moduloVendedores = new Vendedores();

        int opcion = 0;

        do
        {
            LimpiarPantalla();
            Console.WriteLine("==========================================");
            Console.WriteLine("       SISTEMA DE MOTOS - CONCESIONARIO");
            Console.WriteLine("==========================================");
            Console.WriteLine();
            Console.WriteLine("1. Modulo Almacen");
            Console.WriteLine("2. Modulo Vendedores");
            Console.WriteLine("3. Salir del sistema");
            Console.WriteLine();

            opcion = LeerEntero("Seleccione una opcion: ", 1, 3);

            switch (opcion)
            {
                case 1:
                    almacen.Ejecutar(motos, vendedores);
                    break;

                case 2:
                    moduloVendedores.Ejecutar(motos, vendedores);
                    break;

                case 3:
                    Console.WriteLine();
                    Console.WriteLine("Saliendo del sistema...");
                    Console.WriteLine("Gracias por utilizar el concesionario.");
                    break;
            }

        } while (opcion != 3);
    }

    
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
                Console.WriteLine("Seleccione una opcion entre " + minimo + " y " + maximo + ".");
                valido = false;
            }

        } while (!valido);

        return numero;
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


