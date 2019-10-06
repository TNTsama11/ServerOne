using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

namespace ServerOne
{
    /// <summary>
    /// 客户端类
    /// </summary>
    class ClientPeer
    {
        public Socket clientSocket { get; set; }

        public ClientPeer()
        {
            receiveArgs = new SocketAsyncEventArgs();
            receiveArgs.UserToken = this;
        }

        #region 接收数据
        /// <summary>
        /// 数据包解析完成后的回调委托
        /// </summary>
        /// <param name="client"></param>
        /// <param name="value"></param>
        public delegate void ReceiveCompleted(ClientPeer client, SocketMessage msg);
        public ReceiveCompleted receiveCompleted;

        /// <summary>
        /// 存储接收到数据的缓存区
        /// </summary>
        private List<byte> dataCache = new List<byte>();
        /// <summary>
        /// 接收的异步套接字操作请求
        /// </summary>
        public SocketAsyncEventArgs receiveArgs { get; set; }
        /// <summary>
        /// 是否正在处理数据
        /// </summary>
        private bool isProcess = false;

        /// <summary>
        /// client层处理数据包
        /// </summary>
        /// <param name="package"></param>
        public void StartReceive(byte[] package)
        {
            Tool.PrintMessage("执行StartReceive()方法:在client层处理数据包");
            dataCache.AddRange(package);
            if (!isProcess)
            {
                ProccessReveive();
            }
        }
        /// <summary>
        /// 处理数据
        /// </summary>
        private void ProccessReveive()
        {
            isProcess = true;
            byte[] data= EncodeTool.DecodePackage(ref dataCache);
            if (data == null) //数据包解析失败
            {
                isProcess = false;
                return;
            }
            SocketMessage msg = EncodeTool.DecodeMessage(data);

            if (receiveCompleted != null)
            {
                receiveCompleted(this, msg);
            }

            ProccessReveive(); //递归调用持续处理
        }

        #endregion



    }
}
