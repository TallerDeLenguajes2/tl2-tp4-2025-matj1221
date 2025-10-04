namespace espacioCadeteria;

public class Cliente
{
    private string Nombre { get; }
    private string Direccion { get; set; }
    private string Telefono { get; set; }
    private string DatosReferenciaDireccion { get; set; }
    public Cliente(string nombre, string direccion, string telefono, string datosReferenciaDireccion) { Nombre = nombre; Direccion = direccion; Telefono = telefono; DatosReferenciaDireccion = datosReferenciaDireccion; }
    public string VerDireccion() { return Direccion; }
    public string VerDatos() { return $"{Nombre} | {Telefono}"; }
    public string VerNombre() => Nombre;
    public string VerTelefono() => Telefono;
    public string VerRefDir() => DatosReferenciaDireccion;
}