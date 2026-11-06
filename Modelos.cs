using System;
using System.Collections.Generic;

namespace ConcesionarioMotos;

// Indica las formas de pago permitidas.
public enum MetodoPago
{
    Efectivo = 1,
    TarjetaCredito = 2
}

// Representa una motocicleta del inventario.
public class Moto
{
    public int Id { get; private set; }
    public string Modelo { get; private set; }
    public int Cantidad { get; private set; }
    public decimal Precio { get; private set; }

    public Moto(int id, string modelo, int cantidad, decimal precio)
    {
        if (id < 0)
        {
            throw new ArgumentException("El ID no puede ser negativo.");
        }

        if (string.IsNullOrWhiteSpace(modelo))
        {
            throw new ArgumentException("El modelo es obligatorio.");
        }

        if (cantidad < 0)
        {
            throw new ArgumentException("La cantidad inicial no puede ser negativa.");
        }

        if (precio < 0)
        {
            throw new ArgumentException("El precio no puede ser negativo.");
        }

        Id = id;
        Modelo = modelo;
        Cantidad = cantidad;
        Precio = precio;
    }

    // Aumenta las unidades disponibles de la motocicleta.
    public void AgregarStock(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ArgumentException("La cantidad debe ser mayor que cero.");
        }

        Cantidad = Cantidad + cantidad;
    }

    // Comprueba si existe la cantidad solicitada.
    public bool TieneStock(int cantidad)
    {
        return cantidad > 0 && Cantidad >= cantidad;
    }

    // Descuenta unidades después de una venta.
    public void DescontarStock(int cantidad)
    {
        if (!TieneStock(cantidad))
        {
            throw new ArgumentException("Stock insuficiente o cantidad no valida.");
        }

        Cantidad = Cantidad - cantidad;
    }
}

// Representa a un vendedor y guarda su historial de ventas.
public class Vendedor
{
    private List<Venta> ventas = new List<Venta>();

    public int Codigo { get; private set; }

    public IReadOnlyList<Venta> Ventas
    {
        get { return ventas.AsReadOnly(); }
    }

    public Vendedor(int codigo)
    {
        if (codigo <= 0)
        {
            throw new ArgumentException("El codigo del vendedor debe ser positivo.");
        }

        Codigo = codigo;
    }

    // Agrega una venta al historial del vendedor.
    public void RegistrarVenta(Venta venta)
    {
        if (venta == null)
        {
            throw new ArgumentNullException(nameof(venta));
        }

        if (venta.CodigoVendedor != Codigo)
        {
            throw new ArgumentException("La venta no corresponde a este vendedor.");
        }

        ventas.Add(venta);
    }
}

// Representa una venta realizada en el concesionario.
public class Venta
{
    public int CodigoVendedor { get; private set; }
    public int IdMoto { get; private set; }
    public string ModeloMoto { get; private set; }
    public int Cantidad { get; private set; }
    public decimal PrecioUnitario { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Descuento { get; private set; }
    public decimal Total { get; private set; }
    public MetodoPago MetodoPago { get; private set; }
    public DateTime Fecha { get; private set; }

    public Venta(
        int codigoVendedor,
        int idMoto,
        string modeloMoto,
        int cantidad,
        decimal precioUnitario,
        decimal subtotal,
        decimal descuento,
        decimal total,
        MetodoPago metodoPago)
    {
        CodigoVendedor = codigoVendedor;
        IdMoto = idMoto;
        ModeloMoto = modeloMoto;
        Cantidad = cantidad;
        PrecioUnitario = precioUnitario;
        Subtotal = subtotal;
        Descuento = descuento;
        Total = total;
        MetodoPago = metodoPago;
        Fecha = DateTime.Now;
    }
}
