using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ClienteChat
{
    public partial class Form1 : Form
    {
        List<string> mensajes;
        //Cuando clickes en un item del listbox instancias un elemento de la clase Conversacion, previamente confirmas si no esta en la lista de conversaciones
        //si no esta en la lista de conversaciones instancias un objeto de la clase Conversación y le pasas el nombre del usuario destinatario del mensaje como
        //parametro
        TcpClient client;
        public static NetworkStream ns;
        public static StreamReader sr;
        public static StreamWriter sw;
        //Listado de conversaciones que está manteniendo el usuario con otros usuarios
        List<Conversacion> conversaciones = new List<Conversacion>();
        //Conversacion con un usuario destinatario en concreto
        Conversacion conversacion;
        //El usuario al cual se le va a enviar un mensaje
        public static String destinatariochat;
        //El contenido del mensaje
        String dato;
        //El usuario que esta actualmente utilizando el chat
        String currentUser;
        public Form1()
        {
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Validacion para que no existan 2 usuarios con el mismo nombre
            if (usuarioslistbox.Items.Count>1)
            {
                foreach (string user in usuarioslistbox.Items)
                {
                    if (textBox4.Text == user && label5.Text==textBox4.Text)
                    {
                        MessageBox.Show("Ya existe un usuario con ese nombre,elija otro");
                        break;
                    }
                    else
                    {
                        currentUser = textBox4.Text;
                        sw.WriteLine("%LOGIN%" + currentUser + "%" + textBox1.Text);
                        sw.Flush();
                        dato = sr.ReadLine();
                        label5.Text = currentUser;
                        timer1.Enabled = true;
                        button1.Visible = false;
                        usuarioslistbox.Visible = true;
                        button3.Visible = true;
                    }
                }
            }
            else
            {
                if (label5.Text == textBox4.Text)
                {
                    MessageBox.Show("Ya existe un usuario con ese nombre,elija otro");
                }
                else
                {
                    currentUser = textBox4.Text;
                    sw.WriteLine("%LOGIN%" + currentUser + "%" + textBox1.Text);
                    sw.Flush();
                    dato = sr.ReadLine();
                    label5.Text = currentUser;
                    timer1.Enabled = true;
                    button1.Visible = false;
                    usuarioslistbox.Visible = true;
                    button3.Visible = true;
                }

            }
            //Comprobacion del login con los usuarios conectados
                 
        }
        //Conexion con el servidor
        private void button4_Click(object sender, EventArgs e)
        {

            client = new TcpClient(this.textBox2.Text, 2000);
            ns = client.GetStream();
            sr = new StreamReader(ns);
            sw = new StreamWriter(ns);
            dato = sr.ReadLine() + System.Environment.NewLine +
                   sr.ReadLine() + System.Environment.NewLine +
                   sr.ReadLine() + System.Environment.NewLine;
            Console.WriteLine(dato);
            label3.Visible = true;
            textBox4.Visible = true;
            label1.Visible = true;
            textBox1.Visible = true;
            button1.Visible = true;
            label5.Visible = true;
            button4.Visible = false;
            textBox2.Visible = false;
            label2.Visible = false;
            button3.Visible = false;

        }


        // se encargar de leer cada segundo los mensajes que recibe del servidor se encarga de separar los mensajes de funciones
        // de los mensajes de usuarios y del listado de usuarios,todos los mensajes que tengan que ver con una funcion del sistema
        //vienen con un interrogante por lo tanto aquellos que no vengan con interrogante serán mensajes de un usuario
        //En la funcion list limpiamos los items del listbox y nos encargamos de separar los nombres de todos los usuarios y añadirlos
        //como item al listbox
        private void timer1_Tick_1(object sender, EventArgs e)
        {
            sw.WriteLine("%LIST");
            sw.Flush();
            dato = sr.ReadLine();
            if (dato.Contains('%'))
            {
                    if (dato.Split('%')[1] == "LIST")
                    {
                        usuarioslistbox.Items.Clear();
                        foreach (string s in dato.Split('%')[2].Split(','))
                        {
                            if (s != currentUser)
                            {
                                usuarioslistbox.Items.Add(s);
                            }
                        }
                    }             
            }
            else
            {
                ChatVisible();
                //El nombre de la persona que te está enviando el mensaje
                label4.Text=usuarioslistbox.Items[usuarioslistbox.Items.IndexOf(dato.Split(':')[0])].ToString();
                RichTextBox.Text += dato + "\n";
                AnalizadorConversaciones(dato + "\n");
            }


        }

        private void button2_Click(object sender, EventArgs e)
        {
            sw.WriteLine("%SEND%" + label4.Text + "%" + textBox3.Text);
            sw.Flush();
            string mensaje= currentUser + ":" + textBox3.Text + '\n';
            RichTextBox.Text = RichTextBox.Text + mensaje;
            AnalizadorConversaciones(mensaje);
        }

        private void usuarioslistbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (usuarioslistbox.SelectedItem!=null)
            {
                destinatariochat = usuarioslistbox.SelectedItem.ToString();
                label4.Text = destinatariochat;
                ChatVisible();
            }    

        }

        private void label4_TextChanged(object sender, EventArgs e)
        {
            RichTextBox.Clear();
            if (conversaciones.Where(c => c.Destinatario == label4.Text).Count() == 1)
            {
                conversacion = conversaciones.Where(c => c.Destinatario == label4.Text).FirstOrDefault();
                for (int i = 0; i < conversacion.Mensajes.Count; i++)
                {
                    RichTextBox.Text += conversacion.Mensajes[i];
                }
            }
        }
        private void ChatVisible() {
            label4.Visible = true;
            RichTextBox.Visible = true;
            textBox3.Visible = true;
            button2.Visible = true;
        }
        private void AnalizadorConversaciones(string mensaje)
        {
            //Si el item seleccionado esta contenido en la propiedad destinatario de alguna de las clases,la conversacion
            //no se tiene que instanciar, si por el contrario el destinatario no esta en ninguna conversacion se le crea una conversacion   
            if (conversaciones.Where(c => c.Destinatario == label4.Text).Count() == 1)
            {
                conversacion = conversaciones.Where(c => c.Destinatario == label4.Text).FirstOrDefault();
                conversacion.Mensajes.Add(mensaje);
            }
            else
            {
                conversacion = new Conversacion();
                conversacion.Destinatario = label4.Text;
                conversacion.Mensajes.Add(mensaje);
                conversaciones.Add(conversacion);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            sw.WriteLine("%EXIT");
            sw.Flush();
            sr.ReadLine();
            this.button3.Visible = false;

            label3.Visible = false;
            textBox4.Visible = false;
            label1.Visible = false;
            textBox1.Visible = false;
            button1.Visible = false;
            label5.Visible = false;
            usuarioslistbox.Visible = false;
            RichTextBox.Visible = false;
            label4.Visible = false;
            textBox3.Visible = false;
            button2.Visible = false;

            label5.Text = "";
            timer1.Enabled = false;
            label2.Visible = true;
            textBox2.Visible = true;
            button4.Visible = true;

        }
    }
}
