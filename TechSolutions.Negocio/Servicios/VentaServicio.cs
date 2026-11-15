
using System;
using System.Collections.Generic;
using TechSolutions.Datos.Repositorios;
using TechSolutions.Entidades.Ventas;


namespace TechSolutions.Negocio.Servicios
{
    public class VentaServicio
    {
        private readonly VentaDAO ventaDAO;

        public VentaServicio()
        {
            ventaDAO = new VentaDAO();
        }

        // 1. OBTENER HISTORIAL DE VENTAS
        public List<Venta> ObtenerTodas()
        {
            return ventaDAO.ObtenerTodas();
        }

        // 2. OBTENER VENTAS POR RANGO DE FECHAS
        public List<Venta> ObtenerPorFechas(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            if (fechaInicio.Date > fechaFin.Date)
                throw new InvalidOperationException(
                    "La fecha inicial no puede ser posterior a la fecha final.");

            return ventaDAO.ObtenerPorFechas(
                fechaInicio,
                fechaFin);
        }

        // 3. REGISTRAR UNA VENTA
        public int RegistrarVenta(Venta venta)
        {
            if (venta == null)
                throw new ArgumentNullException(nameof(venta));

            if (venta.UsuarioId <= 0)
                throw new InvalidOperationException(
                    "Debe existir un usuario válido para registrar la venta.");

            if (venta.Detalles == null ||
                venta.Detalles.Count == 0)
                throw new InvalidOperationException(
                    "Debe agregar al menos un producto a la venta.");

            decimal totalCalculado = 0;

            foreach (DetalleVenta detalle in venta.Detalles)
            {
                if (detalle == null)
                    throw new InvalidOperationException(
                        "Existe un detalle de venta vacío.");

                if (detalle.ProductoId <= 0)
                    throw new InvalidOperationException(
                        "Seleccione un producto válido.");

                if (detalle.Cantidad <= 0)
                    throw new InvalidOperationException(
                        "La cantidad debe ser mayor que cero.");

                if (detalle.PrecioUnitario <= 0)
                    throw new InvalidOperationException(
                        "El precio del producto debe ser mayor que cero.");

                detalle.Subtotal =
                    detalle.Cantidad * detalle.PrecioUnitario;

                totalCalculado += detalle.Subtotal;
            }

            if (totalCalculado <= 0)
                throw new InvalidOperationException(
                    "El total de la venta debe ser mayor que cero.");

            venta.Total = totalCalculado;

            // ENVIAR LA OPERACIÓN A LA CAPA DE DATOS
            return ventaDAO.RegistrarVenta(venta);
        }

        // 4. OBTENER DETALLES DE UNA VENTA
        public List<DetalleVenta> ObtenerDetalles(int ventaId)
        {
            if (ventaId <= 0)
                throw new InvalidOperationException(
                    "El número de venta no es válido.");

            return ventaDAO.ObtenerDetalles(ventaId);
        }
    }
}