using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace espacioCadeteria;

public class AccesoADatosPedidos
{
    private readonly string _ruta;
    static JsonSerializerOptions Opt => new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

    public AccesoADatosPedidos(string ruta = "Data/pedidos.json") { _ruta = ruta; }

    public List<Pedido> Obtener(List<Cadete> cadetes)
    {
        // 1) archivo inexistente => lista vacia
        if (!File.Exists(_ruta)) return new List<Pedido>();

        // NEW: leer una vez y cortar si está vacío/blanco
        var raw = File.ReadAllText(_ruta);
        if (string.IsNullOrWhiteSpace(raw)) return new List<Pedido>();

        // 2) deserializar
        var dtos = JsonSerializer.Deserialize<List<PedidoDTO>>(File.ReadAllText(_ruta), Opt) ?? new();

        //cadetes nulos son aceptados
        var dic = (cadetes ?? new List<Cadete>()).ToDictionary(c => c.VerId(), c => c);

        var res = new List<Pedido>();
        foreach (var d in dtos)
        {
            // NEW: saltear entradas inválidas
            if (d == null || d.cliente == null) continue;

            var cli = new Cliente(
                d.cliente.nombre ?? string.Empty,
                d.cliente.direccion ?? string.Empty,
                d.cliente.telefono ?? string.Empty,
                d.cliente.refDir ?? string.Empty
            );

            var p = new Pedido(d.nro, cli, d.obs ?? string.Empty);

            // asumís que d.estado es válido para tu enum
            p.CambiarEstado((EstadoPedido)d.estado);

            if (d.idCadete >= 0 && dic.TryGetValue(d.idCadete, out var cad))
                p.AsignarCadete(cad);

            res.Add(p);
        }
        return res;
    }

    public void Guardar(List<Pedido> pedidos)
    {
        var dtos = pedidos.Select(p => new PedidoDTO
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
            idCadete = p.VerCadete() != null ? p.VerCadete().VerId() : -1
        }).ToList();

        Directory.CreateDirectory(Path.GetDirectoryName(_ruta) ?? ".");
        File.WriteAllText(_ruta, JsonSerializer.Serialize(dtos, Opt));
    }
}
