namespace espacioCadeteria
{
    using System.Collections.Generic;

    public interface IAccesoADatos
    {
        Cadeteria CargarCadeteria(string ruta);
        List<Cadete> CargarCadetes(string ruta);
        List<Pedido> CargarPedidos(string ruta, List<Cadete> cadetes);
        void GuardarCadeteria(Cadeteria cadeteria, string ruta);
        void GuardarCadetes(List<Cadete> cadetes, string ruta);
        void GuardarPedidos(List<Pedido> pedidos, string ruta);
    }
}
