using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerOne
{
    public delegate void ExeuteDelegate();

    /// <summary>
    /// 单线程池
    /// 保证应用层单线程调用传递的方法
    /// 避免多线程时发生的一些错误
    /// </summary>
    public class SingleExcute
    {
        /// <summary>
        /// 互斥锁
        /// </summary>
        public Mutex mutex;
        public SingleExcute()
        {
            mutex = new Mutex();
        }
        /// <summary>
        /// 单线程逻辑
        /// </summary>
        /// <param name="exeuteDelegate"></param>
        public void Exeute(ExeuteDelegate executeDelegate)
        {
            lock (this)
            {
                mutex.WaitOne();
                executeDelegate();
                mutex.ReleaseMutex();
            }
        }

    }
}
