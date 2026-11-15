
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;

namespace TechSolutions.Datos.Conexion
{
    public class ConexionBD
    {
        private static ConexionBD instancia;
        private static readonly object bloqueo = new object();

        private readonly string cadenaConexion =
    ConfigurationManager.ConnectionStrings["ConexionBD"].ConnectionString;
        private ConexionBD()
        {
        }

        public static ConexionBD ObtenerInstancia()
        {
            if (instancia == null)
            {
                lock (bloqueo)
                {
                    if (instancia == null)
                    {
                        instancia = new ConexionBD();
                    }
                }
            }

            return instancia;
        }

        public SqlConnection ObtenerConexion()
        {
            return new SqlConnection(cadenaConexion);
        }
    }
}