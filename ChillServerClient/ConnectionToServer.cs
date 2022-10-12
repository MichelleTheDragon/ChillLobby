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

namespace ChillServerClient
{
    internal class ConnectionToServer
    {
        TcpClient client = new TcpClient();
        IPEndPoint serverEp;

        string url = "https://localhost:7045/api/Auth/";
        HttpClient clientAPI = new HttpClient();
        string userToken;

        public ConnectionToServer(string IpAddress)
        {
        }

        public async Task<bool> CreateUserAsync(string username, string password)
        {
            HttpContent stringContent = new StringContent("{\"username\":\"" + username + "\"," + "\"password\":\"" + password +"\"}");
            try
            {
                var response = await clientAPI.PostAsJsonAsync(url + "RegisterUser", stringContent);

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

        public async Task<bool> LoginUser(string username, string password)
        {
            HttpContent stringContent = new StringContent("{\"username\":\"" + username + "\"," + "\"password\":\"" + password + "\"}");
            try
            {
                var response = await clientAPI.PostAsJsonAsync(url + "LoginUser", stringContent);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    userToken = response.Content.ReadAsStringAsync().Result;
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
            serverEp = new IPEndPoint(IPAddress.Parse(serverIp), 11000);
            client.Connect(serverEp);
            
            return false;
        }
    }
}
