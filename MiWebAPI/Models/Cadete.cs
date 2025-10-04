namespace espacioCadeteria;

public class Cadete
{
    private int id { get; }
    private string nombre { get; }
    private string direccion { get; }
    private string telefono { get; }
    public Cadete(int id, string nombre, string direccion, string telefono) { this.id = id; this.nombre = nombre; this.direccion = direccion; this.telefono = telefono; }
    public int VerId() => id;
    public string VerNombre() => nombre;
    public string VerTelefono() => telefono;
    public string VerDireccion() => direccion;
}