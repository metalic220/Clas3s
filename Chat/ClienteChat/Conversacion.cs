using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClienteChat
{
    public class Conversacion
    {
        private string destinatario;
        private List<String> mensajes = new List<string>();

        public string Destinatario
        {
            get
            {
                return destinatario;
            }

            set
            {
                destinatario = value;
            }
        }

        public List<string> Mensajes
        {
            get
            {
                return mensajes;
            }

            set
            {
                mensajes = value;
            }
        }
    }
}
