using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerOne;

namespace GameServer
{
    /// <summary>
    /// 消息中心
    /// </summary>
    public class NetMsgCenter : IApplication
    {
        public void OnDisconnect(ClientPeer client)
        {
            throw new NotImplementedException();
        }

        public void OnReceive(ClientPeer client, SocketMessage smg)
        {
            throw new NotImplementedException();
        }
    }
}
