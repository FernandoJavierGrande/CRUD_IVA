using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CRUD_IVA.clases
{
    class GestionProductos
    {
        public DataTable DT { get; set; } = new DataTable();
        public int UltimoId { get; set; } = 0;



        public GestionProductos()
        {
            
            DT.TableName = "Productos cargados";
            
            DT.Columns.Add("Id");
            DT.Columns.Add("Nombre");
            DT.Columns.Add("Categoria");
            DT.Columns.Add("Tipo iva");
            DT.Columns.Add("Precio bruto");
            DT.Columns.Add("Precio final");
            
            //DT.Columns.

            leerXml();
        }
        public void leerXml()
        {
            if (System.IO.File.Exists("Productos.xml"))
            {
                DT.ReadXml("Productos.xml");
                UltimoId = 0;
                for (int i = 0; i < DT.Rows.Count; i++)
                {
                    if (int.Parse(DT.Rows[i]["Id"].ToString()) > UltimoId)
                    {
                        UltimoId = int.Parse(DT.Rows[i]["Id"].ToString());
                    }
                }
            }
        }


        public bool agregarProducto(Producto prod)
        {
            bool res = false;
            if (prod.Id == 0)
            {
                UltimoId++;
                prod.Id = UltimoId;

                DT.Rows.Add();
                int filaNueva = DT.Rows.Count - 1;

                DT.Rows[filaNueva]["Id"] = prod.Id.ToString();
                DT.Rows[filaNueva]["Nombre"] = prod.NombreProducto;
                DT.Rows[filaNueva]["Categoria"] = prod.CategoriaProducto;
                DT.Rows[filaNueva]["Tipo iva"] = prod.TipoIva;
                DT.Rows[filaNueva]["Precio bruto"] = prod.PrecioBruto.ToString();
                DT.Rows[filaNueva]["Precio final"] = prod.PrecioFinal.ToString(); 


                DT.WriteXml("Productos.xml");
                res = true;
            }
            return res;
        }
        public bool eliminarProducto(Producto prod)
        {
            bool res = false;

            for (int fila = 0; fila < DT.Rows.Count; fila++)
            {
                
                if (Convert.ToInt32(DT.Rows[fila]["Id"]) == prod.Id)
                {
                    
                    DT.Rows[fila].Delete();
                    DT.WriteXml("Productos.xml");
                    res = true;
                    break;
                }
            }



            return res;
        }
        public bool modificarProducto(Producto prod)
        {
            bool res = false;
            if (prod.Id != 0)
            {
                for (int fila = 0; fila < DT.Rows.Count; fila++)
                {

                    if (Convert.ToInt32(DT.Rows[fila]["Id"]) == prod.Id)
                    {
                        


                        DT.Rows[fila]["Nombre"] = prod.NombreProducto;
                        DT.Rows[fila]["Categoria"] = prod.CategoriaProducto;
                        DT.Rows[fila]["Tipo iva"] = prod.TipoIva;
                        DT.Rows[fila]["Precio bruto"] = prod.PrecioBruto.ToString();
                        DT.Rows[fila]["Precio final"] = prod.PrecioFinal.ToString();

                        DT.WriteXml("Productos.xml");
                        res = true;

                        break;
                    }
                }

            }
            return res;
        }
        public void borrarTodo()
        { 
            DT.Clear();
            DT.WriteXml("Productos.xml");
            UltimoId = 0;
            
        }
        
    }
}
