namespace espacioCadeteria;

public enum EstadoPedido { Pendiente = 0, Entregado = 1 }
public class Pedido
{
    private int Nro { get; set; }
    private string Obs { get; set; }
    private Cliente Cliente { get; set; }
    private EstadoPedido Estado { get; set; }
    public EstadoPedido verEstado() => Estado;
    private Cadete CadeteAsignado { get; set; } //nuevo
    public Pedido(int nro, Cliente cliente, string obs = "") { Nro = nro; Cliente = cliente; Obs = obs ?? ""; }
    public int VerNro() => Nro;
    public string VerNombreCliente() => Cliente.VerNombre();
    public string VerTelefonoCliente() => Cliente.VerTelefono();
    public string VerRefDirCliente() => Cliente.VerRefDir();
    public string VerObs() => Obs;
    public Cadete VerCadete() => CadeteAsignado;
    public string VerDireccionCliente() { return Cliente.VerDireccion(); }
    public string VerDatosCliente() { return Cliente.VerDatos(); }
    public void CambiarEstado(EstadoPedido nuevo) { Estado = nuevo; }
    public void AsignarCadete(Cadete cadete) { CadeteAsignado = cadete; }
    public void DesasignarCadete() => CadeteAsignado = null;
    public bool EsEntregadoPorCadete(int idCadete) => CadeteAsignado?.VerId() == idCadete && Estado == EstadoPedido.Entregado;
}