
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using TechSolutions.Datos.Conexion;
using TechSolutions.Entidades.Ventas;

namespace TechSolutions.Datos.Repositorios
{
    public class VentaDAO
    {
        // CONEXION A LA BASE DE DATOS
        private readonly ConexionBD conexion =
            ConexionBD.ObtenerInstancia();

        // 1. OBTENER TODAS LAS VENTAS
        public List<Venta> ObtenerTodas()
        {
            List<Venta> lista = new List<Venta>();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string sql = @"
                    SELECT
                        V.Id,
                        V.UsuarioId,
                        U.Nombre AS NombreUsuario,
                        V.Fecha,
                        V.Total
                    FROM Ventas V
                    LEFT JOIN Usuarios U
                        ON V.UsuarioId = U.Id
                    ORDER BY V.Id DESC";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Venta venta = new Venta();

                            venta.Id = Convert.ToInt32(dr["Id"]);
                            venta.UsuarioId =
                                Convert.ToInt32(dr["UsuarioId"]);

                            venta.NombreUsuario =
                                dr["NombreUsuario"] == DBNull.Value
                                ? "Usuario desconocido"
                                : dr["NombreUsuario"].ToString();

                            venta.Fecha =
                                Convert.ToDateTime(dr["Fecha"]);

                            venta.Total =
                                Convert.ToDecimal(dr["Total"]);

                            lista.Add(venta);
                        }
                    }
                }
            }

            return lista;
        }

        // 2. OBTENER VENTAS POR RANGO DE FECHAS
        public List<Venta> ObtenerPorFechas(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            List<Venta> lista = new List<Venta>();

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string sql = @"
                    SELECT
                        V.Id,
                        V.UsuarioId,
                        U.Nombre AS NombreUsuario,
                        V.Fecha,
                        V.Total
                    FROM Ventas V
                    LEFT JOIN Usuarios U
                        ON V.UsuarioId = U.Id
                    WHERE V.Fecha >= @FechaInicio
                      AND V.Fecha < @FechaFinExclusiva
                    ORDER BY V.Id DESC";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add(
                        "@FechaInicio",
                        SqlDbType.DateTime).Value = fechaInicio.Date;

                    cmd.Parameters.Add(
                        "@FechaFinExclusiva",
                        SqlDbType.DateTime).Value =
                        fechaFin.Date.AddDays(1);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Venta venta = new Venta();

                            venta.Id = Convert.ToInt32(dr["Id"]);

                            venta.UsuarioId =
                                Convert.ToInt32(dr["UsuarioId"]);

                            venta.NombreUsuario =
                                dr["NombreUsuario"] == DBNull.Value
                                ? "Usuario desconocido"
                                : dr["NombreUsuario"].ToString();

                            venta.Fecha =
                                Convert.ToDateTime(dr["Fecha"]);

                            venta.Total =
                                Convert.ToDecimal(dr["Total"]);

                            lista.Add(venta);
                        }
                    }
                }
            }

            return lista;
        }

        // 3. REGISTRAR UNA VENTA CON TRANSACCION SQL
        public int RegistrarVenta(Venta venta)
        {
            if (venta == null)
                throw new ArgumentNullException(nameof(venta));

            if (venta.UsuarioId <= 0)
                throw new InvalidOperationException(
                    "El usuario de la venta no es válido.");

            if (venta.Detalles == null ||
                venta.Detalles.Count == 0)
                throw new InvalidOperationException(
                    "La venta no contiene productos.");

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                cn.Open();

                using (SqlTransaction transaccion =
                    cn.BeginTransaction())
                {
                    try
                    {
                        decimal total = 0;

                        // VALIDAR Y CALCULAR LOS DETALLES
                        foreach (DetalleVenta detalle in venta.Detalles)
                        {
                            if (detalle == null ||
                                detalle.ProductoId <= 0 ||
                                detalle.Cantidad <= 0 ||
                                detalle.PrecioUnitario <= 0)
                            {
                                throw new InvalidOperationException(
                                    "Los datos de un producto no son válidos.");
                            }

                            detalle.Subtotal =
                                detalle.Cantidad *
                                detalle.PrecioUnitario;

                            total += detalle.Subtotal;
                        }

                        if (total <= 0)
                            throw new InvalidOperationException(
                                "El total de la venta no es válido.");

                        venta.Total = total;

                        // 1. INSERTAR CABECERA DE LA VENTA
                        string sqlVenta = @"
                            INSERT INTO Ventas
                                (UsuarioId, Fecha, Total)
                            VALUES
                                (@UsuarioId, @Fecha, @Total);

                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        int ventaId;

                        using (SqlCommand cmd = new SqlCommand(
                            sqlVenta, cn, transaccion))
                        {
                            cmd.Parameters.Add(
                                "@UsuarioId",
                                SqlDbType.Int).Value = venta.UsuarioId;

                            cmd.Parameters.Add(
                                "@Fecha",
                                SqlDbType.DateTime).Value = DateTime.Now;

                            SqlParameter parametroTotal =
                                cmd.Parameters.Add(
                                    "@Total", SqlDbType.Decimal);

                            parametroTotal.Precision = 18;
                            parametroTotal.Scale = 2;
                            parametroTotal.Value = total;

                            object resultado = cmd.ExecuteScalar();

                            if (resultado == null ||
                                resultado == DBNull.Value)
                            {
                                throw new InvalidOperationException(
                                    "No se pudo generar el número de venta.");
                            }

                            ventaId = Convert.ToInt32(resultado);
                        }

                        // 2. REGISTRAR DETALLES Y DESCONTAR STOCK
                        foreach (DetalleVenta detalle in venta.Detalles)
                        {
                            // DESCONTAR STOCK SI EXISTEN UNIDADES
                            string sqlStock = @"
                                UPDATE Productos
                                SET Stock = Stock - @Cantidad
                                WHERE Id = @ProductoId
                                AND Stock >= @Cantidad";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlStock, cn, transaccion))
                            {
                                cmd.Parameters.Add(
                                    "@Cantidad",
                                    SqlDbType.Int).Value =
                                    detalle.Cantidad;

                                cmd.Parameters.Add(
                                    "@ProductoId",
                                    SqlDbType.Int).Value =
                                    detalle.ProductoId;

                                int filas = cmd.ExecuteNonQuery();

                                if (filas != 1)
                                {
                                    throw new InvalidOperationException(
                                        "No hay stock suficiente o el producto no existe. ID: "
                                        + detalle.ProductoId);
                                }
                            }

                            // INSERTAR DETALLE DE LA VENTA
                            string sqlDetalle = @"
                                INSERT INTO DetalleVentas
                                (
                                    VentaId,
                                    ProductoId,
                                    Cantidad,
                                    PrecioUnitario,
                                    Subtotal
                                )
                                VALUES
                                (
                                    @VentaId,
                                    @ProductoId,
                                    @Cantidad,
                                    @PrecioUnitario,
                                    @Subtotal
                                )";

                            using (SqlCommand cmd = new SqlCommand(
                                sqlDetalle, cn, transaccion))
                            {
                                cmd.Parameters.Add(
                                    "@VentaId",
                                    SqlDbType.Int).Value = ventaId;

                                cmd.Parameters.Add(
                                    "@ProductoId",
                                    SqlDbType.Int).Value =
                                    detalle.ProductoId;

                                cmd.Parameters.Add(
                                    "@Cantidad",
                                    SqlDbType.Int).Value =
                                    detalle.Cantidad;

                                SqlParameter precio =
                                    cmd.Parameters.Add(
                                        "@PrecioUnitario",
                                        SqlDbType.Decimal);

                                precio.Precision = 18;
                                precio.Scale = 2;
                                precio.Value =
                                    detalle.PrecioUnitario;

                                SqlParameter subtotal =
                                    cmd.Parameters.Add(
                                        "@Subtotal",
                                        SqlDbType.Decimal);

                                subtotal.Precision = 18;
                                subtotal.Scale = 2;
                                subtotal.Value =
                                    detalle.Subtotal;

                                cmd.ExecuteNonQuery();
                            }
                        }

                        // 3. CONFIRMAR TODAS LAS OPERACIONES
                        transaccion.Commit();

                        return ventaId;
                    }
                    catch
                    {
                        // 4. REVERTIR SI OCURRE ALGUN ERROR
                        // Se conserva la excepcion original.
                        try
                        {
                            transaccion.Rollback();
                        }
                        catch
                        {
                            // No ocultar el error que produjo
                            // el fallo de la transaccion.
                        }

                        throw;
                    }
                }
            }
        }

        // 4. OBTENER DETALLES DE UNA VENTA
        public List<DetalleVenta> ObtenerDetalles(int ventaId)
        {
            List<DetalleVenta> lista =
                new List<DetalleVenta>();

            if (ventaId <= 0)
                throw new InvalidOperationException(
                    "El número de venta no es válido.");

            using (SqlConnection cn = conexion.ObtenerConexion())
            {
                string sql = @"
                    SELECT
                        D.Id,
                        D.VentaId,
                        D.ProductoId,
                        P.Nombre AS NombreProducto,
                        D.Cantidad,
                        D.PrecioUnitario,
                        D.Subtotal
                    FROM DetalleVentas D
                    INNER JOIN Productos P
                        ON D.ProductoId = P.Id
                    WHERE D.VentaId = @VentaId
                    ORDER BY D.Id";

                using (SqlCommand cmd = new SqlCommand(sql, cn))
                {
                    cmd.Parameters.Add(
                        "@VentaId",
                        SqlDbType.Int).Value = ventaId;

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            DetalleVenta detalle =
                                new DetalleVenta();

                            detalle.Id =
                                Convert.ToInt32(dr["Id"]);

                            detalle.VentaId =
                                Convert.ToInt32(dr["VentaId"]);

                            detalle.ProductoId =
                                Convert.ToInt32(dr["ProductoId"]);

                            detalle.NombreProducto =
                                dr["NombreProducto"].ToString();

                            detalle.Cantidad =
                                Convert.ToInt32(dr["Cantidad"]);

                            detalle.PrecioUnitario =
                                Convert.ToDecimal(
                                    dr["PrecioUnitario"]);

                            detalle.Subtotal =
                                Convert.ToDecimal(
                                    dr["Subtotal"]);

                            lista.Add(detalle);
                        }
                    }
                }
            }

            return lista;
        }
    }
}