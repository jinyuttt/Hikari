using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Hikari
{
    public class ConnectionBucket<T> where T : IConcurrentBagEntry
    {
        private ConcurrentStack<T> concurrentStack = null;
        private DateTime emptyTime = DateTime.Now;
        private int emptyTimeM = 2 * 60;//监测

        /// <summary>
        /// 已经移除的对象
        /// </summary>
        public event BagEntryRemove<T> ArrayEntryRemove = null;


        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsEmpty { get { return concurrentStack.IsEmpty; } }

        /// <summary>
        /// 缓存量
        /// </summary>
        public int Count { get { return concurrentStack.Count; } }

        /// <summary>
        /// 监视空闲缓存时间长度
        /// 该时间段内没有出现空闲的情况，则清除现有缓存的数据
        /// 单位：分
        /// 默认120分钟
        /// </summary>
        public int EmptyTime { get { return emptyTimeM; } set { emptyTimeM = value; } }

        public ConnectionBucket()
        {
            concurrentStack = new ConcurrentStack<T>();
            
        }

        public ConnectionBucket(int capticty)
        {
            concurrentStack = new ConcurrentStack<T>();
            
            T[] array = (T[])Array.CreateInstance(typeof(T), capticty);
            if (array != null)
            {
                PushRange(array);
            }
        }

        /// <summary>
        /// 添加移除的数据
        /// </summary>
        /// <param name="item"></param>
        private void AddRemove(T item)
        {
            if (ArrayEntryRemove != null)
            {

                Task.Factory.StartNew(() =>
                {
                    ArrayEntryRemove(this, new T[] { item });
                });
            }

        }

        /// <summary>
        /// 堆栈出顶
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPop(out T item)
        {
            item = null;
            while (true)
            {
                if (!concurrentStack.TryPop(out item))
                {
                    if (concurrentStack.IsEmpty)
                    {
                        emptyTime = DateTime.Now;
                        break;
                    }
                }
                if (item.State == IConcurrentBagEntry.STATE_REMOVED)
                {
                    AddRemove(item);//将获取时加入

                }
                else
                {
                    break;
                }
            }
            if (item != null)
            {
                //设置状态
                item.CompareAndSetState(IConcurrentBagEntry.STATE_NOT_IN_USE, IConcurrentBagEntry.STATE_IN_USE);
                return true;
            }
            return false;

        }

        /// <summary>
        /// 堆栈出顶，不移除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPeek(out T item)
        {
            item = null;
            while (true)
            {
                if(!concurrentStack.TryPeek(out item))
                {
                    if(concurrentStack.IsEmpty)
                    {
                        break;
                    }
                }
                if (item.State == IConcurrentBagEntry.STATE_REMOVED)
                {
                    concurrentStack.TryPop(out item);
                    AddRemove(item);//将获取时加入

                }
                else
                {
                    break;
                }
            }
            if (item != null)
            {
                item.CompareAndSetState(IConcurrentBagEntry.STATE_NOT_IN_USE, IConcurrentBagEntry.STATE_IN_USE);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// 不允许添加已经标记移除的
        /// 堆栈顶添加元素
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Push(T item)
        {
            if (item == null || item.State == IConcurrentBagEntry.STATE_REMOVED)
            {
                return false;
            }
            else
            {
               
                Check();//放入时监测，不影响业务
                item.CompareAndSetState(IConcurrentBagEntry.STATE_IN_USE, IConcurrentBagEntry.STATE_NOT_IN_USE);
                concurrentStack.Push(item);
                return true;
            }
        }

        /// <summary>
        /// 添加一组元素
        /// 其中已经标记移除的不能加入
        /// </summary>
        /// <param name="items"></param>
        public void PushRange(T[] items)
        {
            foreach (T item in items)
            {
                Push(item);
            }
        }

        /// <summary>
        /// 移除,修改状态
        /// </summary>
        /// <param name="item"></param>
        public void Remove(T item)
        {
            //修改状态即可
            item.CompareAndSetState(IConcurrentBagEntry.STATE_NOT_IN_USE, IConcurrentBagEntry.STATE_REMOVED);
        }

        /// <summary>
        /// 监测空闲情况
        /// </summary>
        private bool Check()
        {
            if((DateTime.Now-emptyTime).TotalMinutes>emptyTimeM)
            {
                //说明超过emptyTimeM分钟都没有用完缓存
                //准备自动移除现有缓存，重新建立新的缓存
                //将现有缓存全部视为空闲的
                T item = null;
                while(!concurrentStack.IsEmpty)
                {
                    if(concurrentStack.TryPop(out item))
                    {
                        item.SetState(IConcurrentBagEntry.STATE_REMOVED);
                        AddRemove(item);
                    }
                }
                return false;
            }
            return true;
        }
    }
}
