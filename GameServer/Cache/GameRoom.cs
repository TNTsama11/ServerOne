using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerOne;

namespace GameServer.Cache
{
    /// <summary>
    /// 游戏房间类
    /// </summary>
    public class GameRoom
    {
        /// <summary>
        /// 房间内玩家Account与连接对象的字典
        /// </summary>
        public Dictionary<string, ClientPeer> UserAccClientDict { get; set; }
        
        public int id { get; set; }

        public GameRoom(int id)
        {
            this.id = id;
            this.UserAccClientDict = new Dictionary<string, ClientPeer>();
        }

        /// <summary>
        /// 游戏房间是否满了
        /// </summary>
        /// <returns></returns>
        public bool IsFull()
        {
            return UserAccClientDict.Count == 32;
        }
        /// <summary>
        /// 游戏房间是否空了
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            return UserAccClientDict.Count == 0;
        }
        /// <summary>
        /// 进入游戏房间
        /// </summary>
        /// <param name="acc">Account</param>
        /// <param name="client">连接对象</param>
        public void EnterRoom(string acc,ClientPeer client)
        {
            UserAccClientDict.Add(acc, client);
        }
        /// <summary>
        /// 退出游戏房间
        /// </summary>
        public void ExitRoom(string acc)
        {
            UserAccClientDict.Remove(acc);
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        /// <param name="opCode">操作码</param>
        /// <param name="subCode">子操作</param>
        /// <param name="value">参数</param>
        /// <param name="client">排除在外的连接对象（一般是当前连接对象）</param>
        public void Broadcast(int opCode, int subCode, object value, ClientPeer client = null)
        {
            foreach(var item in UserAccClientDict.Values)
            {
                if (item == client)
                {
                    continue;
                }
                Tool.PrintMessage("向" + item.clientSocket.RemoteEndPoint.ToString() + "广播信息");
                item.SendMessage(opCode, subCode, value);
            }
        }
    }
}
