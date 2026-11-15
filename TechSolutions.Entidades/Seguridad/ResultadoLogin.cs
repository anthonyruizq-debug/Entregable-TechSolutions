using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace TechSolutions.Entidades.Seguridad
{
    public class ResultadoLogin
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; }
        public bool UsuarioBloqueado { get; set; }

        // Información adicional para identificar al usuario
        public int UsuarioId { get; set; }
        public int RolId { get; set; }
        public string Nombre { get; set; }

        // Indica si el bloqueo es permanente
        public bool BloqueoPermanente { get; set; }
    }
}
