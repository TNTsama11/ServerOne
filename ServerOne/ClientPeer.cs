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
        private Socket clientSocket;

        /// <summary>
        /// 设置连接对象
        /// </summary>
        /// <param name="socket"></param>
        public void SetSocket(Socket socket)
        {
            PrintMessage("执行SetSocket():设置客户端连接对象");
            clientSocket = socket;
        }
        #region 接收数据
        /// <summary>
        /// 存储接收到数据的缓存区
        /// </summary>
        private List<byte> dataCache = new List<byte>(); 


        //解决粘包和拆包采用数据包头+数据包尾的方式
        //包头是数据的长度，包尾是数据内容

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
