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
        /// <summary>
        /// 房间内玩家Account与Transform信息的字典
        /// </summary>
        public Dictionary<string,TransformInfo> UserTransDict { get; set; }
        
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
            return UserAccClientDict.Count == 8;
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
            TransformInfo trans = new TransformInfo();
            UserTransDict.Add(acc, trans);
        }
        /// <summary>
        /// 退出游戏房间
        /// </summary>
        public void ExitRoom(string acc)
        {
            UserAccClientDict.Remove(acc);
            UserTransDict.Remove(acc);
        }
        /// <summary>
        /// 刷新玩家位置信息
        /// </summary>
        public void RefreshTrans(string acc,float[]pos,float[] rota)
        {
            UserTransDict[acc].Change(pos, rota);
        }
        public TransformInfo GetTransByAcc(string acc)
        {
            return UserTransDict[acc];
        }
        public void RefreshTrans(string acc, int[] pos)
        {
            UserTransDict[acc].pos[0] = pos[0];
            UserTransDict[acc].pos[2] = pos[1];
        }
        /// <summary>
        /// 获取一个不和其他玩家冲突的位置（x和z）
        /// </summary>
        /// <returns></returns>
        public int[] GetRandomPosition()
        {
            Random ra =new Random(Guid.NewGuid().GetHashCode());
            int x = ra.Next(-20, 20);
            int z= ra.Next(-20, 20);
            foreach(var item in UserTransDict.Values)
            {
                if(Math.Abs(x-item.pos[0])>5|| Math.Abs(z - item.pos[2]) > 5)
                {
                    int[] xz = new int[2];
                    xz[0] = x;
                    xz[1] = z;
                    return xz;
                }
            }
            return GetRandomPosition();
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
