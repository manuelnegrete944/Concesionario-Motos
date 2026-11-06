using System.Globalization;
using ConcesionarioMotos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ConcesionarioMotos.Web.Pages;

public sealed class EstadoConcesionario
{
    public List<Moto> Motos { get; } = new DatosIniciales().CrearMotos();
    public List<Vendedor> Vendedores { get; } = new DatosIniciales().CrearVendedores();
}

public class IndexModel(EstadoConcesionario estado) : PageModel
{
    public (int Id, string Modelo, int Stock, decimal Precio)[] Inventario { get; private set; } = [];
    public (int Codigo, long Unidades, decimal Total)[] Consolidado { get; private set; } = [];
    public (string Modelo, long Unidades)[] Detalle { get; private set; } = [];
    public int? Codigo { get; private set; }
    public Venta? Factura { get; private set; }
    public string? Error { get; private set; }
    [TempData] public string? Mensaje { get; set; }

    public static string Dinero(decimal valor) =>
        "$" + valor.ToString("N0", CultureInfo.GetCultureInfo("es-CO"));

    public void OnGet(int? codigo, int? factura)
    {
        lock (estado)
        {
            Cargar(codigo);
            if (factura is > 0 && codigo.HasValue)
            {
                var vendedor = estado.Vendedores.Find(v => v.Codigo == codigo);
                if (vendedor != null && factura.Value <= vendedor.Ventas.Count)
                    Factura = vendedor.Ventas[factura.Value - 1];
            }
        }
    }

    public IActionResult OnPostLote(int? idMoto, int? cantidad)
    {
        lock (estado)
        {
            var moto = estado.Motos.Find(m => m.Id == idMoto);
            if (!ModelState.IsValid || moto == null || cantidad is null or <= 0)
                return Rechazar("Selecciona una motocicleta e ingresa una cantidad positiva.");
            if (cantidad.Value > int.MaxValue - moto.Cantidad)
                return Rechazar("La cantidad excede el límite del inventario.");
            moto.AgregarStock(cantidad.Value);
            Mensaje = $"Lote ingresado. Nuevo stock de {moto.Modelo}: {moto.Cantidad}.";
            return RedirectToPage();
        }
    }

    public IActionResult OnPostVenta(int? codigo, int? idMoto, int? cantidad, MetodoPago? pago)
    {
        lock (estado)
        {
            var vendedor = estado.Vendedores.Find(v => v.Codigo == codigo);
            var moto = estado.Motos.Find(m => m.Id == idMoto);
            if (!ModelState.IsValid || vendedor == null || moto == null || cantidad is null or <= 0
                || pago is not (MetodoPago.Efectivo or MetodoPago.TarjetaCredito))
                return Rechazar("Revisa el vendedor, la motocicleta, la cantidad y el pago.");
            if (!moto.TieneStock(cantidad.Value))
                return Rechazar($"Stock insuficiente. Unidades disponibles: {moto.Cantidad}.");

            decimal subtotal = moto.Precio * cantidad.Value;
            decimal descuento = pago == MetodoPago.Efectivo ? subtotal * 0.02m : 0m;
            var venta = new Venta(vendedor.Codigo, moto.Id, moto.Modelo, cantidad.Value,
                moto.Precio, subtotal, descuento, subtotal - descuento, pago.Value);
            moto.DescontarStock(cantidad.Value);
            vendedor.RegistrarVenta(venta);
            return RedirectToPage(new { codigo, factura = vendedor.Ventas.Count });
        }
    }

    private IActionResult Rechazar(string mensaje)
    {
        Error = mensaje;
        Cargar(null);
        return Page();
    }

    private void Cargar(int? codigo)
    {
        Inventario = estado.Motos.Select(m => (m.Id, m.Modelo, m.Cantidad, m.Precio)).ToArray();
        Consolidado = estado.Vendedores.Select(v => (v.Codigo,
            v.Ventas.Sum(x => (long)x.Cantidad), v.Ventas.Sum(x => x.Total))).ToArray();
        var vendedor = estado.Vendedores.Find(v => v.Codigo == codigo);
        if (vendedor == null)
        {
            if (codigo.HasValue)
                Error = "No existe un vendedor con ese código.";
            return;
        }

        Codigo = vendedor.Codigo;
        Detalle = estado.Motos.Select(m => (m.Modelo,
            vendedor.Ventas.Where(v => v.IdMoto == m.Id).Sum(v => (long)v.Cantidad))).ToArray();
    }
}
