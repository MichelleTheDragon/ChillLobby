using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChillLobbyServer
{
    [Serializable]
    internal class JServerLogin
    {
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
    }
}
