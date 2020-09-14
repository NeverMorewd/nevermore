using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Text;

namespace nevermore.learn.async
{
    public class Tasks
    {
        private static Queue<Action> m_List;
        //线程互斥
        private static object m_obj = new object();

        /// <summary>
        /// 初始化队列
        /// </summary>
        public Tasks()
        {
            if (m_List == null)
                m_List = new Queue<Action>();
        }

        /// <summary>
        /// 线程工作的函数
        /// </summary>
        public void ThreadWork()
        {
            while (true)
            {
                //获取任务
                Action work = Pop();
                //执行任务
                work();
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 从任务队列中取出任务
        /// </summary>
        /// <returns></returns>
        public Action Pop()
        {
            Monitor.Enter(m_obj);
            Action ac = null;
            try
            {
                //当队列有数据，出队.否则等待
                if (m_List.Count > 0)
                {
                    ac = m_List.Dequeue();
                }
                else
                {
                    Monitor.Wait(m_obj);
                    ac = m_List.Dequeue();
                }
            }
            finally
            {
                Monitor.Exit(m_obj);
            }
            return ac;
        }

        /// <summary>
        /// 把任务加入任务队列
        /// </summary>
        public void Push()
        {
            Work w = new Work();
            //上锁
            Monitor.Enter(m_obj);
            //委托
            Action action = new Action(w.RunWork);
            //把任务加入队列中
            m_List.Enqueue(action);
            //通知等待队列中的线程锁定对象状态的更改。
            Monitor.Pulse(m_obj);
            Monitor.Exit(m_obj);
        }
    }

    public class Work
    {
        private static int number;

        /// <summary>
        /// 工作函数
        /// </summary>
        public void RunWork()
        {
            number++;
            Console.WriteLine("hello world：" + number.ToString());
        }
    }

    public class TestTaskQueue
    {
        public static void  Run()
        {
            //加入任务
            for (int i = 0; i < 200; i++)
            {
                Tasks tast = new Tasks();
                tast.Push();
            }

            //开启线程来完成执行任务队列中的任务
            //for (int i = 0; i < 2; i++)
            //{
                Tasks t = new Tasks();
                Thread th = new Thread(new ThreadStart(t.ThreadWork));
                th.Start();
            //}
            Console.Read();
        }
    }
}