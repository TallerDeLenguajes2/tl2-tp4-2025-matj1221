using System;
using System.Collections.Generic;
using System.Linq;

namespace espacioCadeteria;

public class Cadeteria
{
    private string nombre { get; }
    private string telefono { get; }
    private List<Cadete> ListadoCadetes { get; } = new();
    private List<Pedido> ListadoPedidos { get; } = new();
    public Cadeteria(string nombre, string telefono) { this.nombre = nombre; this.telefono = telefono; }
    public List<Cadete> VerCadetes() => ListadoCadetes;
    public List<Pedido> VerPedidos() => ListadoPedidos;
    public void AgregarCadete(Cadete agregar) { ListadoCadetes.Add(agregar); }

    private Cadete BuscarCadetePorId(int id) => ListadoCadetes.FirstOrDefault(c => c.VerId() == id);
    private Pedido BuscarPedidoPorNro(int nro) => ListadoPedidos.FirstOrDefault(p => p.VerNro() == nro);
    public bool AltaPedido(int nro, string nombre, string direccion, string telefono, string refDir, string obs)
    {
        if (ListadoPedidos.Any(p => p.VerNro() == nro)) return false; // nro unico
        var cliente = new Cliente(nombre, direccion, telefono, refDir);
        var pedido = new Pedido(nro, cliente, obs);
        ListadoPedidos.Add(pedido);
        return true;
    }

    public bool EliminarPedido(int nro)
    {
        var pedido = BuscarPedidoPorNro(nro);
        if (pedido is null) return false;
        ListadoPedidos.Remove(pedido);
        return true;
    }
    public bool AsignarCadeteAPedido(int idCadete, int nroPedido)
    {
        var cadete = BuscarCadetePorId(idCadete);
        var pedido = BuscarPedidoPorNro(nroPedido);
        if (cadete is null || pedido is null) return false;

        pedido.AsignarCadete(cadete);
        return true;
    }

    public bool CambiarEstado(int nroPedido, EstadoPedido nuevo)
    {
        var pedido = BuscarPedidoPorNro(nroPedido);
        if (pedido is null) return false;
        pedido.CambiarEstado(nuevo);
        return true;
    }

    public bool ReasignarPedido(int nroPedido, int idCadeteDestino)
    {
        var pedido = BuscarPedidoPorNro(nroPedido);
        var destino = BuscarCadetePorId(idCadeteDestino);
        if (pedido is null || destino is null) return false;

        pedido.AsignarCadete(destino);
        return true;
    }
    public List<Pedido> VerPedidosSinAsignar() => ListadoPedidos.Where(p => p.VerCadete() == null).ToList();
    public int JornalPorPedido = 500;
    public int CantidadEntregados(int idCadete)
        => ListadoPedidos.Count(p => p.EsEntregadoPorCadete(idCadete));
    public int JornalACobrar(int idCadete) => CantidadEntregados(idCadete) * JornalPorPedido;

    public string VerNombre() => nombre;
    public string VerTelefono() => telefono;

    public bool AgregarPedido(Pedido p)
    {
        if (ListadoPedidos.Any(x => x.VerNro() == p.VerNro())) return false;
        ListadoPedidos.Add(p);
        return true;
    }

    public bool CambiarEstadoPedidoPorNro(int nroPedido, EstadoPedido nuevo) => CambiarEstado(nroPedido, nuevo);
    public bool ReasignarPedidoPorNro(int nroPedido, int idCadeteDestino) => ReasignarPedido(nroPedido, idCadeteDestino);

    public void AgregarListaCadetes(List<Cadete> cadetes)
    {
        ListadoCadetes.Clear();
        if (cadetes != null) ListadoCadetes.AddRange(cadetes);
    }

    public void AgregarListaPedidos(List<Pedido> pedidos)
    {
        ListadoPedidos.Clear();
        if (pedidos != null) ListadoPedidos.AddRange(pedidos);
    }
}