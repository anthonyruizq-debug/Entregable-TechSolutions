using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechSolutions.Datos.Repositorios;
using TechSolutions.Entidades.Productos;

namespace TechSolutions.Negocio.Servicios
{
    public class ProductoServicio
    {
        private readonly ProductoDAO productoDAO;

        public ProductoServicio()
        {
            productoDAO = new ProductoDAO();
        }

        // OBTENER TODOS LOS PRODUCTOS
        public List<Producto> ObtenerTodos()
        {
            return productoDAO.ObtenerTodos();
        }

        // REGISTRAR PRODUCTO
        public void Registrar(Producto producto)
        {
            ValidarProducto(producto);

            productoDAO.Registrar(producto);
        }

        // ACTUALIZAR PRODUCTO
        public void Actualizar(Producto producto)
        {
            if (producto == null || producto.Id <= 0)
                throw new Exception("Seleccione un producto válido.");

            ValidarProducto(producto);

            productoDAO.Actualizar(producto);
        }

        // ELIMINAR PRODUCTO
        public void Eliminar(int id)
        {
            if (id <= 0)
                throw new Exception("El producto seleccionado no es válido.");

            productoDAO.Eliminar(id);
        }

        // VALIDAR DATOS
        private void ValidarProducto(Producto producto)
        {
            if (producto == null)
                throw new Exception("Los datos del producto son obligatorios.");

            if (string.IsNullOrWhiteSpace(producto.Nombre))
                throw new Exception("Ingrese el nombre del producto.");

            if (string.IsNullOrWhiteSpace(producto.Categoria))
                throw new Exception("Ingrese la categoría del producto.");

            if (producto.Precio <= 0)
                throw new Exception("El precio debe ser mayor que cero.");

            if (producto.Stock < 0)
                throw new Exception("El stock no puede ser negativo.");
        }
    }
}