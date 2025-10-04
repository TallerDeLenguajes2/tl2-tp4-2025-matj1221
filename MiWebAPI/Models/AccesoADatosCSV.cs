namespace espacioCadeteria
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public class AccesoADatosCSV : IAccesoADatos
    {
        static string[] Split(string s) =>
            s.Split(new[] { ';', ',' }, StringSplitOptions.None)   // no usar RemoveEmptyEntries!
             .Select(x => x.Trim()).ToArray();

        public Cadeteria CargarCadeteria(string ruta)
        {
            // Ej: Nombre;Telefono
            var lineas = File.ReadAllLines(ruta).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            int start = lineas.Count > 0 && Split(lineas[0])[0].Equals("Nombre", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            var c = Split(lineas[start]);
            return new Cadeteria(c[0], c[1]);
        }

        public List<Cadete> CargarCadetes(string ruta)
        {
            var res = new List<Cadete>();
            var lineas = File.ReadAllLines(ruta).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            int start = lineas.Count > 0 && Split(lineas[0])[0].Equals("Id", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
            for (int i = start; i < lineas.Count; i++)
            {
                var c = Split(lineas[i]);
                if (c.Length < 4) continue;
                res.Add(new Cadete(int.Parse(c[0]), c[1], c[2], c[3]));
            }
            return res;
        }

        public List<Pedido> CargarPedidos(string ruta, List<Cadete> cadetes)
        {
            // Ej Pedidos: Nro;Nombre;Direccion;Telefono;RefDir;Obs;IdCadeteAsignado;Estado
            var res = new List<Pedido>();
            var dicCad = cadetes.ToDictionary(c => c.VerId(), c => c);
            var lineas = File.ReadAllLines(ruta).Where(l => !string.IsNullOrWhiteSpace(l)).ToList();
            int start = lineas.Count > 0 && Split(lineas[0])[0].Equals("Nro", StringComparison.OrdinalIgnoreCase) ? 1 : 0;

            for (int i = start; i < lineas.Count; i++)
            {
                var c = Split(lineas[i]);
                if (c.Length < 8) continue;
                int nro = int.Parse(c[0]);
                var cli = new Cliente(c[1], c[2], c[3], c[4]);
                var p = new Pedido(nro, cli, c[5]);
                if (int.TryParse(c[6], out int idCad) && dicCad.TryGetValue(idCad, out var cad))
                    p.AsignarCadete(cad);
                if (int.TryParse(c[7], out int est))
                    p.CambiarEstado((EstadoPedido)est);
                res.Add(p);
            }
            return res;
        }

        public void GuardarCadeteria(Cadeteria cadeteria, string ruta)
        {
            File.WriteAllText(ruta, "Nombre;Telefono\n" + $"{cadeteria.VerNombre()};{cadeteria.VerTelefono()}");
        }

        public void GuardarCadetes(List<Cadete> cadetes, string ruta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id;Nombre;Direccion;Telefono");
            foreach (var c in cadetes)
                sb.AppendLine($"{c.VerId()};{c.VerNombre()};{c.VerDireccion()};{c.VerTelefono()}");
            File.WriteAllText(ruta, sb.ToString());
        }

        public void GuardarPedidos(List<Pedido> pedidos, string ruta)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Nro;Nombre;Direccion;Telefono;RefDir;Obs;IdCadeteAsignado;Estado");
            foreach (var p in pedidos)
            {
                var cadId = p.VerCadete()?.VerId().ToString() ?? "";
                sb.AppendLine($"{p.VerNro()};{p.VerNombreCliente()};{p.VerDireccionCliente()};{p.VerTelefonoCliente()};{p.VerRefDirCliente()};{p.VerObs()};{cadId};{(int)p.verEstado()}");
            }
            File.WriteAllText(ruta, sb.ToString());
        }
    }
}
