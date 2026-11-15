
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TechSolutions.Datos.Conexion;
using TechSolutions.Entidades.Usuarios;

namespace TechSolutions.Datos.Repositorios
{
    public class UsuarioDAO
    {
        private readonly ConexionBD conexion =
            ConexionBD.ObtenerInstancia();

        public bool ExisteUsuario(string nombreUsuario)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string consulta =
                    "SELECT COUNT(*) FROM Usuarios WHERE Usuario = @Usuario";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50)
                        .Value = nombreUsuario;

                    cn.Open();

                    int cantidad = Convert.ToInt32(cmd.ExecuteScalar());

                    return cantidad > 0;
                }
            }
        }

        // Registrar usuario
        public bool RegistrarUsuario(Usuario usuario)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                using (SqlTransaction transaccion = cn.BeginTransaction())
                {
                    try
                    {
                        string consulta = @"
                            INSERT INTO Usuarios
                            (Nombre, Usuario, Salt, Hash, RolId)
                            VALUES
                            (@Nombre, @Usuario, @Salt, @Hash, @RolId)";

                        using (SqlCommand cmd = new SqlCommand(consulta, cn))
                        {
                            cmd.Transaction = transaccion;

                            cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100)
                                .Value = usuario.Nombre;

                            cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50)
                                .Value = usuario.UsuarioNombre;

                            cmd.Parameters.Add("@Salt", SqlDbType.VarBinary, 32)
                                .Value = usuario.Salt;

                            cmd.Parameters.Add("@Hash", SqlDbType.VarBinary, 64)
                                .Value = usuario.Hash;

                            cmd.Parameters.Add("@RolId", SqlDbType.Int)
                                .Value = usuario.RolId;

                            int filas = cmd.ExecuteNonQuery();

                            if (filas > 0)
                            {
                                transaccion.Commit();
                                return true;
                            }
                            else
                            {
                                transaccion.Rollback();
                                return false;
                            }
                        }
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        // Buscar usuario por nombre de usuario
        public Usuario ObtenerPorUsuario(string nombreUsuario)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string consulta = @"
                    SELECT Id, Nombre, Usuario, Salt, Hash,
                           RolId, IntentosFallidos, Bloqueado,
                           FechaRegistro, BloqueoPermanente,
                           DesbloqueosAdmin
                    FROM Usuarios
                    WHERE Usuario = @Usuario";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50)
                        .Value = nombreUsuario;

                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                UsuarioNombre = reader["Usuario"].ToString(),
                                Salt = (byte[])reader["Salt"],
                                Hash = (byte[])reader["Hash"],
                                RolId = Convert.ToInt32(reader["RolId"]),
                                IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                                Bloqueado = Convert.ToBoolean(reader["Bloqueado"]),
                                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                                BloqueoPermanente = Convert.ToBoolean(reader["BloqueoPermanente"]),
                                DesbloqueosAdmin = Convert.ToInt32(reader["DesbloqueosAdmin"])
                            };
                        }
                    }
                }
            }

            return null;
        }

        // Registrar intento fallido y bloquear al llegar a tres
        public bool RegistrarIntentoFallido(string nombreUsuario)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string consulta = @"
                    UPDATE Usuarios
                    SET IntentosFallidos = IntentosFallidos + 1,
                        Bloqueado = CASE
                            WHEN IntentosFallidos + 1 >= 3 THEN 1
                            ELSE 0
                        END,
                        BloqueoPermanente = CASE
                            WHEN DesbloqueosAdmin >= 1
                                 AND IntentosFallidos + 1 >= 3 THEN 1
                            ELSE BloqueoPermanente
                        END
                    WHERE Usuario = @Usuario
                      AND BloqueoPermanente = 0;

                    SELECT Bloqueado
                    FROM Usuarios
                    WHERE Usuario = @Usuario;";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50)
                        .Value = nombreUsuario;

                    cn.Open();

                    object resultado = cmd.ExecuteScalar();

                    return resultado != null &&
                           Convert.ToBoolean(resultado);
                }
            }
        }

        // Reiniciar intentos fallidos
        public void ReiniciarIntentosFallidos(string nombreUsuario)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string consulta = @"
                    UPDATE Usuarios
                    SET IntentosFallidos = 0
                    WHERE Usuario = @Usuario";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50)
                        .Value = nombreUsuario;

                    cn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Obtener usuarios bloqueados
        public List<Usuario> ObtenerUsuariosBloqueados()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string consulta = @"
                    SELECT Id, Nombre, Usuario, RolId,
                           IntentosFallidos, Bloqueado,
                           BloqueoPermanente, DesbloqueosAdmin
                    FROM Usuarios
                    WHERE Bloqueado = 1";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                UsuarioNombre = reader["Usuario"].ToString(),
                                RolId = Convert.ToInt32(reader["RolId"]),
                                IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                                Bloqueado = Convert.ToBoolean(reader["Bloqueado"]),
                                BloqueoPermanente = Convert.ToBoolean(reader["BloqueoPermanente"]),
                                DesbloqueosAdmin = Convert.ToInt32(reader["DesbloqueosAdmin"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        // Obtener todos los usuarios registrados
        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            List<Usuario> lista = new List<Usuario>();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string consulta = @"
                    SELECT Id, Nombre, Usuario, RolId,
                           IntentosFallidos, Bloqueado, FechaRegistro,
                           BloqueoPermanente, DesbloqueosAdmin
                    FROM Usuarios
                    ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            lista.Add(new Usuario
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Nombre = reader["Nombre"].ToString(),
                                UsuarioNombre = reader["Usuario"].ToString(),
                                RolId = Convert.ToInt32(reader["RolId"]),
                                IntentosFallidos = Convert.ToInt32(reader["IntentosFallidos"]),
                                Bloqueado = Convert.ToBoolean(reader["Bloqueado"]),
                                FechaRegistro = Convert.ToDateTime(reader["FechaRegistro"]),
                                BloqueoPermanente = Convert.ToBoolean(reader["BloqueoPermanente"]),
                                DesbloqueosAdmin = Convert.ToInt32(reader["DesbloqueosAdmin"])
                            });
                        }
                    }
                }
            }

            return lista;
        }

        // Desbloquear usuario solamente si no tiene bloqueo permanente
        public bool DesbloquearUsuario(int idUsuario)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string consulta = @"
                    UPDATE Usuarios
                    SET Bloqueado = 0,
                        IntentosFallidos = 0,
                        DesbloqueosAdmin = DesbloqueosAdmin + 1
                    WHERE Id = @Id
                      AND Bloqueado = 1
                      AND BloqueoPermanente = 0
                      AND DesbloqueosAdmin = 0";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int)
                        .Value = idUsuario;

                    cn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Actualizar los datos de un usuario
        public bool ActualizarUsuario(Usuario usuario)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string consulta = @"
                    UPDATE Usuarios
                    SET Nombre = @Nombre,
                        Usuario = @Usuario,
                        RolId = @RolId
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(consulta, cn))
                {
                    cmd.Parameters.Add("@Id", SqlDbType.Int)
                        .Value = usuario.Id;

                    cmd.Parameters.Add("@Nombre", SqlDbType.VarChar, 100)
                        .Value = usuario.Nombre;

                    cmd.Parameters.Add("@Usuario", SqlDbType.VarChar, 50)
                        .Value = usuario.UsuarioNombre;

                    cmd.Parameters.Add("@RolId", SqlDbType.Int)
                        .Value = usuario.RolId;

                    cn.Open();

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
