﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MySql.Data.MySqlClient;

namespace BDatos_API
{
    /// <summary>
    /// Lógica de interacción para Verificacion.xaml
    /// </summary>
    public partial class Nuevo_usuario : MetroWindow
    {
        private bool boolcontrol=false;
        private bool contrasena_igual = false;
        Metodos_comunes Controles;
        Metodos_bd metodos_bd;
        private string modo = "crear";
       
        public Nuevo_usuario()
        {
            InitializeComponent();
            Controles = new Metodos_comunes();
            metodos_bd = new Metodos_bd();
            Controles.Limpiar_lista();

            Controles.campos.Add(caja_texto_usuario);
            Controles.campos.Add(caja_contrasena);
            Controles.campos.Add(caja_contrasena1);
            Controles.campos.Add(Combobox_tipo);
            Controles.Inicial.Focus();
            cargar_datagrid();
        }

        #region datagrid
        private void cargar_datagrid()
        {
            metodos_bd.popular_tabla(tabla_Principal, "id_usuario,nombre_Usuario,tipo_Usuario", "tabla_usuario", "cargarUsuarios");
        }

        private void llenarCampos(int Id)
        {
            int local = 0;
            string SQL = string.Format("SELECT * FROM tabla_usuario WHERE id_usuario = '{0}'", Id);
            MySqlDataReader reader = ConectorDB.Consultas(SQL);
            while (reader.Read())
            {
                Usuario2.ID_USUARIO = reader.GetInt32(0);
                Usuario2.USUARIO = reader.GetString(1);
                Usuario2.CONTRASEÑA = reader.GetString(2);
                Usuario2.TIPO_USUARIO = reader.GetInt16(3);
            }
            caja_texto_usuario.Text = Usuario2.USUARIO;
            caja_contrasena.Password = Usuario2.CONTRASEÑA;
            caja_contrasena1.Password = Usuario2.CONTRASEÑA;
            local = Usuario2.TIPO_USUARIO;
            switch (local)
            {
                case 1: Combobox_tipo.SelectedIndex = 0;
                    break;
                case 2:
                    Combobox_tipo.SelectedIndex = 1;
                    break;
                case 3:
                    Combobox_tipo.SelectedIndex = 2;
                    break;
            }
           
            modo = "editar";
            boton_borrar.Visibility = Visibility.Visible;
            boton_guardar.Content = "Editar";
        }//fin metodo llenarCampos

        private void editar()
        {
            string SQL = string.Format("UPDATE tabla_usuario SET nombre_Usuario='{0}', contraseña_Usuario='{1}', tipo_Usuario='{2}' WHERE id_usuario='{3}'",
                Usuario2.USUARIO,Usuario2.CONTRASEÑA,(Usuario2.TIPO_USUARIO+1), Usuario2.ID_USUARIO);
            ConectorDB.Inyectar(SQL);
        }

        private void DataGridCell_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (tabla_Principal.SelectedItems.Count == 1)
            {
                int id = 0;
                try
                {
                    DataRowView row = (DataRowView)tabla_Principal.SelectedItems[0];
                    id = Convert.ToInt32(row[0]);
                }
                catch (Exception)
                {
                }
                abrir();
                llenarCampos(id);
                ConectorDB.CerrarConexion();
            }

        }
        #endregion

