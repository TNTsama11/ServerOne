using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServerOne;
using GameServer.Cache;
using GameServer.Model;
using CommunicationProtocol.Dto;
using CommunicationProtocol.Code;


namespace GameServer.Logic
{
    /// <summary>
    /// 游戏逻辑层
    /// </summary>
    public class GameHandler : IHandler
    {
        private GameCache gameCache = Caches.game;
        private UserCache userCache = Caches.user;

        private TransformDto transformDto = new TransformDto();

        public void OnDisconnect(ClientPeer client)
        {
            if (!userCache.IsOnline(client))
            {               
                return;
            }
            string acc = userCache.GetAccByClient(client);
            if (gameCache.IsGameRoom(acc))
            {
                Exit(client);
            }
        }

        public void OnReceive(ClientPeer client, int subCode, object value)
        {
            switch (subCode)
            {
                case GameCode.GAME_ENTER_CERQ:
                    Enter(client);
                    break;
                case GameCode.GAME_EXIT_CERQ:
                    Exit(client);
                    break;
                case GameCode.GAME_SYNC_TRASNFORM_CERQ:
                    ExcuteSyncTransform(client,value as TransformDto);
                    break;
                case GameCode.GAME_SPAWN_CERQ:
                    ExcutePlayerSpawn(client);
                    break;
                case GameCode.GAME_DEATH_CERQ:
                    //TODO
                    //玩家死亡
                    break;
                case GameCode.GAME_SYNC_STATE_HP_CERQ:
                    //TODO
                    //同步hp
                    break;
                case GameCode.GAME_SYNC_STATE_HG_CERQ:
                    //TODO
                    //同步hg
                    break;
                case GameCode.GAME_DO_SKILLS_CERQ:
                    //TODO
                    //释放技能请求
                    break;
                case GameCode.GAME_SYNC_ATTACKTYPE_CERQ:
                    //TODO
                    //同步弹幕类型
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 进入
        /// </summary>
        /// <param name="client"></param>
        public void Enter(ClientPeer client)
        {
            SingleExcute.Instance.Exeute(()=> 
            {
                if (!userCache.IsOnline(client))
                {
                    return;
                }
                string acc = userCache.GetAccByClient(client);
                if (gameCache.IsGameRoom(acc))
                {
                    return;
                }
                GameRoom gameRoom = gameCache.Enter(acc, client);

                if (gameRoom == null) //如果没找到游戏房间
                {
                    //向客户端发送一个空 客户端收到后会向服务器发出进入匹配的请求
                    client.SendMessage(OpCode.GAME, GameCode.GAME_ENTER_SREP, null);
                    return;
                }
                ////给新加入的玩家分配一个初始位置
                //int[] pos = gameRoom.GetRandomPosition();
                //gameRoom.RefreshTrans(acc, pos);

                Tool.PrintMessage("客户端：" + client.clientSocket.RemoteEndPoint.ToString() + "进入游戏,房间ID："+gameRoom.id);
                UserModel model = userCache.GetModelByAcc(acc);
                UserDto userdto = new UserDto(model.Account, model.Name, model.IconID, model.ModelID, model.Lv);
                //将新加入的玩家数据广播
                gameRoom.Broadcast(OpCode.GAME, GameCode.GAME_ENTER_BROA, userdto, client);
                //将新加入的玩家的初始位置广播
                TransformInfo transformInfo = gameRoom.GetTransByAcc(acc);
                TransformDto transformDto1 = new TransformDto(acc, transformInfo.pos, transformInfo.rota);
                gameRoom.Broadcast(OpCode.GAME, GameCode.GAME_SYNC_TRANSFORM_BROA,transformDto1, client);

                GameRoomDto gameRoomDto = new GameRoomDto();
                foreach(var item in gameRoom.UserAccClientDict.Keys)
                {
                    if (!userCache.IsOnline(item))
                    {
                        continue;
                    }
                    UserModel userModel = userCache.GetModelByAcc(item);
                    UserDto userDto = new UserDto(userModel.Account, userModel.Name, userModel.IconID, userModel.ModelID, userModel.Lv);
                    gameRoomDto.UserAccDtoDict.Add(item, userDto);
                    TransformInfo transformInfo2 = gameRoom.GetTransByAcc(item);
                    TransformDto transformDto2 = new TransformDto(item, transformInfo2.pos, transformInfo2.rota);
                    gameRoomDto.UserTransDto.Add(item, transformDto2);
                    int hp = gameRoom.GetHpByAcc(item);
                    HpDto hpDto = new HpDto(item, hp);
                    gameRoomDto.UserHpDict.Add(item, hpDto);
                    int hg = gameRoom.GetHgByAcc(item);
                    HgDto hgDto = new HgDto(item, hg);
                    gameRoomDto.UserHgDict.Add(item, hgDto);
                    int kill = gameRoom.GetKillByAcc(item);
                    KillDto killDto = new KillDto(item, kill);
                    gameRoomDto.UserKillDict.Add(item, killDto);
                }
                //向新加入的玩家发送当前房间信息
                client.SendMessage(OpCode.GAME, GameCode.GAME_ENTER_SREP, gameRoomDto); 
                //向新加入的玩家发送分配给他的位置信息
                //client.SendMessage(OpCode.GAME, GameCode.GAME_SPAWN_SREP, transformDto1);
            });
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="client"></param>
        public void Exit(ClientPeer client)
        {
            SingleExcute.Instance.Exeute(()=> 
            {
                if (!userCache.IsOnline(client))
                {
                    return;
                }
                string acc = userCache.GetAccByClient(client);
                if (!gameCache.IsGameRoom(acc))
                {
                    return;
                }
                GameRoom gameRoom = gameCache.Exit(acc);
                Tool.PrintMessage("客户端：" + client.clientSocket.RemoteEndPoint.ToString() + "离开游戏,房间ID：" + gameRoom.id);
                gameRoom.Broadcast(OpCode.GAME, GameCode.GAME_EXIT_BROA, acc);
            });
        }

        private void ExcuteSyncTransform(ClientPeer client,TransformDto dto)
        {
            SingleExcute.Instance.Exeute(()=> 
            {
                if (dto == null)
                {
                    return;
                }
                //广播客户端传来的自己的方位信息
                string acc = userCache.GetAccByClient(client);
                GameRoom room = gameCache.GetGameRoom(acc);
                room.RefreshTrans(acc, dto.pos, dto.rota);
                room.Broadcast(OpCode.GAME, GameCode.GAME_SYNC_TRANSFORM_BROA, dto, client);
                Tool.PrintMessage("收到客户端" + client.clientSocket.RemoteEndPoint.ToString() + "方位信息：[pos:"+dto.pos[0]+","+dto.pos[1]+ "," + dto.pos[2] + ";rota:"+dto.rota[0]+ "," + dto.rota[1]+ "," + dto.rota[2] + "]");
            });
        }

        private void ExcutePlayerSpawn(ClientPeer client)
        {
            SingleExcute.Instance.Exeute(()=> 
            {
                string acc = userCache.GetAccByClient(client);
                GameRoom room = gameCache.GetGameRoom(acc);
                int[] xz = room.GetRandomPosition();
                room.RefreshTrans(acc, xz);
                TransformInfo transformInfo = room.GetTransByAcc(acc);
                transformDto.Change(acc, transformInfo.pos, transformInfo.rota);
                //将新的重生点发送给玩家
                client.SendMessage(OpCode.GAME, GameCode.GAME_SPAWN_SREP, transformDto);
                //广播重生的玩家的位置
                room.Broadcast(OpCode.GAME, GameCode.GAME_SPAWN_BROA, transformDto, client);
                Tool.PrintMessage("玩家" + client.clientSocket.RemoteEndPoint.ToString() + "已重生");
            });
        }

    }
}
