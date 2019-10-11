using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerOne;
using GameServer.Logic;
using CommunicationProtocol.Code;

namespace GameServer
{
    /// <summary>
    /// 消息中心
    /// </summary>
    public class NetMsgCenter : IApplication
    {

        IHandler transform = new TransformHandler();

        public void OnDisconnect(ClientPeer client)
        {
            transform.OnDisconnect(client);
        }

        public void OnReceive(ClientPeer client, SocketMessage smg)
        {
            switch (smg.opCode) //通过操作码判断是给什么模块的数据
            {
                case OpCode.TRANSFORM:
                    transform.OnReceive(client, smg.subCode,smg.value);
                    break;
                default: break;
            }
        }
    }
}
