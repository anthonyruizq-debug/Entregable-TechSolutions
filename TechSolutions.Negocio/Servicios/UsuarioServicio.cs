
using System;
using System.Collections.Generic;
using TechSolutions.Datos.Repositorios;
using TechSolutions.Entidades.Usuarios;
using TechSolutions.Entidades.Seguridad;
using TechSolutions.Negocio.Seguridad;

namespace TechSolutions.Negocio.Servicios
{
    public class UsuarioServicio
    {
        private readonly UsuarioDAO usuarioDAO;

        public UsuarioServicio()
        {
            usuarioDAO = new UsuarioDAO();
        }

        // Registrar usuario
        public bool RegistrarUsuario(
            string nombre,
            string nombreUsuario,
            string contraseña,
            int rolId)
        {
            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(nombreUsuario) ||
                string.IsNullOrWhiteSpace(contraseña))
            {
                throw new Exception("Todos los campos son obligatorios.");
            }

            if (usuarioDAO.ExisteUsuario(nombreUsuario))
            {
                throw new Exception("El nombre de usuario ya existe.");
            }

            byte[] salt = SeguridadPassword.GenerarSalt();

            byte[] hash =
                SeguridadPassword.GenerarHash(contraseña, salt);

            Usuario usuario = new Usuario
            {
                Nombre = nombre,
                UsuarioNombre = nombreUsuario,
                Salt = salt,
                Hash = hash,
                RolId = rolId
            };

            return usuarioDAO.RegistrarUsuario(usuario);
        }

        // Iniciar sesión
        public ResultadoLogin IniciarSesion(
            string nombreUsuario,
            string contraseña)
        {
            ResultadoLogin resultado = new ResultadoLogin();

            Usuario usuario =
                usuarioDAO.ObtenerPorUsuario(nombreUsuario);

            if (usuario == null)
            {
                resultado.Exitoso = false;
                resultado.Mensaje = "Usuario o contraseña incorrectos.";
                return resultado;
            }

            if (usuario.Bloqueado)
            {
                resultado.Exitoso = false;
                resultado.UsuarioBloqueado = true;
                resultado.BloqueoPermanente = usuario.BloqueoPermanente;

                resultado.Mensaje = usuario.BloqueoPermanente
                    ? "La cuenta está bloqueada permanentemente."
                    : "La cuenta está bloqueada. Solicite al administrador que la desbloquee.";

                return resultado;
            }

            bool contraseñaCorrecta =
                SeguridadPassword.VerificarPassword(
                    contraseña,
                    usuario.Salt,
                    usuario.Hash);

            if (!contraseñaCorrecta)
            {
                bool bloqueado =
                    usuarioDAO.RegistrarIntentoFallido(nombreUsuario);

                Usuario usuarioActualizado =
                    usuarioDAO.ObtenerPorUsuario(nombreUsuario);

                resultado.Exitoso = false;
                resultado.UsuarioBloqueado = bloqueado;

                resultado.BloqueoPermanente =
                    usuarioActualizado != null &&
                    usuarioActualizado.BloqueoPermanente;

                if (resultado.BloqueoPermanente)
                {
                    resultado.Mensaje =
                        "Cuenta bloqueada permanentemente. No puede ser desbloqueada.";
                }
                else if (bloqueado)
                {
                    resultado.Mensaje =
                        "Cuenta bloqueada por superar los 3 intentos fallidos.";
                }
                else
                {
                    resultado.Mensaje =
                        "Contraseña incorrecta. Intentos restantes: " +
                        (2 - usuario.IntentosFallidos);
                }

                return resultado;
            }

            usuarioDAO.ReiniciarIntentosFallidos(nombreUsuario);

            resultado.Exitoso = true;
            resultado.Mensaje = "Inicio de sesión correcto.";

            resultado.UsuarioId = usuario.Id;
            resultado.RolId = usuario.RolId;
            resultado.Nombre = usuario.Nombre;

            return resultado;
        }

        // Obtener usuarios bloqueados
        public List<Usuario> ObtenerUsuariosBloqueados()
        {
            return usuarioDAO.ObtenerUsuariosBloqueados();
        }

        // Obtener todos los usuarios
        public List<Usuario> ObtenerTodosLosUsuarios()
        {
            return usuarioDAO.ObtenerTodosLosUsuarios();
        }

        // Desbloquear una cuenta desde la administración
        public bool DesbloquearUsuario(int idUsuario)
        {
            return usuarioDAO.DesbloquearUsuario(idUsuario);
        }

        // Editar o actualizar un usuario
        public bool ActualizarUsuario(
            int id,
            string nombre,
            string nombreUsuario,
            int rolId)
        {
            if (id <= 0)
            {
                throw new Exception("El usuario seleccionado no es válido.");
            }

            if (string.IsNullOrWhiteSpace(nombre) ||
                string.IsNullOrWhiteSpace(nombreUsuario))
            {
                throw new Exception("Todos los campos son obligatorios.");
            }

            Usuario usuarioExistente =
                usuarioDAO.ObtenerPorUsuario(nombreUsuario);

            if (usuarioExistente != null &&
                usuarioExistente.Id != id)
            {
                throw new Exception(
                    "El nombre de usuario ya está siendo utilizado.");
            }

            Usuario usuario = new Usuario
            {
                Id = id,
                Nombre = nombre.Trim(),
                UsuarioNombre = nombreUsuario.Trim(),
                RolId = rolId
            };

            return usuarioDAO.ActualizarUsuario(usuario);
        }
    }
}
