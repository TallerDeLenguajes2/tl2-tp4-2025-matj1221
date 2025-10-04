using System.IO;
using System.Text.Json;

namespace espacioCadeteria
{
    public class AccesoADatosCadeteria
    {
        private readonly string _ruta;
        static JsonSerializerOptions Opt => new() { WriteIndented = true, PropertyNameCaseInsensitive = true };

        private class CadeteriaDTO { public string nombre { get; set; } public string telefono { get; set; } }

        public AccesoADatosCadeteria(string ruta = "Data/cadeteria.json") { _ruta = ruta; }

        public Cadeteria Obtener()
        {
            if (!File.Exists(_ruta)) return new Cadeteria("Mi Cadetería", "381-5555555");
            var dto = JsonSerializer.Deserialize<CadeteriaDTO>(File.ReadAllText(_ruta), Opt);
            return new Cadeteria(dto?.nombre ?? "Mi Cadetería", dto?.telefono ?? "381-5555555");
        }
    }
}
