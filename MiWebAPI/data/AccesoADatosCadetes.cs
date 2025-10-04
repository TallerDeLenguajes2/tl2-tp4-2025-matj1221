using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace espacioCadeteria
{
    public class AccesoADatosCadetes
    {
        private readonly string _ruta;
        static JsonSerializerOptions Opt => new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

        private class CadeteDTO { public int id { get; set; } public string nombre { get; set; } public string direccion { get; set; } public string telefono { get; set; } }

        public AccesoADatosCadetes(string ruta = "Data/cadetes.json") { _ruta = ruta; }

        public List<Cadete> Obtener()
        {
            if (!File.Exists(_ruta)) return new List<Cadete>();
            var dtos = JsonSerializer.Deserialize<List<CadeteDTO>>(File.ReadAllText(_ruta), Opt) ?? new();
            return dtos.Select(d => new Cadete(d.id, d.nombre, d.direccion, d.telefono)).ToList();
        }
    }
}
