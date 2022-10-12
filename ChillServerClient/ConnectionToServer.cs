using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ChillServerClient
{
    internal class ConnectionToServer
    {
        TcpClient client = new TcpClient();
        IPEndPoint serverEp;

        string url = "https://localhost:7045/api/Auth/";
        HttpClient clientAPI = new HttpClient();
        string userToken;
        string logedUsername;

        public async Task<bool> CreateUserAsync(string username, string password)
        {
            var jUser = new JUser()
            {
                username = username,
                password = password
            };
            string serializedLogin = JsonConvert.SerializeObject(jUser);
            StringContent httpContent = new StringContent(serializedLogin, Encoding.UTF8, "application/json");
            try
            {
                var response = await clientAPI.PostAsync(url + "RegisterUser", httpContent);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public async Task<bool> LoginUserAsync(string username, string password)
        {
            var jUser = new JUser()
            {
                username = username,
                password = password
            };
            var serializedLogin = JsonConvert.SerializeObject(jUser);
            StringContent httpContent = new StringContent(serializedLogin, Encoding.UTF8, "application/json");
            try
            {
                var response = await clientAPI.PostAsJsonAsync(url + "LoginUser", httpContent);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    userToken = response.Content.ReadAsStringAsync().Result;
                    logedUsername = username;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool ConnectToServer(string serverIp)
        {
            char[] delimiterChars = { ':' };
            string[] fullIpAddress = serverIp.Split(delimiterChars);
            int serverPort;
            bool success = int.TryParse(fullIpAddress[1], out serverPort);
            if (success)
            {
                serverEp = new IPEndPoint(IPAddress.Parse(fullIpAddress[0]), serverPort);
                client.Connect(serverEp);

                byte[] encodedMsg = Encoding.UTF8.GetBytes(userToken + ":-:SplitPoint:-:" + logedUsername);
                client.GetStream().Write(encodedMsg, 0, encodedMsg.Length);
                return true;
            }

            return false;
        }
    }

    [Serializable]
    internal class JUser
    {
        public string username { get; set; }
        public string password { get; set; }
    }
}