        #region eventos_controles

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Navegacion.NavegarAtras();
            Controles.Limpiar_lista();
        }

        private void Caja_texto_usuario_KeyDown(object sender, KeyEventArgs e)
        {
            Controles.Marcar_control(caja_texto_usuario, true);
            if (e.Key == Key.Enter) caja_contrasena.Focus();
        }

        private void Caja_contrasena_KeyDown(object sender, KeyEventArgs e)
        {
            Controles.Marcar_control(caja_contrasena, true);
            if (e.Key == Key.Enter) caja_contrasena1.Focus();
        }

        private void Caja_contrasena1_KeyDown(object sender, KeyEventArgs e)
        {
            Controles.Marcar_control(caja_contrasena1, true);
            if (e.Key == Key.Enter) Combobox_tipo.Focus();
        }

        private void Combobox_tipo_KeyDown(object sender, KeyEventArgs e)
        {
            Controles.Marcar_control(Combobox_tipo, true);
            if (Combobox_tipo.IsDropDownOpen==true)
            {
                if (Combobox_tipo.SelectedItem != null)
                {
                    if (e.Key == Key.Enter) boton_guardar.Focus();
                }
            }
            else
            {
                Combobox_tipo.IsDropDownOpen = true;
            }
        }

        private void Combobox_tipo_DropDownClosed(object sender, EventArgs e)
        {
            Controles.Marcar_control(Combobox_tipo, true);
            if (Combobox_tipo.SelectedItem != null)
                boton_guardar.Focus();
            else
                Combobox_tipo.Focus();
       
        }

        private void Boton_guardar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter & boolcontrol == false) botoningresar();
           
        }

        private void Boton_guardar_Click(object sender, RoutedEventArgs e)
        {
            if (boolcontrol == false) botoningresar();
        }

        private void Boton_borrar_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Boton_borrar_KeyDown(object sender, KeyEventArgs e)
        {

        }
        #endregion

        #region mensajes_y_apuntadores
        public async System.Threading.Tasks.Task verificar_bdAsync()
        {
            var res = await this.ShowMessageAsync("Error base de datos", "No se conecto servidor. Contactar administrador" +
                "\nDatos registrados: \nServidor: " + ConectorDB.server + "\nBase de datos: " + ConectorDB.database, MessageDialogStyle.Affirmative);
            if (res == MessageDialogResult.Affirmative)
            {
                this.Close();
            }
        }

        private async System.Threading.Tasks.Task comandoAsync(string a, string b)
        {
            var res = await this.ShowMessageAsync(a, b, MessageDialogStyle.Affirmative);
            if (res == MessageDialogResult.Affirmative)
            {
                
            }
        }

        private async void confirmarAsync(string a, string b)
        {
            var res = await this.ShowMessageAsync(a, b, MessageDialogStyle.AffirmativeAndNegative);
            if (res == MessageDialogResult.Affirmative)
            {
                abrir();
                editar();
                await this.ShowMessageAsync("Editar usuario", "Usuario modificado", MessageDialogStyle.Affirmative);
            }
            ConectorDB.CerrarConexion();
            cargar_datagrid();
            limpiar();
        }

        private void limpiar()
        {
            caja_texto_usuario.Clear();
            caja_contrasena.Clear();
            caja_contrasena1.Clear();
            Combobox_tipo.SelectedItem = null;
            caja_contrasena.Background = Brushes.White;
            caja_contrasena1.Background = Brushes.White;
            modo = "crear";
            boton_borrar.Visibility = Visibility.Hidden;
            boolcontrol =Controles.Seleccionar_control(false);
        }

        private void Caja_contrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            verificar_contrasena();
        }

        private void Caja_contrasena1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            verificar_contrasena();
        }

        void verificar_contrasena()
        {
            if (caja_contrasena.Password != caja_contrasena1.Password)
            {
                caja_contrasena1.Background = Brushes.LightYellow;
                caja_contrasena.Background = Brushes.LightYellow;
                contrasena_igual = false;
            }
            else
            {
                caja_contrasena1.Background = Brushes.LightGreen;
                caja_contrasena.Background = Brushes.LightGreen;
                contrasena_igual = true;
            }
        }

        #endregion

        #region metodos_base_datos

        private async void botoningresar()
        {
            boolcontrol = Controles.Seleccionar_control(false);

            if (boolcontrol)
            {
                abrir();
                boolcontrol = false;
                bool local = Verificar();
                ConectorDB.CerrarConexion();
                if (local)
                {
                    var a = comandoAsync("Nuevo usuario","Nombre no disponible");
                }
                else
                {
                    if (contrasena_igual)
                    {
                        abrir();
                        switch (modo)
                        {
                            case "crear":
                                Guardar();
                                var c = comandoAsync("Nuevo usuario", "Usuario creado");
                                break;
                            case "editar":
                                Usuario2.USUARIO = caja_texto_usuario.Text.Trim();
                                Usuario2.CONTRASEÑA = caja_contrasena.Password.Trim();
                                Usuario2.TIPO_USUARIO = Combobox_tipo.SelectedIndex;
                                confirmarAsync("Editar usuario", "¿Guardar cambios?");
                                break;
                        }
                        ConectorDB.CerrarConexion();
                        cargar_datagrid();
                        limpiar();
                    }
                    else
                    {
                        var b = comandoAsync("Nuevo usuario", "Las contraseñas no coinciden");
                    }
                }
                caja_texto_usuario.Focus();
            }
            else
            {
                await this.ShowMessageAsync("Nuevo usuario", "Campos en blanco");
            }
        }

        public void abrir()
        {
            bool local = ConectorDB.ObtenerConexion();
            if (local == false)
            {
                var task = verificar_bdAsync();
            }
        }

        private void Guardar()
        {
            /*Creamos la sentencia SQL que indicara correctamente como guardar un nuevo usuario en la tabla USUARIOS*/
            string SQL = string.Format("Insert into tabla_usuario (nombre_Usuario, contraseña_Usuario, tipo_Usuario)" +
                "values ('{0}','{1}','{2}')",
               caja_texto_usuario.Text.Trim(),
               caja_contrasena.Password.ToString(),
               Combobox_tipo.SelectedIndex+1);
            ConectorDB.Inyectar(SQL);
        }

        private bool Verificar()
        {
            bool retorno = false;
            /*Creamos la sentencia SQL que podra realizar la consulta que nesesitamos*/
            string SQL = String.Format("SELECT id_usuario FROM tabla_usuario where nombre_Usuario ='{0}'",caja_texto_usuario.Text.Trim());
            MySqlDataReader reader = ConectorDB.Consultas(SQL);
            while (reader.Read())
            {
                /*Si existe el usuario mandara un id_usuario mayor a 0*/
                Usuario.ID_USUARIO = reader.GetInt16(0);
                if (Usuario.ID_USUARIO > 0)
                {
                    retorno = true;
                }
            }
            if (modo == "editar")
            {
                if (Usuario.ID_USUARIO == Usuario2.ID_USUARIO)
                {
                    retorno = false;
                }
            }
            reader.Dispose();
            return retorno;
        }//fin metodo Verificar

        #endregion
    }
}
