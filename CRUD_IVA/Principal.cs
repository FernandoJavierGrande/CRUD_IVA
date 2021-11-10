using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CRUD_IVA.clases;

namespace CRUD_IVA
{
    public partial class Principal : Form
    {
        double precioIngresado, precioFinal, iva = 1.0, precioEliminar, elim_PSinIva;
        string mensaje, titulo;
        int  id_prodElim;
        string nombreProdELim;
        bool bandera = false;

        GestionProductos gestion = new GestionProductos();
        Producto producto = new Producto();

     
        


        public Principal()
        {
            InitializeComponent();


            cmb_iva.DropDownStyle = ComboBoxStyle.DropDownList;
            cmb_Categoria.DropDownStyle = ComboBoxStyle.DropDownList;
            dg.AllowUserToAddRows = false;


            dg.DataSource = gestion.DT;
            dg.ReadOnly = true;
            dg.Columns[0].Width = 25;
            dg.Columns[1].Width = 110;
            dg.Columns[2].Width = 108;
            dg.Columns[3].Width = 45;
            dg.Columns[4].Width = 100;
            dg.Columns[5].Width = 100;



        }


        public bool CalcularIva()
        {

            string selectItem;
            
            mensaje = "!Valor no permitido¡";
            iva = 1.0;
            try
            {
                
                if (txt_Precio.Text.Contains(","))
                {
                    txt_Precio.Text = txt_Precio.Text.Replace(",",".");
                }
                precioIngresado = double.Parse(txt_Precio.Text.Trim(), CultureInfo.InvariantCulture);




                selectItem = cmb_iva.SelectedItem.ToString().Trim();
                switch (selectItem)
                {

                    case "21%":
                        iva = iva + 0.21;
                        break;
                    case "10.5%":
                        iva = iva + 0.105;
                        break;
                    case "27%":
                        iva = iva + 0.27;
                        break;
                    case "4%":
                        iva = iva + 0.04;
                        break;
                    default:
                        iva = iva + 0.21;
                        
                        break;
                }

                precioFinal = precioIngresado * iva;
                txt_PrecioFinal.Text = precioFinal.ToString();
                txt_PrecioFinal.BackColor = Color.LightGreen;
                txt_Precio.BackColor = Color.LightGreen;
                return true;
            }
            catch (Exception e)
            {
                if (txt_Precio.Text.Trim()=="")
                {
                    mensaje = "El campo está vacío";
                }
                txt_Precio.BackColor = Color.OrangeRed;
                MessageBox.Show(mensaje, "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                
                return false;
            }

            
        }

        private void Boton_Guardar_Click(object sender, EventArgs e)
        {
            bool validar_ok, iva_ok;

            
            validar_ok = ValidarCampos();

            
            if (validar_ok)
            {
                if (cmb_iva.SelectedIndex == -1)
                {
                    cmb_iva.SelectedIndex = 0;   

                }
                iva_ok = CalcularIva();

                if (iva_ok)
                {
                     
                    producto.NombreProducto = txt_Nombre.Text.Trim();
                    producto.CategoriaProducto = txt_Categoria.Text.Trim();
                    producto.TipoIva = cmb_iva.SelectedItem.ToString().Trim();
                    producto.PrecioBruto = precioIngresado;
                    producto.PrecioFinal = precioFinal;



                    bool res;
                    res = gestion.agregarProducto(producto);

                    if (res)
                    {
                        mensaje = "Se guardó correctamente";
                        titulo = "Guardado";
                    }
                    else
                    {
                        mensaje = "¡Error al guardar!\n Intente Nuevamente";
                        titulo = "Error";
                    }

                    MessageBox.Show(mensaje,titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    producto = new Producto();
                    Limpiar();
                }
            }
 

        }
        private void botonEliminar_Click(object sender, EventArgs e)
        {
            bool control = false;
            try
            {


                producto.Id = id_prodElim;


                control = gestion.eliminarProducto(producto);
                Console.WriteLine("id " + id_prodElim, " id prod" + producto.Id);

                if (!control)
                {
                    mensaje = "No se pudo Eliminar el producto: '" + nombreProdELim + "'.";
                    MessageBox.Show(mensaje, "No existe el producto", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    mensaje = "Se elimino correctamente \n el producto: '" + nombreProdELim + "'.";
                    MessageBox.Show(mensaje, "Eliminación exitosa ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                bandera = false;

                producto = new Producto();
                desbloquearEntradas(0);

            }
            catch (Exception)
            {
                MessageBox.Show("No se pudo Eliminar", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void botonCancelar_Click(object sender, EventArgs e)
        {
            desbloquearEntradas(0);
        }
        private void actualizarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dg.DataSource = gestion.DT;

        }

        private void botonModificar_Click(object sender, EventArgs e)
        {
            bool res_info, res_precio;
            res_info = ValidarCampos();
            if (res_info)
            {
                res_precio = CalcularIva();

                if (res_precio)
                {
                    try
                    {
                        producto = new Producto();

                        producto.Id = id_prodElim;
                        producto.NombreProducto = txt_Nombre.Text.Trim();
                        producto.CategoriaProducto = txt_Categoria.Text.Trim();
                        producto.TipoIva = cmb_iva.SelectedItem.ToString().Trim();
                        producto.PrecioBruto = precioIngresado;
                        producto.PrecioFinal = precioFinal;

                        if (gestion.modificarProducto(producto))
                        {

                            MessageBox.Show("Se modificó correctamente.", "Modificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            dg.DataSource = gestion.DT;
                            producto = new Producto();
                        }


                        desbloquearEntradas(0);
                    }
                    catch (Exception)
                    {

                        MessageBox.Show("Ocurrio un error \nal intentar realizar la modificacion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }


                }
            }


        }
        private void dg_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            string itemIva;
            string itemCategoria;
            if (bandera)
            {
                try
                {
                    id_prodElim = int.Parse(dg.CurrentRow.Cells[0].Value.ToString());
                    nombreProdELim = dg.CurrentRow.Cells[1].Value.ToString();
                    txt_Nombre.Text = nombreProdELim;
                    elim_PSinIva = Convert.ToDouble(dg.CurrentRow.Cells[4].Value);
                    precioEliminar = Convert.ToDouble(dg.CurrentRow.Cells[5].Value);

                    txt_PrecioFinal.Text = precioEliminar.ToString();
                    txt_Precio.Text = elim_PSinIva.ToString();
                    botonEliminar.Enabled = true;


                    if (botonModificar.Visible)
                    {
                        itemCategoria = dg.CurrentRow.Cells[2].Value.ToString();
                        txt_Categoria.Text = itemCategoria;
                        itemIva = dg.CurrentRow.Cells[3].Value.ToString();
                        botonEliminar.Enabled = false;
                        botonModificar.Enabled = true;
                        if (itemIva == "21%")
                        {
                            cmb_iva.SelectedIndex = 0;
                        }
                        else if (itemIva == "10.5%")
                        {
                            cmb_iva.SelectedIndex = 1;
                        }
                        else if (itemIva == "4%")
                        {
                            cmb_iva.SelectedIndex = 3;
                        }
                        else if (itemIva == "27%")
                        {
                            cmb_iva.SelectedIndex = 2;
                        }
                        else
                        {
                            cmb_iva.SelectedIndex = -1;
                        }

                        desbloquearEntradas(1);


                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + "Error por celda null");
                }

            }

        }
        private void BotonLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void eliminarUnProductoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (gestion.DT.Rows.Count == 0)
            {
                mensaje = "No hay registros guardados.";
                MessageBox.Show(mensaje, "Memoria vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                bandera = true;
                bloquearEntradas(1);
            }



        }
        private void modificarUnProductoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gestion.DT.Rows.Count == 0)
            {
                mensaje = "No hay registros a modificar.";
                MessageBox.Show(mensaje, "Memoria vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                bandera = true;
                bloquearEntradas(2);
            }

        }
        private void vaciarTablaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gestion.DT.Rows.Count == 0)
            {
                mensaje = "No hay registros a eliminar.";
                MessageBox.Show(mensaje, "Memoria vacía", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                DialogResult respuesta;
                respuesta = MessageBox.Show("Desea eliminar todos los registros", "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (respuesta == DialogResult.Yes)
                {
                    try
                    {
                        gestion.borrarTodo();

                        Limpiar();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Problemas vaciando la tabla" + ex);
                    }
                }
            }

        }
        public void Limpiar()
        {
            txt_Nombre.Text = "";
            txt_Categoria.Text = "";
            txt_Precio.Text = "";
            txt_PrecioFinal.Text = "";

            cmb_iva.SelectedIndex = -1;
            cmb_Categoria.SelectedIndex = -1;

            txt_Nombre.BackColor = Color.White;
            txt_Categoria.BackColor = Color.White;
            txt_Precio.BackColor = Color.White;
            txt_PrecioFinal.BackColor = Color.White;
        }


        


        private void cmb_iva_SelectionChangeCommitted(object sender, EventArgs e)
        {
             CalcularIva().ToString();
        }

        private void txt_Categoria_MouseClick(object sender, MouseEventArgs e)
        {
            if (txt_Categoria.BackColor == Color.OrangeRed)
            {
                txt_Categoria.BackColor = Color.White;
            }

        }
    

        public void desbloquearEntradas(int arg)
        {

            //labelEliminar.Visible = false;
            label_subtitulo.Text = "Cargar nuevo producto";
            txt_Categoria.ReadOnly =false;
            txt_Precio.ReadOnly = false;
            txt_Nombre.ReadOnly = false;


            cmb_Categoria.SelectedIndex = -1;
            
            cmb_Categoria.Enabled = true;

            Boton_Guardar.Enabled = true;
            BotonLimpiar.Enabled = true;
            botonEliminar.Visible = false;
            

            switch (arg)
            {
                case 0:     // habilita todos los componentes para cargar normalmente
                    bandera = false;
                    Limpiar();
                    botonCancelar.Visible = false;
                    botonModificar.Visible = false;
                    cmb_iva.Enabled = true;
                    label10.Visible = false;
                    cmb_iva.SelectedIndex = -1;

                    break;
                case 1:           // 1 habilita para modificar pero no el bton guardar p evitar otro regist nuevo no vacia
                    bandera = true;
                    Boton_Guardar.Enabled = false;

                    break;
                default:
                    
                    break;
            }

        }
        public void bloquearEntradas(int opciones)
        {
            Limpiar();
            txt_Nombre.BackColor = Color.LightGreen;
            txt_Precio.BackColor = Color.LightGreen;
            txt_PrecioFinal.BackColor = Color.LightGreen;
            txt_Categoria.ReadOnly = true;
            txt_Precio.ReadOnly = true;
            txt_Nombre.ReadOnly = true;
            cmb_iva.SelectedIndex = -1;
            cmb_Categoria.SelectedIndex = -1;

            Boton_Guardar.Enabled = false;
            botonCancelar.Visible = true;

            label10.Text = "Realice doble click sobre el producto\n que desea modificar/eliminar";
            label10.Visible = true;
            switch (opciones)
            {
                case 1:             // para eliminar
                    label_subtitulo.Text = "Producto a eliminar";
                    cmb_iva.Enabled = false;
                    cmb_Categoria.Enabled = false;
                    BotonLimpiar.Enabled = false;
                    botonModificar.Visible = false;
                    botonEliminar.Visible = true;
                    botonEliminar.Enabled = false;
                    break;
                case 2:         // para modificar
                    label_subtitulo.Text = "Producto a Modificar";
                    cmb_iva.Enabled = true;
                    cmb_Categoria.Enabled = true;
                    BotonLimpiar.Enabled = true;
                    botonModificar.Visible = true;
                    botonModificar.Enabled = false;
                    botonEliminar.Visible = false;
                    break;               
            } 

        }
   
  

        private void txt_Nombre_MouseClick(object sender, MouseEventArgs e)
        {
            if (txt_Nombre.BackColor == Color.OrangeRed)
            {
                txt_Nombre.BackColor = Color.White;
            }
        }

        private void txt_Precio_MouseClick(object sender, MouseEventArgs e)
        {
            if (txt_Precio.BackColor == Color.OrangeRed)
            {
                txt_Precio.BackColor = Color.White;
            }
        }

        private void cmb_Categoria_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txt_Categoria.Text = cmb_Categoria.SelectedItem.ToString().Trim();
            txt_Categoria.BackColor = Color.White;
        }

        public bool ValidarCampos()
        {
            try
            {
                if (txt_Nombre.Text.Trim() == "" || txt_Categoria.Text.Trim() == "")
                {
                    mensaje = "Se deben completar todos los campos";
                    if (txt_Nombre.Text.Trim() == "")
                    {
                        txt_Nombre.BackColor = Color.OrangeRed;
                    }
                    if (txt_Categoria.Text.Trim() == "")
                    {
                        txt_Categoria.BackColor = Color.OrangeRed;
                    }
                    MessageBox.Show(mensaje, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return false;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("error al comparar cadenas de los cmb");

            }

            return true;
        }
        
    }


}
