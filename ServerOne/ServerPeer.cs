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
    /// 服务端类
    /// </summary>
public class ServerPeer
    {
        /// <summary>
        /// 服务端Socket对象
        /// </summary>
        private Socket serverSocket;
        /// <summary>
        /// 限制客户端连接数量
        /// </summary>
        private Semaphore acceptSemaphore;
        /// <summary>
        /// 客户端连接池
        /// </summary>
        private ClientPeerPool clientPeerPool;

        /// <summary>
        /// 开启服务器的方法
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="maxCount">最大链接数</param>
        public void Start(int port,int maxCount)
        {
            PrintMessage("执行Start():开始尝试启动服务器");
            try
            {
                PrintMessage("开始启动服务器");
                PrintMessage("初始化服务器对象");
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                PrintMessage("初始化最大请求数为"+ maxCount);
                acceptSemaphore = new Semaphore(maxCount, maxCount);
                PrintMessage("初始化客户端连接池，大小为" + maxCount);
                clientPeerPool = new ClientPeerPool(maxCount);
                ClientPeer tempClientPeer = null;
                for(int i = 0; i < maxCount; i++)  //在开启服务器时new出客户端对象保存在内存中，运行时可取出，减轻服务器运行时的性能开销
                {
                    tempClientPeer = new ClientPeer();
                    PrintMessage("初始化客户端对象"+i+"，存入连接池");
                    clientPeerPool.Enqueue(tempClientPeer);//入队
                }
                PrintMessage(" serverSocket.Bind尝试绑定本地终结点");
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                PrintMessage(" serverSocket.Listen开始监听等待队列最大为10");
                serverSocket.Listen(10);
                StartAccept(null);
            }
            catch (Exception e) //输出异常信息
            {
                PrintMessage(e.Message);
                
            }
        }




        /// <summary>
        /// 控制台输出带时间戳的信息
        /// </summary>
        /// <param name="message">信息</param>
        private void PrintMessage(string message)
        {
            Console.WriteLine(DateTime.Now.ToLongTimeString().ToString()+"-"+message);
        }


        #region 接受连接
        /// <summary>
        /// 开始等待客户端连接
        /// </summary>
        /// <param name="e"></param>
        private void StartAccept(SocketAsyncEventArgs e)
        {
            PrintMessage("执行StartAccept():开始等待客户端连接");
            if (e == null)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += Accept_Completed;
            }
            acceptSemaphore.WaitOne(); //限制线程访问

            PrintMessage("serverSocket.AcceptAsync():开始异步接受客户端连接");
            bool result= serverSocket.AcceptAsync(e); //返回true表示还在执行，返回false表示执行完成
            if (result==false)
            {
                ProcessAccpet(e);
            }
        }

        /// <summary>
        /// 处理连接请求
        /// </summary>
        /// <param name="e"></param>
        private void ProcessAccpet(SocketAsyncEventArgs e)
        {
            PrintMessage("执行ProcessAccpet():开始处理客户端的连接请求");
            ClientPeer client = clientPeerPool.Dequeue(); //从队列中取出一个
            client.SetSocket(e.AcceptSocket);
            e.AcceptSocket = null; //重置e.AcceptSocket
            StartAccept(e);        //递归调用 参数可以传null，但是每次会new耗费性能所以复用e
        }
        /// <summary>
        /// 接受连接请求异步完成后触发的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Accept_Completed(object sender,SocketAsyncEventArgs e)
        {
            PrintMessage("执行Accept_Completed():接受连接请求异步事件完成，开始处理客户端的连接请求");
            ProcessAccpet(e);
        }
        #endregion

        #region 发送数据

        #endregion

        #region 断开连接

        #endregion

        #region 接收数据

        #endregion
    }
}
