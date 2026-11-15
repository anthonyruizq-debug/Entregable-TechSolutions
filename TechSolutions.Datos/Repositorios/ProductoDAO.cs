using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSolutions.Datos.Conexion;
using TechSolutions.Entidades.Productos;

namespace TechSolutions.Datos.Repositorios
{
    public class ProductoDAO
    {
        private readonly ConexionBD conexion =
            ConexionBD.ObtenerInstancia();

        // 1. LISTAR TODOS LOS PRODUCTOS
        public List<Producto> ObtenerTodos()
        {
            List<Producto> lista = new List<Producto>();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string sql = @"
                    SELECT Id, Nombre, Categoria, Precio, Stock
                    FROM Productos
                    ORDER BY Id DESC";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Producto producto = new Producto();

                            producto.Id = Convert.ToInt32(dr["Id"]);
                            producto.Nombre = dr["Nombre"].ToString();
                            producto.Categoria = dr["Categoria"].ToString();
                            producto.Precio = Convert.ToDecimal(dr["Precio"]);
                            producto.Stock = Convert.ToInt32(dr["Stock"]);

                            lista.Add(producto);
                        }
                    }
                }
            }

            return lista;
        }

        // 2. REGISTRAR PRODUCTO
        public void Registrar(Producto producto)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string sql = @"
                    INSERT INTO Productos
                    (Nombre, Categoria, Precio, Stock)
                    VALUES
                    (@Nombre, @Categoria, @Precio, @Stock)";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@Nombre",
                        SqlDbType.VarChar, 100).Value = producto.Nombre;

                    cmd.Parameters.Add("@Categoria",
                        SqlDbType.VarChar, 100).Value = producto.Categoria;

                    SqlParameter precio =
                        cmd.Parameters.Add("@Precio", SqlDbType.Decimal);
                    precio.Precision = 18;
                    precio.Scale = 2;
                    precio.Value = producto.Precio;

                    cmd.Parameters.Add("@Stock",
                        SqlDbType.Int).Value = producto.Stock;

                    cn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // 3. ACTUALIZAR PRODUCTO
        public void Actualizar(Producto producto)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string sql = @"
                    UPDATE Productos
                    SET Nombre = @Nombre,
                        Categoria = @Categoria,
                        Precio = @Precio,
                        Stock = @Stock
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@Id",
                        SqlDbType.Int).Value = producto.Id;

                    cmd.Parameters.Add("@Nombre",
                        SqlDbType.VarChar, 100).Value = producto.Nombre;

                    cmd.Parameters.Add("@Categoria",
                        SqlDbType.VarChar, 100).Value = producto.Categoria;

                    SqlParameter precio =
                        cmd.Parameters.Add("@Precio", SqlDbType.Decimal);
                    precio.Precision = 18;
                    precio.Scale = 2;
                    precio.Value = producto.Precio;

                    cmd.Parameters.Add("@Stock",
                        SqlDbType.Int).Value = producto.Stock;

                    cn.Open();

                    if (cmd.ExecuteNonQuery() == 0)
                        throw new Exception("No se encontró el producto.");
                }
            }
        }

        // 4. ELIMINAR PRODUCTO
        public void Eliminar(int id)
        {
            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string sql = @"
                    DELETE FROM Productos
                    WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add("@Id",
                        SqlDbType.Int).Value = id;

                    cn.Open();

                    if (cmd.ExecuteNonQuery() == 0)
                        throw new Exception("No se encontró el producto.");
                }
            }
        }
    }
}