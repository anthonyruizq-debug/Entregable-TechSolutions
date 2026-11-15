using System;

namespace TechSolutions.Entidades.Usuarios
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UsuarioNombre { get; set; }
        public byte[] Salt { get; set; }
        public byte[] Hash { get; set; }
        public int RolId { get; set; }
        public int IntentosFallidos { get; set; }
        public bool Bloqueado { get; set; }
        public DateTime FechaRegistro { get; set; }

        public bool BloqueoPermanente { get; set; }
        public int DesbloqueosAdmin { get; set; }
    }
}