using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
namespace espacioCadeteria;

public class CadeteriaDTO
{
    public string nombre { get; set; }
    public string telefono { get; set; }
}

public class CadeteDTO
{
    public int id { get; set; }
    public string nombre { get; set; }
    public string direccion { get; set; }
    public string telefono { get; set; }
}

public class ClienteDTO
{
    public string nombre { get; set; }
    public string direccion { get; set; }
    public string telefono { get; set; }
    public string refDir { get; set; }
}

public class PedidoDTO
{
    public int nro { get; set; }
    public ClienteDTO cliente { get; set; }
    public string obs { get; set; }
    public int estado { get; set; }
    public int idCadete { get; set; } // -1 = sin asignar
}

public class AccesoADatosJSON : IAccesoADatos
{
    static JsonSerializerOptions Opt => new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    public Cadeteria CargarCadeteria(string ruta)
    {
        var dto = JsonSerializer.Deserialize<CadeteriaDTO>(File.ReadAllText(ruta))!;
        return new Cadeteria(dto.nombre, dto.telefono);
    }

    public List<Cadete> CargarCadetes(string ruta)
    {
        var dtos = JsonSerializer.Deserialize<List<CadeteDTO>>(File.ReadAllText(ruta)) ?? new();
        return dtos.Select(d => new Cadete(d.id, d.nombre, d.direccion, d.telefono)).ToList();
    }

    public List<Pedido> CargarPedidos(string ruta, List<Cadete> cadetes)
    {
        var dic = cadetes.ToDictionary(c => c.VerId(), c => c);
        var dtos = JsonSerializer.Deserialize<List<PedidoDTO>>(File.ReadAllText(ruta)) ?? new();
        var res = new List<Pedido>();

        foreach (var d in dtos)
        {
            var cli = new Cliente(d.cliente.nombre, d.cliente.direccion, d.cliente.telefono, d.cliente.refDir);
            var p = new Pedido(d.nro, cli, d.obs);
            p.CambiarEstado((EstadoPedido)d.estado);

            if (d.idCadete >= 0 && dic.TryGetValue(d.idCadete, out var cadete))
                p.AsignarCadete(cadete);

            res.Add(p);
        }
        return res;
    }

    public void GuardarCadeteria(Cadeteria cadete, string ruta)
    {
        var dto = new CadeteriaDTO
        {
            nombre = cadete.VerNombre(),
            telefono = cadete.VerTelefono()
        };
        File.WriteAllText(ruta, JsonSerializer.Serialize(dto, Opt));
    }

    public void GuardarCadetes(List<Cadete> cs, string ruta)
    {
        var dtos = cs.Select(c => new CadeteDTO
        {
            id = c.VerId(),
            nombre = c.VerNombre(),
            direccion = c.VerDireccion(),
            telefono = c.VerTelefono()
        }).ToList();

        File.WriteAllText(ruta, JsonSerializer.Serialize(dtos, Opt));
    }

    public void GuardarPedidos(List<Pedido> ps, string ruta)
    {
        var dtos = ps.Select(p => new PedidoDTO
        {
            nro = p.VerNro(),
            cliente = new ClienteDTO
            {
                nombre = p.VerNombreCliente(),
                direccion = p.VerDireccionCliente(),
                telefono = p.VerTelefonoCliente(),
                refDir = p.VerRefDirCliente()
            },
            obs = p.VerObs(),
            estado = (int)p.verEstado(),
            idCadete = (p.VerCadete() != null) ? p.VerCadete().VerId() : -1  // sentinela
        }).ToList();

        File.WriteAllText(ruta, JsonSerializer.Serialize(dtos, Opt));
    }
}