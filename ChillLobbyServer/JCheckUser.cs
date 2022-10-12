using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChillLobbyServer
{
    [Serializable]
    internal class JCheckUser
    {
        public string token { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
    }
}
