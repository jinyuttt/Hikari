#region << 版 本 注 释 >>
/*----------------------------------------------------------------
* 项目名称 ：HikariAPI
* 项目描述 ：
* 类 名 称 ：SqlMapper
* 类 描 述 ：
* 命名空间 ：HikariAPI
* CLR 版本 ：4.0.30319.42000
* 作    者 ：jinyu
* 创建时间 ：2019
* 版 本 号 ：v1.0.0.0
*******************************************************************
* Copyright @ jinyu 2019. All rights reserved.
*******************************************************************
//----------------------------------------------------------------*/
#endregion



using System;
using System.Collections.Generic;
using System.Text;
using Hikari.Manager;
using Hikari.Integration.Entity;
using System.Text.RegularExpressions;
using System.Data;

namespace HikariAPI
{
    /* ============================================================================== 
* 功能描述：SqlMapper 
* 创 建 者：jinyu 
* 创建日期：2019 
* 更新时间 ：2019
* ==============================================================================*/

    public class SqlMapper : IORM
    {
        private readonly string CfgName = null;

        public SqlMapper(string name=null)
        {
            CfgName = name;
           
        }

        #region

        /// <summary>
        /// DML执行
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Execute(string sql, params dynamic[] param)
        {
            if (param.Length == 0)
            {
               return ManagerPool.Singleton.ExecuteUpdate(sql,CfgName);
            }
            else
            {
                List<string> lst = GetSQLPara(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                int i = 0;
                foreach(string p in lst)
                {
                    dic[p] = param[i];
                }
               return ManagerPool.Singleton.ExecuteUpdate(sql, CfgName, dic);
            }
        }

        /// <summary>
        /// 获取第一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object ExecuteScalar(string sql, params dynamic[] param)
        {
            if (param.Length == 0)
            {
                return ManagerPool.Singleton.ExecuteScalar(sql, CfgName);
            }
            else
            {
                List<string> lst = GetSQLPara(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                int i = 0;
                foreach (string p in lst)
                {
                    dic[p] = param[i];
                }
                return ManagerPool.Singleton.ExecuteScalar(sql, CfgName, dic);
            }
        }

       /// <summary>
       /// 查询转换实体
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <param name="sql"></param>
       /// <param name="param"></param>
       /// <returns></returns>
        public List<T> Query<T>(string sql, params dynamic[] param) where T : new()
        {
            if (param.Length == 0)
            {
                var reader= ManagerPool.Singleton.ExecuteQueryReader(sql, CfgName);
                return reader.ToEntity<T>();
            }
            else
            {
                List<string> lst = GetSQLPara(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                int i = 0;
                foreach (string p in lst)
                {
                    dic[p] = param[i];
                }
                var reader= ManagerPool.Singleton.ExecuteQueryReader(sql, CfgName, dic);
                return reader.ToEntity<T>();
            }
        }

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public DataSet QueryData(string sql, params dynamic[] param)
        {
            if (param.Length == 0)
            {
                return ManagerPool.Singleton.ExecuteQuery(sql, CfgName);
                 
            }
            else
            {
                List<string> lst = GetSQLPara(sql);
                Dictionary<string, object> dic = new Dictionary<string, object>();
                int i = 0;
                foreach (string p in lst)
                {
                    dic[p] = param[i];
                }
               return ManagerPool.Singleton.ExecuteQuery(sql, CfgName, dic);
               
            }
        }


        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="sql"></param>
        /// <param name="lst"></param>
        /// <returns></returns>
        public void BulkCopy<P>(List<P> lst)
        {
            if(lst==null||lst.Count==0)
            {
                return;
            }
            ManagerPool.Singleton.BluckCopy(CfgName, lst.FromEntity<P>());
        }


        /// <summary>
        ///计划使用 insert into table(XXX,XXX)values (XXX,XXX),(XXX,XXX)
        /// </summary>
        /// <typeparam name="P"></typeparam>
        /// <param name="lst"></param>
        public void SqlBulk<P>(List<P> lst=null)
        {
            
        }

        /// <summary>
        /// 提取SQL语句中的参数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private List<string> GetSQLPara(string sql)
        {
            List<string> result = new List<string>();
            Regex paramReg = new Regex(@"(?<!@)[^\w$#@]@(?!@)[\w$#@]+");
            MatchCollection matches = paramReg.Matches(sql);
            foreach (Match m in matches)
            {
                result.Add(m.Groups[0].Value.Substring(m.Groups[0].Value.IndexOf("@")));
            }

            return result;
        }

        #endregion

        /// <summary>
        /// 添加数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int Add<T>(T obj)
        {
            StringBuilder builder = new StringBuilder();
            StringBuilder sbr = new StringBuilder();
            builder.Append("insert into ");
            Dictionary<string, object> para = new Dictionary<string, object>();
            var curType = obj.GetType();
            builder.Append(curType.Name);
            builder.Append("(");
            foreach(var p in curType.GetProperties())
            {
                builder.Append(p.Name);
                sbr.AppendFormat("@{0},", p.Name);
                para[p.Name] = p.GetValue(obj, null);
            }
            builder.Append(") values");
            sbr.Remove(sbr.Length, 1);
            sbr.Append(")");
            string sql = builder.ToString() + sbr.ToString();
           return ManagerPool.Singleton.ExecuteUpdate(sql, CfgName, para);
        }

        /// <summary>
        /// 为了支持匿名类；SQL为NULL则在Val查找TableName属性；
        /// </summary>
        /// <param name="sql">参数化SQL语句</param>
        /// <param name="val">数据</param>
        /// <returns></returns>
        public int Add(dynamic val,string sql=null)
        {
            Dictionary<string, object> para = new Dictionary<string, object>();
            if (string.IsNullOrEmpty(sql))
            {
                Random random = new Random();
                string space = "####" + random.Next();//占位

                StringBuilder builder = new StringBuilder();
                StringBuilder sbr = new StringBuilder();
                builder.Append("insert into ");
              
                var curType = val.GetType();
                builder.Append(space);
                builder.Append("(");
                foreach (var p in curType.GetProperties())
                {
                    builder.Append(p.Name);
                    sbr.AppendFormat("@{0},", p.Name);
                    para[p.Name] = p.GetValue(val, null);
                }
                builder.Append(") values");
                sbr.Remove(sbr.Length, 1);
                sbr.Append(")");
                if (para.ContainsKey("TableName"))
                {
                    sql = builder.ToString() + sbr.ToString();
                    sql = sql.Replace(space, para["TableName"].ToString());
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                var curType = val.GetType();
           
                foreach (var p in curType.GetProperties())
                {
                    para[p.Name] = p.GetValue(val, null);
                }
            }
            return ManagerPool.Singleton.ExecuteUpdate(sql, CfgName, para);
        }

        #region 方便Model属性作为参数，支持匿名类

        /// <summary>
        /// 支持匿名类;使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int ExecuteUpdateModel(string sql, dynamic entity)
        {
            //反射实体
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type cur = entity.FirstOrDefault().GetType();
            foreach (var p in cur.GetProperties())
            {
                dic[p.Name] = p.GetValue(entity, null);
            }
          return  ManagerPool.Singleton.ExecuteUpdate(sql, CfgName, dic);
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public object ExecuteScalarModel(string sql, dynamic entity)
        {
            //反射实体
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type cur = entity.FirstOrDefault().GetType();
            foreach (var p in cur.GetProperties())
            {
                dic[p.Name] = p.GetValue(entity, null);
            }
            return ManagerPool.Singleton.ExecuteScalar(sql, CfgName, dic);
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataSet ExecuteQueryModel(string sql, dynamic entity)
        {
            //反射实体
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type cur = entity.FirstOrDefault().GetType();
            foreach (var p in cur.GetProperties())
            {
                dic[p.Name] = p.GetValue(entity, null);
            }
            return ManagerPool.Singleton.ExecuteQuery(sql, CfgName, dic);
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IDataReader ExecuteQueryReaderModel(string sql, dynamic entity)
        {
            //反射实体
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type cur = entity.FirstOrDefault().GetType();
            foreach (var p in cur.GetProperties())
            {
                dic[p.Name] = p.GetValue(entity, null);
            }
            return ManagerPool.Singleton.ExecuteQueryReader(sql, CfgName, dic);
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<T> ExecuteQueryEntity<T>(string sql, dynamic entity)
        {
            //反射实体
            Dictionary<string, object> dic = new Dictionary<string, object>();
            Type cur = entity.FirstOrDefault().GetType();
            foreach (var p in cur.GetProperties())
            {
                dic[p.Name] = p.GetValue(entity, null);
            }
            var reader= ManagerPool.Singleton.ExecuteQueryReader(sql, CfgName, dic);
           return reader.ToEntity<T>();
        }

        #endregion

    }
}
