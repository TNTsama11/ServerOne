using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerOne;
using ServerOne.Concurrent;

namespace GameServer.Cache
{
    /// <summary>
    /// 游戏房间缓存层
    /// </summary>
    public class GameCache
    {
        /// <summary>
        /// 玩家Account和房间id的字典
        /// </summary>
        private Dictionary<string, int> UserRoomDict = new Dictionary<string, int>();
        /// <summary>
        /// 房间id和房间数据模型的字典
        /// </summary>
        private Dictionary<int, GameRoom> RoomRoomModelDict = new Dictionary<int, GameRoom>();

        private ConcurrentInt id = new ConcurrentInt(-1);
        /// <summary>
        /// 判断玩家是否在房间内
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public bool IsGameRoom(string acc)
        {
            return UserRoomDict.ContainsKey(acc);
        }
        /// <summary>
        /// 进入游戏房间
        /// </summary>
        /// <param name="acc"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public GameRoom Enter(string acc,ClientPeer client)
        {
            foreach(var item in RoomRoomModelDict.Values)//有空位就加入
            {
                if (item.IsFull())
                {
                    continue;
                }
                item.EnterRoom(acc, client);
                UserRoomDict.Add(acc, item.id);
                return item;
            }//如果满了就新建
            GameRoom gameRoom = new GameRoom(id.Add_Get());
            gameRoom.EnterRoom(acc, client);
            UserRoomDict.Add(acc, gameRoom.id);
            RoomRoomModelDict.Add(gameRoom.id, gameRoom);
            return gameRoom;
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public GameRoom Exit(string acc)
        {
            int id = UserRoomDict[acc];
            GameRoom room = RoomRoomModelDict[id];
            room.ExitRoom(acc);
            UserRoomDict.Remove(acc);
            if (room.IsEmpty())
            {
                RoomRoomModelDict.Remove(id);
            }
            return room;
        }
        /// <summary>
        /// 根据玩家Account获取所在房间
        /// </summary>
        /// <param name="acc"></param>
        /// <returns></returns>
        public GameRoom GetGameRoom(string acc)
        {
            int id = UserRoomDict[acc];
            GameRoom room = RoomRoomModelDict[id];
            return room;
        }
        /// <summary>
        /// 清理游戏房间
        /// </summary>
        /// <param name="room"></param>
        public void ClearGameRoom(GameRoom room)
        {
            RoomRoomModelDict.Remove(room.id);
            foreach(var item in room.UserAccClientDict.Keys)
            {
                UserRoomDict.Remove(item);
            }
            room.UserAccClientDict.Clear();
        }
    }

   
}
