using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
/**
* 命名空间: Hikari 
* 类 名： ConnectionBag
* CLR版本： 4.0.30319.42000
* 版本 ：v1.0
* Copyright (c) jinyu  
*/

namespace Hikari
{

    public delegate void BagEntryRemove<T>(object sender, T[] entrys);

    /// <summary>
    /// 功能描述    ：ConnectionBag  
    /// 创 建 者    ：jinyu
    /// 创建日期    ：2018/10/25 23:31:13 
    /// 最后修改者  ：jinyu
    /// 最后修改日期：2018/10/25 23:31:13 
    /// </summary>
    public class ConcurrentList<T> where T: IConcurrentBagEntry
    {
        private LinkedList<T> link = null;
        private T[] removeList = null;
        private int removeIndex =-1;//移除数据
        private  int removeCount = 10;
        private int size = 0;
        private object lock_obj = new object();
        public event BagEntryRemove<T> ArrayEntryRemove = null;
        
        /// <summary>
        /// 是否有数据
        /// </summary>
        public bool IsEmpty { get {return link.Count == 0; } }

        /// <summary>
        /// 元素个数
        /// </summary>
        public int Count { get { return link.Count; } }

        /// <summary>
        /// 批量移除数据推送个数
        /// </summary>
        public int EntryRemoveCount { get { return removeCount; } set { removeCount = value; RefreshRemove(); } }
       
        /// <summary>
        /// 有效数据大小
        /// </summary>
        public int Size { get { return size; } set { size = value; } }

        /// <summary>
        /// 构造方法
        /// </summary>
         public ConcurrentList()
        {
            link = new LinkedList<T>();
            removeList = new T[removeCount];
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="capticty"></param>
        public ConcurrentList(int capticty)
        {
             T[] array = (T[])Array.CreateInstance(typeof(T), capticty);
            link = new LinkedList<T>(array);
            removeList = new T[removeCount];
        }

        /// <summary>
        /// 添加移除的数据
        /// </summary>
        /// <param name="item"></param>
        private void AddRemove(T item)
        {
          
            removeList[++removeIndex] = item;
            if(removeIndex+1==removeCount)
            {
                if (ArrayEntryRemove != null)
                {
                    T[] tmp = new T[removeCount];
                    Array.Copy(removeList, tmp, tmp.Length);
                    Task.Factory.StartNew(() =>
                    {
                        ArrayEntryRemove(this, tmp);
                    });
                }
            }
        }

        /// <summary>
        /// 刷新移除数据
        /// </summary>
        private void RefreshRemove()
        {
            if (removeIndex>-1)
            {
                if (ArrayEntryRemove != null)
                {
                    T[] tmp = new T[removeIndex + 1];
                    Array.Copy(removeList, tmp, tmp.Length);
                    removeIndex = -1;
                    Task.Factory.StartNew(() =>
                    {
                        ArrayEntryRemove(this, tmp);
                    });
                }
            }
            removeList = new T[removeCount];
        }
        
        /// <summary>
        /// 堆栈出顶
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPop(out T item)
        {
            item = null;
            bool token = false;
            try
            {
                Monitor.Enter(lock_obj, ref token);
                if (link.Last != null)
                {
                    item = link.Last.Value;
                    link.RemoveLast();
                    while (item.State == IConcurrentBagEntry.STATE_REMOVED)
                    {
                        AddRemove(item);//将获取时加入
                        if (link.Last != null)
                        {
                            item = link.Last.Value;//重新取出最后一个
                            link.RemoveLast();//再次移除；
                        }
                        else
                        {
                            item = null;
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
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Logger.Singleton.Debug("ConcurrentList数据POP出异常:" + ex.Message);
                return false;
            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
        }

        /// <summary>
        /// 堆栈出顶，不移除
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryPeek(out T item)
        {
            item = null;
            bool token = false;
            try
            {
                Monitor.Enter(lock_obj, ref token);
                if (link.Last != null)
                {
                    item = link.Last.Value;
                    while (item.State == IConcurrentBagEntry.STATE_REMOVED)
                    {
                        AddRemove(item);//将获取时加入
                        link.RemoveLast();//移除后面的
                        if (link.Last != null)
                        {
                            item = link.Last.Value;
                        }
                        else
                        {
                            item = null;
                            break;
                        }

                    }
                    if (item == null)
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Logger.Singleton.Debug("ConcurrentList数据Peek出异常:" + ex.Message);
                return false;
            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
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
                bool token=false;
                try
                {
                    Monitor.Enter(lock_obj, ref token);
                    link.AddLast(item);
                    size++;
                    if(!item.IsRegister)
                    {
                        //只需一次
                        item.StateUpdate += Item_StateUpdate;
                    }
                }
                catch(Exception ex)
                {
                    Logger.Singleton.Debug("ConcurrentList数据Push出异常:" + ex.Message);
                }
                finally
                {
                    if (token) Monitor.Exit(lock_obj);
                }
                return true;
            }
        }

        private void Item_StateUpdate(object sender, int state)
        {
            if(IConcurrentBagEntry.STATE_REMOVED==state)
            {
                Interlocked.Decrement(ref size);
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
        /// 堆栈底部出元素
        /// </summary>
        /// <returns></returns>
        public T Dequeue()
        {
            T item = null;
            bool token = false;
            try
            {
                Monitor.Enter(lock_obj, ref token);
                if (link.First != null)
                {
                    item = link.First.Value;
                    link.RemoveFirst();
                    while (item.State == IConcurrentBagEntry.STATE_REMOVED)
                    {
                        AddRemove(item);
                        if (link.First != null)
                        {
                            item = link.First.Value;
                            link.RemoveFirst();
                        }
                        else
                        {
                            item = null;
                            break;
                        }
                    }

                    return item;
                }
                else
                {
                    return null;
                }
            }
            catch(Exception ex)
            {
                Logger.Singleton.Debug("ConcurrentList数据Dequeue出异常:" + ex.Message);
                return null;
            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
        }

        /// <summary>
        /// 堆栈底部添加元素
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            bool token = false;
            try
            {
                Monitor.Enter(lock_obj, ref token);
                link.AddFirst(item);
            }
            catch(Exception ex)
            {
                Logger.Singleton.Debug("ConcurrentList数据Enqueue出异常:" + ex.Message);
            }
            finally
            {
                if (token) Monitor.Exit(lock_obj);
            }
        }

    }
}
