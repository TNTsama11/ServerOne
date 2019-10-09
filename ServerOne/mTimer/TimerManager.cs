using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using ServerOne.Concurrent;

namespace ServerOne.mTimer
{
    /// <summary>
    /// 计时器管理类
    /// </summary>
    public class TimerManager
    {
        private static TimerManager instance = null;
        public static TimerManager Instance
        {
            get
            {
                lock (instance)
                {
                    if (instance == null)
                    {
                        instance = new TimerManager();
                    }
                    return instance;
                }
            }
        }

        private Timer timer;
        /// <summary>
        /// 存储任务ID和任务模型映射的线程安全的字典
        /// </summary>
        private ConcurrentDictionary<int, TimerModel> idModelDict = new ConcurrentDictionary<int, TimerModel>();
        /// <summary>
        /// 需要移除的任务的列表
        /// </summary>
        private List<int> removeList = new List<int>();
        /// <summary>
        /// ID计数
        /// </summary>
        private ConcurrentInt id = new ConcurrentInt(-1);
        public TimerManager()
        {
            timer = new Timer(10);
            timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// 到达间隔时间时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (removeList) //多个线程访问，所以需要加锁
            {
                TimerModel tempModel = null;
                foreach (var id in removeList)
                {
                    idModelDict.TryRemove(id, out tempModel);//移除触发过的任务
                }
                removeList.Clear();
            }
           
            foreach (var model in idModelDict.Values) 
            {
                if (model.Time<=DateTime.Now.Ticks) //如果之前设置的时间小于或者等于当前时间就执行
                {
                    model.Run(); //任务触发完后需要删除，foreach遍历字典的时候无法对字典元素进行操作否则会出错，所以在之前删除
                  //  removeList.Add(model.Id);
                }
            }
        }

        /// <summary>
        /// 添加计时任务
        /// 指定触发时间点
        /// </summary>
        /// <param name="datetime"></param>
        /// <param name="timeDelegate"></param>
        public void AddTimerEvent(DateTime datetime,TimeDelegate timeDelegate)
        {
            long delayTime = datetime.Ticks - DateTime.Now.Ticks;
            if (delayTime<=0) //时间不能小于零
            {
                return;
            }
            AddTimerEvent(delayTime, timeDelegate);
        }
        /// <summary>
        /// 添加计时任务
        /// 指定延迟时间
        /// </summary>
        /// <param name="delayTime">ms</param>
        /// <param name="timeDelegate"></param>
        public void AddTimerEvent(long delayTime,TimeDelegate timeDelegate)
        {
            TimerModel model = new TimerModel(id.Add_Get(), DateTime.Now.Ticks + delayTime, timeDelegate);
            idModelDict.TryAdd(model.Id, model);
        }
    }
}
