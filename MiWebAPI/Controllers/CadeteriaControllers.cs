using System.Linq;
using espacioCadeteria;
using Microsoft.AspNetCore.Mvc;

// DTOs simples para requests/responses
public record PedidoDto(int nro, string nombre, string direccion, string telefono, string refDir, string? obs, int estado, int? idCadete);
public record AsignacionDto(int idPedido, int idCadete);
public record EstadoDto(int idPedido, int nuevoEstado);
public record ReasignacionDto(int idPedido, int idNuevoCadete);

public record InformeCadeteDto(int idCadete, string nombre, int entregados, int jornal);
public record InformeDto(int totalPedidos, int pendientes, int entregados, List<InformeCadeteDto> porCadete);

[ApiController]
[Route("api/[controller]")]
public class CadeteriaController : ControllerBase
{
    private Cadeteria _cadeteria = new Cadeteria("Mi Cadetería", "381-5555555");
    // GET: GetPedidos() => lista de pedidos
    [HttpGet("pedidos")]
    public ActionResult<List<object>> GetPedidos()
    {
        var lista = _cadeteria.VerPedidos();
        return Ok(lista);
    }

    // GET: GetCadetes() => lista de cadetes
    [HttpGet("cadetes")]
    public ActionResult<List<object>> GetCadetes()
    {
        var lista = _cadeteria.VerCadetes(); // Id, Nombre, Telefono, Direccion
        return Ok(lista);
    }

    // GET: GetInforme() => objeto Inforsme
    [HttpGet("informe")]
    public ActionResult<InformeDto> GetInforme()
    {
        var cadetes = _cadeteria.VerCadetes();
        var pedidos = _cadeteria.VerPedidos();

        int total = pedidos.Count();
        int entregados = pedidos.Count(p => p.verEstado() == EstadoPedido.Entregado);
        int pendientes = total - entregados;

        var porCadete =
            from c in cadetes
            let cant = _cadeteria.CantidadEntregados(c.VerId())
            let jornal = _cadeteria.JornalACobrar(c.VerId())
            select new InformeCadeteDto(c.VerId(), c.VerNombre(), cant, jornal);

        return Ok(new InformeDto(total, pendientes, entregados, porCadete.ToList()));
    }

    // POST: AgregarPedido(Pedido pedido)
    [HttpPost("pedidos")]
    public ActionResult AgregarPedido([FromBody] PedidoDto dto)
    {
        if (dto is null) return BadRequest("Body inválido"); // 400
        var cliente = new Cliente(dto.nombre, dto.direccion, dto.telefono, dto.refDir);
        var p = new Pedido(dto.nro, cliente, dto.obs ?? "");
        _cadeteria.AgregarPedido(p);

        // Si vino idCadete, asignamos
        if (dto.idCadete is int idC)
        {
            var ok = _cadeteria.AsignarCadeteAPedido(idC, dto.nro);
            if (!ok) return NotFound("Cadete o Pedido no encontrado"); // 404
        }

        // 201 Created (como “vimos en clase”)
        return Created($"/api/cadeteria/pedidos/{dto.nro}", new { dto.nro });
    }

    // PUT: AsignarPedido(int idPedido, int idCadete)
    [HttpPut("pedidos/{idPedido:int}/asignar/{idCadete:int}")]
    public ActionResult AsignarPedido([FromRoute] int idPedido, [FromRoute] int idCadete)
    {
        var ok = _cadeteria.AsignarCadeteAPedido(idCadete, idPedido);
        return ok ? Ok() : NotFound("Cadete o Pedido no encontrado");
    }

    // PUT: CambiarEstadoPedido(int idPedido,int NuevoEstado)
    [HttpPut("pedidos/{idPedido:int}/estado/{nuevoEstado:int}")]
    public ActionResult CambiarEstadoPedido([FromRoute] int idPedido, [FromRoute] int nuevoEstado)
    {
        if (nuevoEstado is not (0 or 1)) return BadRequest("Estado inválido (0=Pendiente,1=Entregado)");
        var ok = _cadeteria.CambiarEstadoPedidoPorNro(idPedido, (EstadoPedido)nuevoEstado);
        return ok ? Ok() : NotFound("Pedido no encontrado");
    }

    // PUT: CambiarCadetePedido(int idPedido,int idNuevoCadete)
    [HttpPut("pedidos/{idPedido:int}/cadete/{idNuevoCadete:int}")]
    public ActionResult CambiarCadetePedido([FromRoute] int idPedido, [FromRoute] int idNuevoCadete)
    {
        var ok = _cadeteria.ReasignarPedidoPorNro(idPedido, idNuevoCadete);
        return ok ? Ok() : NotFound("Cadete o Pedido no encontrado");
    }
}
