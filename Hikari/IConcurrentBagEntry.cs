using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Hikari
{
    public delegate void BagEntryUpdateState(object sender, int state);
   public abstract class IConcurrentBagEntry
    {
        public const int STATE_NOT_IN_USE = 0;//没有使用
        public const int STATE_IN_USE = 1;//正在使用
        public const int STATE_REMOVED = -1;//移除
        public const int STATE_RESERVED = -2;//预留
        protected   int state;
        public event BagEntryUpdateState StateUpdate;
       
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="expectState"></param>
        /// <param name="newState"></param>
        /// <returns></returns>
        public  virtual bool CompareAndSetState(int expectState, int newState)
        {
            if (Interlocked.CompareExchange(ref state, newState, expectState) != expectState)
            {
                if(StateUpdate!=null)
                {
                    StateUpdate(this, newState);
                }
                return true;
            }
            else
            {
                //没有替换
                return false;
            }
        }

        /// <summary>
        /// 设置状态
        /// </summary>
        /// <param name="newState"></param>
        public virtual void SetState( int newState)
        {
            Interlocked.Exchange(ref state, newState);
        }

        /// <summary>
        /// 注册状态信息
        /// </summary>
        public bool IsRegister { get { return StateUpdate != null; } }
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get { return state; } }
    }
}
