
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace TechSolutions.Negocio.Seguridad
{
    public class SeguridadPassword
    {
        private const int TamañoSalt = 32;
        private const int TamañoHash = 64;
        private const int Iteraciones = 100000;

        // Generar un Salt aleatorio
        public static byte[] GenerarSalt()
        {
            byte[] salt = new byte[TamañoSalt];

            using (RandomNumberGenerator generador =
                RandomNumberGenerator.Create())
            {
                generador.GetBytes(salt);
            }

            return salt;
        }

        // Generar Hash de la contraseña
        public static byte[] GenerarHash(string contraseña, byte[] salt)
        {
            using (Rfc2898DeriveBytes derivador =
                new Rfc2898DeriveBytes(
                    contraseña,
                    salt,
                    Iteraciones,
                    HashAlgorithmName.SHA256))
            {
                return derivador.GetBytes(TamañoHash);
            }
        }

        // Verificar contraseña
        public static bool VerificarPassword(
            string contraseña,
            byte[] salt,
            byte[] hashOriginal)
        {
            byte[] hashCalculado = GenerarHash(contraseña, salt);

            if (hashOriginal == null ||
                hashCalculado.Length != hashOriginal.Length)
            {
                return false;
            }

            int diferencia = 0;

            for (int i = 0; i < hashOriginal.Length; i++)
            {
                diferencia |= hashCalculado[i] ^ hashOriginal[i];
            }

            return diferencia == 0;
        }
    }
}
