using System.Linq;
using espacioCadeteria;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CadeteriaController : ControllerBase
{
    private Cadeteria _cadeteria;
    private AccesoADatosCadeteria _adCadeteria;
    private AccesoADatosCadetes _adCadetes;
    private AccesoADatosPedidos _adPedidos;

    public CadeteriaController()
    {
        _adCadeteria = new AccesoADatosCadeteria();
        _adCadetes = new AccesoADatosCadetes();
        _adPedidos = new AccesoADatosPedidos();

        _cadeteria = _adCadeteria.Obtener();
        var cadetes = _adCadetes.Obtener();
        var pedidos = _adPedidos.Obtener(cadetes);

        _cadeteria.AgregarListaCadetes(cadetes);
        _cadeteria.AgregarListaPedidos(pedidos);
    }

    // ========== GETs ==========
    [HttpGet("pedidos")]
    public ActionResult GetPedidos()
    {
        var lista = _cadeteria.VerPedidos()
            .Select(p => new { nro = p.VerNro(), estado = (int)p.verEstado(), idCadete = p.VerCadete()?.VerId() });
        return Ok(lista);
    }

    [HttpGet("cadetes")]
    public ActionResult GetCadetes()
    {
        var lista = _cadeteria.VerCadetes()
            .Select(c => new { id = c.VerId(), nombre = c.VerNombre(), telefono = c.VerTelefono(), direccion = c.VerDireccion() });
        return Ok(lista);
    }

    [HttpGet("informe")]
    public ActionResult GetInforme()
    {
        var cadetes = _cadeteria.VerCadetes();
        var pedidos = _cadeteria.VerPedidos();

        int total = pedidos.Count();
        int entregados = pedidos.Count(p => p.verEstado() == EstadoPedido.Entregado);
        int pendientes = total - entregados;

        var porCadete = cadetes.Select(c => new
        {
            idCadete = c.VerId(),
            nombre = c.VerNombre(),
            entregados = _cadeteria.CantidadEntregados(c.VerId()),
            jornal = _cadeteria.JornalACobrar(c.VerId())
        });

        return Ok(new { totalPedidos = total, pendientes, entregados, porCadete });
    }

    // ========== POST ==========
    public record PedidoCreateDto(int nro, string nombre, string direccion, string telefono, string refDir, string? obs, int? idCadete);

    [HttpPost("pedidos")]
    public ActionResult AgregarPedido([FromBody] PedidoCreateDto dto)
    {
        if (dto is null) return BadRequest("Body inválido");
        if (dto.nro <= 0 || string.IsNullOrWhiteSpace(dto.nombre)) return BadRequest("Datos inválidos");

        var cliente = new Cliente(dto.nombre, dto.direccion, dto.telefono, dto.refDir);
        var p = new Pedido(dto.nro, cliente, dto.obs ?? "");
        if (!_cadeteria.AgregarPedido(p)) return BadRequest("Nro de pedido ya existe");

        if (dto.idCadete is int idC)
        {
            var ok = _cadeteria.AsignarCadeteAPedido(idC, dto.nro);
            if (!ok) return NotFound("Cadete o Pedido no encontrado");
        }

        // **TP5 punto 3: Guardar después de alta**
        _adPedidos.Guardar(_cadeteria.VerPedidos());

        return Created($"/api/cadeteria/pedidos/{dto.nro}", new { dto.nro });
    }

    // ========== PUTs ==========
    [HttpPut("pedidos/{idPedido:int}/asignar/{idCadete:int}")]
    public ActionResult AsignarPedido([FromRoute] int idPedido, [FromRoute] int idCadete)
    {
        var ok = _cadeteria.AsignarCadeteAPedido(idCadete, idPedido);
        if (!ok) return NotFound("Cadete o Pedido no encontrado");

        // **TP5 punto 3: Guardar después de asignar**
        _adPedidos.Guardar(_cadeteria.VerPedidos());
        return Ok();
    }

    [HttpPut("pedidos/{idPedido:int}/estado/{nuevoEstado:int}")]
    public ActionResult CambiarEstadoPedido([FromRoute] int idPedido, [FromRoute] int nuevoEstado)
    {
        if (nuevoEstado is not (0 or 1)) return BadRequest("Estado inválido (0=Pendiente,1=Entregado)");

        var ok = _cadeteria.CambiarEstadoPedidoPorNro(idPedido, (EstadoPedido)nuevoEstado);
        if (!ok) return NotFound("Pedido no encontrado");

        // **TP5 punto 3: Guardar después de cambiar estado**
        _adPedidos.Guardar(_cadeteria.VerPedidos());
        return Ok();
    }

    [HttpPut("pedidos/{idPedido:int}/cadete/{idNuevoCadete:int}")]
    public ActionResult CambiarCadetePedido([FromRoute] int idPedido, [FromRoute] int idNuevoCadete)
    {
        var ok = _cadeteria.ReasignarPedidoPorNro(idPedido, idNuevoCadete);
        if (!ok) return NotFound("Cadete o Pedido no encontrado");

        // **TP5 punto 3: Guardar después de reasignar**
        _adPedidos.Guardar(_cadeteria.VerPedidos());
        return Ok();
    }
}
