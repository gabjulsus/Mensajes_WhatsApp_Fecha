using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace Mensajes_WhatsApp

{

            public class ChatMessage
                {
                    public string Id { get; set; }
                    public string From { get; set; }
                    public string To { get; set; }
                    public string Author { get; set; }
                    public string Ack { get; set; }
                    public string Body { get; set; }
                    public bool FromMe { get; set; }
                    public string Type { get; set; }
                    public long Timestamp { get; set; }
                    public bool IsForwarded { get; set; }
                    public bool IsMentioned { get; set; }
                    public List<string> MentionedIds { get; set; }
                    public object QuotedMsg { get; set; }
                }


    class Program
    {
        static async Task Main()
        {
            UltraMsg u = new UltraMsg("28699", "0k7q6ovcjpwxen4w");

            string Salida = await u.JsonChatsAsync();
            List<WhatsAppContact> contactos = JsonConvert.DeserializeObject<List<WhatsAppContact>>(Salida);
            System.IO.StreamWriter sr = new System.IO.StreamWriter("c:\\temp\\salida.json");
            sr.Write(Salida);
            sr.Close();
            sr.Close();
            System.IO.StreamWriter sr2 = new System.IO.StreamWriter("c:\\temp\\fechas.json");
            foreach(WhatsAppContact c in contactos)
            {
                DateTime fecha = DateTimeOffset.FromUnixTimeSeconds(c.last_time).UtcDateTime;
                DateTime fecha_local= fecha.ToLocalTime();
                sr2.WriteLine(fecha_local.ToString("dd/MM/yyyy"));
            }
            sr2.Close();

           // Console.Write(Salida);

            string IDS = await u.JsIDAsync();
            IDS = IDS.Replace("\"", "").Replace("[", "").Replace("]", "");
            string[] ids = IDS.Split(',');

            System.IO.StreamWriter sr3 = new System.IO.StreamWriter("c:\\temp\\ids.txt");
            
            foreach (string id in ids) 
            { 
                Console.WriteLine(id);
                string JsonMsb=await u.JsMsgbyId(id);
                sr3.WriteLine(id);
                List<ChatMessage> chatMessages = JsonConvert.DeserializeObject<List<ChatMessage>>(JsonMsb);
                string connectionString = "Data Source=127.0.0.1;Initial Catalog=AdminProp;Integrated Security=SSPI;";
                SqlConnection connection = new SqlConnection(connectionString);
                try
                {
                    connection.Open();

                    foreach (var message in chatMessages)
                    {

                        DateTime fecha = DateTimeOffset.FromUnixTimeSeconds(message.Timestamp).UtcDateTime;
                        DateTime fecha_local = fecha.ToLocalTime();
                        string StrFecha = fecha_local.ToString("dd/MM/yyyy");
                        DateTime fecha_insert = DateTime.Parse(StrFecha);

                        SqlCommand command = new SqlCommand("InsertChatMessage", connection);
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Configurar los parámetros para el procedimiento almacenado
                        command.Parameters.AddWithValue("@Id", message.Id);
                        command.Parameters.AddWithValue("@From", message.From);
                        command.Parameters.AddWithValue("@To", message.To);
                        command.Parameters.AddWithValue("@Author", message.Author);
                        command.Parameters.AddWithValue("@Ack", message.Ack);
                        command.Parameters.AddWithValue("@Body", message.Body);
                        command.Parameters.AddWithValue("@FromMe", message.FromMe);
                        command.Parameters.AddWithValue("@Type", message.Type);
                        command.Parameters.AddWithValue("@Timestamp", fecha_insert);
                        command.Parameters.AddWithValue("@IsForwarded", message.IsForwarded);
                      

                        command.ExecuteNonQuery();
                    }
                }
                finally
                {
                    // Asegúrate de cerrar la conexión manualmente
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        connection.Close();
                    }
                }
            }



            sr3.Close();
        }

    }


    internal class WhatsAppContact
    {
        public string id { get ; set; }
        public string name { get; set; }
        public bool isReadOnly { get; set; }
        public bool isGroup { get; set; }
        public object groupMetadata { get; set; }
        public long last_time { get; set; }
        public bool archived { get; set; }
        public bool pinned { get; set; }
        public bool isMuted { get; set; }
        public int unread { get; set; }

        // Constructor
        public WhatsAppContact()
        {
            SetDefaultStringValues();
        }

        // Método privado para establecer valores predeterminados para propiedades de tipo string
        private void SetDefaultStringValues()
        {
            foreach (var property in GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string))
                {
                    var value = property.GetValue(this) as string;
                    if (value == null)
                    {
                        property.SetValue(this, "");
                    }
                }
            }
        }
    }
}