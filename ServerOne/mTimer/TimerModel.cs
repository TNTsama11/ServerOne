using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerOne.mTimer
{
    /// <summary>
    /// 计时器到时间后触发
    /// </summary>
    public delegate void TimeDelegate();
    /// <summary>
    /// 计时器任务模型
    /// </summary>
    public class TimerModel
    {
       public int Id;
        /// <summary>
        /// 任务执行时间
        /// </summary>
        public long Time;
        private TimeDelegate timeDelegate;
        public TimerModel(int id,long time,TimeDelegate td)
        {
            Id = id;
            Time = time;
            timeDelegate = td;
        }

        /// <summary>
        /// 触发任务
        /// </summary>
        public void Run()
        {
            timeDelegate();
            
        }
    }
}
