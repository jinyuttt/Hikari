using System;
using System.Collections.Generic;
using System.Text;

namespace Hikari
{
   public abstract class IConcurrentBagEntry
    {
        public const int STATE_NOT_IN_USE = 0;//没有使用
        public const int STATE_IN_USE = 1;//正在使用
        public const int STATE_REMOVED = -1;//移除
        public const int STATE_RESERVED = -2;//预留
        public  abstract bool CompareAndSet(int expectState, int newState);
       public int State { get; set; }
    }
}
