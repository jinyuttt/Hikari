//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

///**
//* 命名空间: Hikari 
//* 类 名： ConnectionBag
//* CLR版本： 4.0.30319.42000
//* 版本 ：v1.0
//* Copyright (c) jinyu  
//*/

//namespace Hikari
//{
//    public delegate void BagEntryRemove<T>(object sender, T[] entrys);
//    /// <summary>
//    /// 功能描述    ：ConnectionBag  
//    /// 创 建 者    ：jinyu
//    /// 创建日期    ：2018/10/25 12:48:27 
//    /// 最后修改者  ：jinyu
//    /// 最后修改日期：2018/10/25 12:48:27 
//    /// </summary>
//    public class ConnectionStack<T> where T: IConcurrentBagEntry
//    {
//        private ConcurrentStack<T> concurrentStack = null;
//        private int size = 0;
//        private T[] removeList = null;
//        private ReaderWriterLockSlim readerWriterLockSlim=null;
//        private int removeIndex = -1;//移除数据
//        private int removeCount = 10;
//        public event BagEntryRemove<T> ArrayEntryRemove = null;
//        public int Count { get { return concurrentStack.Count; }  }

//        public bool IsEmpty { get { return concurrentStack.IsEmpty; } }

//        /// <summary>
//        /// 批量移除推送
//        /// </summary>
//        public int EntryRemoveCount { get { return removeCount; } set { removeCount = value; RefreshRemove(); } }


//        /// <summary>
//        /// 真实的数据
//        /// 被标记为移除的不计算
//        /// </summary>
//        public int Size { get { return size; } }

//        public ConnectionStack()
//        {
//            concurrentStack = new ConcurrentStack<T>();
//            readerWriterLockSlim = new ReaderWriterLockSlim();
//        }
//        public ConnectionStack(int capticty)
//        {
//            concurrentStack = new ConcurrentStack<T>();
//            readerWriterLockSlim = new ReaderWriterLockSlim();
//            T[] array = (T[])Array.CreateInstance(typeof(T), capticty);
//            if(array!=null)
//            {
//                PushRange(array);
//            }
//        }

//        /// <summary>
//        /// 添加移除的数据
//        /// </summary>
//        /// <param name="item"></param>
//        private void AddRemove(T item)
//        {
//            if(item==null)
//            { return; }
//            readerWriterLockSlim.EnterWriteLock();
//            try
//            {
//                removeList[++removeIndex] = item;
//                if (removeIndex + 1 == removeCount)
//                {
//                    if (ArrayEntryRemove != null)
//                    {
//                        T[] tmp = new T[removeCount];
//                        Array.Copy(removeList, tmp, tmp.Length);
//                        Task.Factory.StartNew(() =>
//                        {
//                            //移除推送
//                            ArrayEntryRemove(this, tmp);
//                        });
//                    }

//                }
//            }
//            catch(Exception ex)
//            {
//                Logger.Singleton.Error("关闭数据异常：" + ex.Message);
//            }
//            readerWriterLockSlim.ExitWriteLock();
//        }

//        /// <summary>
//        /// 刷新
//        /// </summary>
//        private void RefreshRemove()
//        {
//            readerWriterLockSlim.EnterWriteLock();
//            if (removeIndex > -1)
//            {
//                if (ArrayEntryRemove != null)
//                {

//                    T[] tmp = new T[removeIndex + 1];
//                    Array.Copy(removeList, tmp, tmp.Length);
//                    removeIndex = -1;
//                    Task.Factory.StartNew(() =>
//                    {
//                        ArrayEntryRemove(this, tmp);
//                    });
//                }
//            }
//            removeList = new T[removeCount];
//            readerWriterLockSlim.ExitWriteLock();
//        }


//        /// <summary>
//        /// 加入
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public bool Push(T item)
//        {
//            if(item==null||item.State==IConcurrentBagEntry.STATE_REMOVED)
//            {
//                AddRemove(item);
//                return false;
//            }
//            concurrentStack.Push(item);
//            size++;
           
//            item.CompareAndSetState(IConcurrentBagEntry.STATE_IN_USE, IConcurrentBagEntry.STATE_NOT_IN_USE);
//            return true;
//        }

//        /// <summary>
//        /// 添加有效数据
//        /// </summary>
//        /// <param name="items"></param>
//        public void PushRange(T[] items)
//        {
//           foreach(T obj in items)
//            {
//                Push(obj);
//            }
//        }

//        /// <summary>
//        /// 取出有效数据
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public bool TryPeek(out T item)
//        {
//            bool r = false;
//            if(concurrentStack.TryPeek(out item))
//            {
               
//                    //循环取出没有删除的
//                    while (item.State == IConcurrentBagEntry.STATE_REMOVED)
//                    {
//                         concurrentStack.TryPop(out item);
//                         AddRemove(item);
//                        if(!concurrentStack.TryPeek(out item))
//                        {
//                            //没有取出成功跳出;
//                            break;
//                        }
//                    }
                
//                if (item != null)
//                {
                   
//                    item.CompareAndSetState(IConcurrentBagEntry.STATE_IN_USE, IConcurrentBagEntry.STATE_NOT_IN_USE);
//                    r = true;
//                }
//            }
//            return r;
//        }

//        /// <summary>
//        /// 取出有效数据
//        /// </summary>
//        /// <param name="item"></param>
//        /// <returns></returns>
//        public bool TryPop(out T item)
//        {
//            bool r = false;
//            if (concurrentStack.TryPop(out item))
//            {

//                while (item.State == IConcurrentBagEntry.STATE_REMOVED)
//                {
//                    AddRemove(item);
//                    if (!concurrentStack.TryPop(out item))
//                    {
//                        break;//没有取出成功或者取出的可以使用则成功；
//                    }
//                }
//                //更新状态，只有在使用状态下更新
//                //如果已经标记移除或者其它则不更新
//                item.CompareAndSetState(IConcurrentBagEntry.STATE_IN_USE, IConcurrentBagEntry.STATE_NOT_IN_USE);
//                r = true;
//            }
//            return r;
//        }

//        /// <summary>
//        /// 移除只是一个标记
//        /// </summary>
//        /// <param name="item"></param>
//        public void Remove(T item)
//        {
//            //修改状态即可
//            item.CompareAndSetState(IConcurrentBagEntry.STATE_NOT_IN_USE, IConcurrentBagEntry.STATE_REMOVED);
//            size--;
//        }

//        private bool Check()
//        {
//            if ((DateTime.Now - emptyTime).TotalMinutes > emptyTimeM)
//            {
//                //说明超过emptyTimeM分钟都没有用完缓存
//                //准备自动移除现有缓存，重新建立新的缓存
//                //将现有缓存全部视为空闲的
//                T item = null;
//                while (!concurrentStack.IsEmpty)
//                {
//                    if (concurrentStack.TryPop(out item))
//                    {
//                        item.SetState(IConcurrentBagEntry.STATE_REMOVED);
//                        AddRemove(item);
//                    }
//                }
//                return false;
//            }
//            return true;
//        }
//    }
//}
