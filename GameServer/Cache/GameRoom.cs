﻿using System;
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
        /// <summary>
        /// 房间内玩家Account与hp的字典
        /// </summary>
        public Dictionary<string,int> UserHpDict { get; set; }
        /// <summary>
        /// 房间内玩家Account与hg的字典
        /// </summary>
        public Dictionary<string,int> UserHgDict { get; set; }
        /// <summary>
        /// 房间内玩家Account与击杀数的字典
        /// </summary>
        public Dictionary<string,int> UserKillDict { get; set; }

        public int id { get; set; }

        public GameRoom(int id)
        {
            this.id = id;
            this.UserAccClientDict = new Dictionary<string, ClientPeer>();
            this.UserTransDict = new Dictionary<string, TransformInfo>();
            this.UserHpDict = new Dictionary<string, int>();
            this.UserHgDict = new Dictionary<string, int>();
            this.UserKillDict = new Dictionary<string, int>();
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
            UserHpDict.Add(acc,100);
            UserHgDict.Add(acc, 200);
            UserKillDict.Add(acc, 0);
        }
        /// <summary>
        /// 退出游戏房间
        /// </summary>
        public void ExitRoom(string acc)
        {
            UserAccClientDict.Remove(acc);
            UserTransDict.Remove(acc);
            UserHpDict.Remove(acc);
            UserHgDict.Remove(acc);
            UserKillDict.Remove(acc);
        }
        /// <summary>
        /// 获取玩家hp
        /// </summary>
        public int GetHpByAcc(string acc)
        {
            return UserHpDict[acc];
        }
        /// <summary>
        /// 设置玩家hp
        /// </summary>
        public void SetHp(string acc,int hp)
        {
            UserHpDict[acc] = hp;
        }
        /// <summary>
        /// 获取玩家hg
        /// </summary>
        public int GetHgByAcc(string acc)
        {
            return UserHgDict[acc];
        }
        /// <summary>
        /// 设置玩家hg
        /// </summary>
        public void SetHg(string acc,int hg)
        {
            UserHgDict[acc] = hg;
        }
        /// <summary>
        /// 获取玩家kill
        /// </summary>
        public int GetKillByAcc(string acc)
        {
            return UserKillDict[acc];
        }
        /// <summary>
        /// 设置玩家kill
        /// </summary>
        public void SetKill(string acc,int kill)
        {
            UserKillDict[acc] = kill;
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
        /// 生成一个不和其他玩家冲突的位置（x和z）
        /// </summary>
        public int[] GetRandomPosition()
        {
            Random ra =new Random(Guid.NewGuid().GetHashCode());
            int x = ra.Next(-20, 20);
            int z= ra.Next(-20, 20);
            foreach(var item in UserTransDict.Values)
            {
                if(Math.Abs(x-item.pos[0])>5|| Math.Abs(z - item.pos[2]) > 5)
                {
                    continue;
                }
                else
                {
                    return GetRandomPosition();
                }
            }
            int[] xz = new int[2];
            xz[0] = x;
            xz[1] = z;
            return xz;
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
