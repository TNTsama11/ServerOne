using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerOne;
using CommunicationProtocol.Code;
using CommunicationProtocol.Dto;

namespace GameServer.Logic
{
    /// <summary>
    /// 处理位置信息的逻辑
    /// </summary>
    class TransformHandler : IHandler
    {
        public void OnDisconnect(ClientPeer client)
        {
            
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case TransformCode.TRANS_MOVE:
                    TransformDto dto = value as TransformDto;
                    Tool.PrintMessage("收到客户端" + client.clientSocket.RemoteEndPoint.ToString() + "移动消息：" +dto.posX+"  "+dto.posZ + "  " + dto.rotaY );
                    //client.SendMessage(OpCode.TRANSFORM,TransformCode.TRANS_POS,dto);
                    break;
                default: break;
            }
        }
    }
}
