﻿/*
 * Created by SharpDevelop.
 * User: Yeu
 * Date: 06/09/2018
 * Time: 11:07 a. m.
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Notepad_
{
    /// <summary>
    /// Description of MainForm.
    /// </summary>
    public partial class Notepad_form : Form
    {
        public string Archivo_Actual;
        public Configuration Config;
        public class Configuration
        {
            //public System.Text.Encoding Encoding { get; set; }
            public Font ActualFont { get; set; }
            public Color ColorEditor { get; set; }
            public Boolean GuardadoAutomaticoStatus { get; set; }
            public Size TamañoForm { get; set; }
            public List<String> ArchivosRecientes = new List<String>();

            public static Configuration GetConfiguration()
            {
                inicio:
                if (System.IO.File.Exists("config.json"))
                {
                    var dataraw = System.IO.File.ReadAllText("config.json");
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<Configuration>(dataraw);
                }

                else
                {
                    var configTemp = new Configuration();

                    configTemp.TamañoForm = new Size(776, 417);
                    configTemp.ColorEditor = Color.White;
                    configTemp.GuardadoAutomaticoStatus = false;
                    // configTemp.Encoding = System.Text.Encoding.Default;
                    configTemp.ActualFont = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    configTemp.Save();

                    goto inicio;
                }

            }
            public void Save()
            {

                var dataraw = Newtonsoft.Json.JsonConvert.SerializeObject(this);
                System.IO.File.WriteAllText("config.json", dataraw);
            }

        }
        public Notepad_form()
        {
            //
            // The InitializeComponent() call is required for Windows Forms designer support.
            //

            InitializeComponent();
            this.Config = Configuration.GetConfiguration();
            this.ApplyConfig();
            //
            // TODO: Add constructor code after the InitializeComponent() call.
            //
        }

        private void ApplyConfig()
        {
            this.Size = Config.TamañoForm;
            this.editor.Font = Config.ActualFont;
            this.guardarToolStripMenuItem.Checked = Config.GuardadoAutomaticoStatus;
            this.editor.BackColor = Config.ColorEditor;
            foreach(var item in this.Config.ArchivosRecientes)
            {
                recientesToolStripMenuItem.DropDownItems.Add(item);
            }
            
        }

        private void abrirArchivoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AbrirArchivo();
        }
        public void AbrirArchivo(string filename)
        {
            editor.Text = System.IO.File.ReadAllText(filename);//, this.Config.Encoding);
            Archivo_Actual = filename;
            Config.ArchivosRecientes.Add(Archivo_Actual);
            recientesToolStripMenuItem.DropDownItems.Add(Archivo_Actual);
        }
        public void AbrirArchivo()
        {
            OpenFileDialog Open = new OpenFileDialog();

            //se especifica que tipos de archivos se podran abrir y se verifica si existe
            //Open.Filter = "Text [*.txt*]|*.txt|All Files [*,*]|*,*";           Open.CheckFileExists = true;
            Open.Title = "Abrir Archivo";
            Open.ShowDialog(this);
            try
            {
                //este codigo se utiliza para que se pueda pueda mostrar la informacion del archivo que queremos abrir en el rich textbox
                //Open.OpenFile();
                //myStreamReader = System.IO.File.OpenText(Open.FileName);
                editor.Text = System.IO.File.ReadAllText(Open.FileName);//, this.Config.Encoding);

//                string repetido = (from x in Config.ArchivosRecientes where x == Open.FileName select x).ToArray()[0];
                

                Archivo_Actual = Open.FileName;
            }
            catch (Exception) { }
            //finally { myStreamReader.Close(); }
            
            Config.ArchivosRecientes.Add(Archivo_Actual);
            recientesToolStripMenuItem.DropDownItems.Add(Archivo_Actual);
        }


        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //se crea un objeto de tipo savefiledialog que nos servira para guardar el archivo
            SaveFileDialog Save = new SaveFileDialog();
            
            //al igual que para abrir el tipo de documentos aqui se especifica en que extenciones se puede guardar el archivo
            Save.Filter = "Text (*.txt)|*.txt|HTML(*.html*)|*.html|All files(*.*)|*.*";
            Save.CheckPathExists = true;
            Save.Title = "Guardar como";
            Save.ShowDialog(this);
            try
            {
                //este codigo se utiliza para guardar el archivo de nuestro editor
                //myStreamWriter = System.IO.File.AppendText(Save.FileName);
                //myStreamWriter.Write(editor.Text);
                //myStreamWriter.Flush();
                System.IO.File.WriteAllText(Save.FileName, editor.Text);//, this.Config.Encoding);

            }
            catch (Exception) { }
            //finally { myStreamWriter.Close(); }


        }

        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void licenciaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Licencia().ShowDialog();
        }

        private void secretPortalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new SecretPortal().ShowDialog();
        }

        private void editor_TextChanged(object sender, EventArgs e)
        {
            contadores();

            if(guardadoAutomaticoToolStripMenuItem.Checked)
            {
                if(!(String.IsNullOrEmpty(Archivo_Actual)))
                {
                    System.IO.File.WriteAllLines(Archivo_Actual, editor.Lines);
                }
            }
        }

        private void contadores()
        {
            if (editor.Text.Length == 0)
            {
                toolStripStatusContadorPalabras.Text = "0 Palabras";
                toolStripStatusContadorLineas.Text = "0 Lineas";
                toolStripStatusContadorCaracteres.Text = "0 Caracteres";
            }
            else
            {
                toolStripStatusContadorLineas.Text = editor.Lines.Length.ToString() + " Lineas";            
                var resulContador =   ContadorFrases.PalabrasCaracteres(editor.Text);
                // indice 0 es palabras el primer  indice  trae los carcteres

                toolStripStatusContadorPalabras.Text = resulContador[0].ToString() + " Palabras";
                toolStripStatusContadorCaracteres.Text = resulContador[1].ToString() + " Caracteres";

            }

        }

        private void guardadoAutomaticoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.guardadoAutomaticoToolStripMenuItem.Checked = !(this.guardadoAutomaticoToolStripMenuItem.Checked);
        }

        private void codificacionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void uTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void uTF16ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void uNICODEToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void aSCIIToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void dEFAULTToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void fuenteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var font = new FontDialog();
            font.ShowDialog();
            this.Config.ActualFont = font.Font;
            editor.Font = font.Font;
            
        }

        private void Notepad_form_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Config.TamañoForm = this.Size;
            this.Config.ActualFont = this.editor.Font;
            this.Config.GuardadoAutomaticoStatus = guardadoAutomaticoToolStripMenuItem.Checked;
            this.Config.Save();
        }

        private void colorDeFondoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var colorDialog = new ColorDialog();
            colorDialog.ShowDialog();
            this.editor.BackColor = colorDialog.Color;
            this.Config.ColorEditor = colorDialog.Color;
        }

        private void recientesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
           
        }

        private void recientesToolStripMenuItem_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            AbrirArchivo(e.ClickedItem.Text);
        }
    }
}
