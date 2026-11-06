using System.Collections.Generic;

namespace ConcesionarioMotos;


public class DatosIniciales
{
    public List<Moto> CrearMotos()
    {
        List<Moto> motos = new List<Moto>
        {
            new Moto(0,  "Yamaha MT09",                  3,  68000000m),
            new Moto(1,  "Yamaha Tracer 900 GT",          3,  75000000m),
            new Moto(2,  "Honda CBR1000Rr",              3, 115000000m),
            new Moto(3,  "Honda Africa Twin CRF1100L",    3, 105000000m),
            new Moto(4,  "BMW R1300 GS",                 3, 125000000m),
            new Moto(5,  "BMW M1000RR",                  3, 195000000m),
            new Moto(6,  "KTM 1390 Super Adv",           3, 118000000m),
            new Moto(7,  "KTM 1390 Super Duke R",        3, 110000000m),
            new Moto(8,  "Suzuki GSX R1000R",            3, 112000000m),
            new Moto(9,  "Suzuki V-Strom 1050",          3,  84000000m),
            new Moto(10, "Kawasaki Ninja ZX-10R",        3,  92000000m),
            new Moto(11, "Kawasaki Ninja H2",            3, 140000000m),
            new Moto(12, "Ducati Panigale V4",           3, 138000000m),
            new Moto(13, "Ducati Multistrada V4",        3, 128000000m),
            new Moto(14, "Triumph Rocket 3",             3, 135000000m)
        };

        return motos;
    }

    public List<Vendedor> CrearVendedores()
    {
        List<Vendedor> vendedores = new List<Vendedor>
        {
            new Vendedor(101),
            new Vendedor(102),
            new Vendedor(103),
            new Vendedor(104),
            new Vendedor(105)
        };

        return vendedores;
    }
}
