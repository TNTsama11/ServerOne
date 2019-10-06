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

        #region 接收数据
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
            PrintMessage("执行StartReceive()方法:在client层处理数据包");
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

        }

        #endregion



        /// <summary>
        /// 控制台输出带时间戳的信息
        /// </summary>
        /// <param name="message">信息</param>
        private void PrintMessage(string message)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString().ToString() + "-" + message);
        }
    }
}
