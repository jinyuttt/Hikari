using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
namespace HikariAPI
{
   public class ValueTypeParam
    {

        /// <summary>
        /// 值类参数顺序
        /// </summary>
        public static string[] ParamArray = new string[] { "Id,Name" };
       

        /// <summary>
        /// 重新初始化参数名称
        /// </summary>
        /// <param name="cfgFile"></param>
        public static void InitParamArray(string cfgFile="Config/Param.txt")
        {
            if(File.Exists(cfgFile))
            {
                List<string> lst = new List<string>();
                using (StreamReader rd = new StreamReader(cfgFile))
                {
                    while(rd.Peek()!=-1)
                    {
                        string line = rd.ReadLine().Trim();
                        string[] arry = line.Split(',');
                        lst.AddRange(arry);
                    }
                }
                //
                ParamArray = lst.ToArray();
            }
        }
    }
}
