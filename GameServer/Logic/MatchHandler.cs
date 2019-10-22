using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerOne;
using CommunicationProtocol.Code;
using GameServer.Cache;

namespace GameServer.Logic
{
    public class MatchHandler : IHandler
    {

        private MatchCache matchCache = Caches.match;
        private UserCache userCache = Caches.user;

        public void OnDisconnect(ClientPeer client)
        {
         
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case MatchCode.MATCH_ENTER_CREQ:

                    break;
                case MatchCode.MATCH_EXIT_CREQ:

                    break;
                case MatchCode.MATCH_READY_CREQ:

                    break;
                default:
                    break;
            }
        }

        private void Enter(ClientPeer client)
        {
            SingleExcute.Instance.Exeute(delegate {
                string acc = userCache.GetAccByClient(client);
                if (matchCache.IsMatchRoom(acc)) //重复进入
                {
                    return;
                }
                MatchRoom room = matchCache.Enter(acc,client);
                //TODO
                //广播给其他玩家当前玩家已进入
                //将当前房间的数据发给当前玩家
            });
        }
    }
}
