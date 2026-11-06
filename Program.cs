using System;
using System.Collections.Generic;

namespace ConcesionarioMotos;

internal class Program
{
    private static void Main(string[] args)
    {
        // Creamos los datos iniciales del concesionario.
        DatosIniciales datos = new DatosIniciales();

        List<Moto> motos = datos.CrearMotos();
        List<Vendedor> vendedores = datos.CrearVendedores();

        // Iniciamos el menu principal y enviamos las listas.
        MenuPrincipal menu = new MenuPrincipal();
        try
        {
            menu.Ejecutar(motos, vendedores);
        }
        catch (System.IO.EndOfStreamException ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            Console.WriteLine("Fin del programa. Los datos solo se conservan mientras esta abierto.");
        }
    }
}

