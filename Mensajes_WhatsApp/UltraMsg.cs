using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;

namespace Mensajes_WhatsApp
{
    public class UltraMsg
    {
        private string instance;
        private string token;
        public UltraMsg(string i, string t)
        {
            instance = i;
            token = t;
        }

        public async Task<string> JsonChatsAsync()
        {
            var url = "https://api.ultramsg.com/instance" + instance + "/chats";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Get);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", token);

            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            string retorno = string.IsNullOrEmpty(output) ? "" : output;
            return retorno;
        }

        public async Task<string> JsIDAsync()
        {
            var url = "https://api.ultramsg.com/instance" + instance + "/chats/ids";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Get);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", token);

            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            string retorno = string.IsNullOrEmpty(output) ? "" : output;
            return retorno;
        }

        public async Task<string> JsMsgbyId(string ChatId)
        {
            var url = "https://api.ultramsg.com/instance" + instance + "/chats/messages";
            var client = new RestClient(url);
            var request = new RestRequest(url, Method.Get);
            request.AddHeader("content-type", "application/x-www-form-urlencoded");
            request.AddParameter("token", token);
            request.AddParameter("chatId", ChatId);
            request.AddParameter("limit", "1000");

            RestResponse response = await client.ExecuteAsync(request);
            var output = response.Content;
            string retorno = string.IsNullOrEmpty(output) ? "" : output;
            return retorno;
        }

    }
}
