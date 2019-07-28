using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace HikariAPI
{

    /// <summary>
    /// 配置SQL文件
    /// </summary>
    public class HikariContext
    {
        readonly SqlMapper mapper = null;
        static readonly Dictionary<string, string> cfgSQL = new Dictionary<string, string>();
      
        private const string PreCfg = "_Sql";//配置文件名称后缀
      
        /// <summary>
        /// 默认文件
        /// </summary>
        private const string DefaultFile = "Hikari_Sql.xml";
       
        /// <summary>
        /// SQL配置文件，不带文件后缀
        /// </summary>
        public static string  GloabFile="";

        /// <summary>
        /// SQL语句节点名称，默认Sql
        /// </summary>
        public static string GloabSql = "";

        private readonly string sqlFile=null;
        private readonly string nodeName=null;

        public HikariContext(string file,string node)
        {
            sqlFile = file;
            nodeName = node;
            mapper = new SqlMapper(file);
        }
        public HikariContext()
        {

        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <returns></returns>
        private string GetFile()
        {
            string file = sqlFile;
            if(string.IsNullOrEmpty(file))
            {
                file = GloabFile;
                if(string.IsNullOrEmpty(file))
                {
                    file = DefaultFile;
                }
            }
            return file+PreCfg;
        }

        /// <summary>
        /// 获取节点
        /// </summary>
        /// <returns></returns>
        private string GetNode()
        {
            string node = nodeName;
            if (string.IsNullOrEmpty(node))
            {
                node = GloabSql;
            }
            if(string.IsNullOrEmpty(node))
            {
                node = sqlFile;
            }
            if(string.IsNullOrEmpty(node))
            {
                node = GloabFile;
            }
            //默认文件不判断，直接采用SQL;
            return node;
        }

        #region
        /// <summary>
        /// DML执行
        /// </summary>
        /// <param name="sql">SQL语句</param>
        /// <param name="param"></param>
        /// <returns></returns>
        public int Execute(string key, params dynamic[] param)
        {
            string sql = null;
            if(!cfgSQL.TryGetValue(key,out sql))
            {
                sql = ReadSql(key);
                if(!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.Execute(sql, param);
            }
            
            return -1;
        }

        /// <summary>
        /// 获取SQL
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private string ReadSql(string key)
        {
            Configuration cfg = new Configuration(GetFile());
            var result = Task.Factory.StartNew(async () =>
            {
                
                string tmp = await cfg.ReadAsync(key, GetNode());
                //配置节点，默认节点都没有,则与文件同名称查找
                if (string.IsNullOrEmpty(tmp))
                {
                    string cur = sqlFile;
                    if (string.IsNullOrEmpty(cur))
                    {
                        cur = GloabFile;
                    }
                    if (!string.IsNullOrEmpty(cur))
                    {
                        tmp = await cfg.ReadAsync(key, cur);
                    }
                }
                return tmp;
            });
            return result.Result.Result;
        }

        /// <summary>
        /// 获取第一个值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object ExecuteScalar(string key, params dynamic[] param)
        {
            string sql = null;
            if (!cfgSQL.TryGetValue(key, out sql))
            {
                sql = ReadSql(key);
                if (!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.ExecuteScalar(sql, param);
            }
            
            return null;
        }

        /// <summary>
        /// 查询转换实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<T> Query<T>(string key, params dynamic[] param) where T : new()
        {
            string sql = null;
            if (!cfgSQL.TryGetValue(key, out sql))
            {
                sql = ReadSql(key);
                if (!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.Query<T>(sql, param);
            }
            return null;
        }

        public DataSet QueryData(string key, params dynamic[] param)
        {
            string sql = null;
            if (!cfgSQL.TryGetValue(key, out sql))
            {
                sql = ReadSql(key);
                if (!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.QueryData(sql, param);
            }
            return null;
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
            mapper.BulkCopy(lst);
        }

        public void SqlBulk<P>(List<P> lst = null)
        {
            mapper.SqlBulk(lst);
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
            return mapper.Add(obj);
        }

        /// <summary>
        /// 为了支持匿名类；SQL为NULL则在Val查找TableName属性；
        /// </summary>
        /// <param name="sql">参数化SQL语句</param>
        /// <param name="val">数据</param>
        /// <returns></returns>
        public int Add(dynamic val, string sql = null)
        {
          return  mapper.Add(val, sql);
        }


        #region 方便Model属性作为参数，支持匿名类

        /// <summary>
        /// 支持匿名类;使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public int ExecuteUpdateModel(string key, dynamic entity)
        {

            string sql = null;
            if (!cfgSQL.TryGetValue(key, out sql))
            {
                sql = ReadSql(key);
                if (!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.ExecuteUpdateModel(sql, entity);
            }
            return -1;
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public object ExecuteScalarModel(string key, dynamic entity)
        {
            string sql = null;
            if (!cfgSQL.TryGetValue(key, out sql))
            {
                sql = ReadSql(key);
                if (!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.ExecuteScalarModel(sql, entity);
            }
            return null;
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public DataSet ExecuteQueryModel(string key, dynamic entity)
        {
            string sql = null;
            if (!cfgSQL.TryGetValue(key, out sql))
            {
                sql = ReadSql(key);
                if (!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.ExecuteQueryModel(sql, entity);
            }
            return null;
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public IDataReader ExecuteQueryReaderModel(string key, dynamic entity)
        {
            string sql = null;
            if (!cfgSQL.TryGetValue(key, out sql))
            {
                sql = ReadSql(key);
                if (!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.ExecuteQueryReaderModel(sql, entity);
            }
            return null;
        }

        /// <summary>
        /// 支持匿名类；使用实体属性参数化传值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public List<T> ExecuteQueryEntity<T>(string key, dynamic entity)
        {
            string sql = null;
            if (!cfgSQL.TryGetValue(key, out sql))
            {
                sql = ReadSql(key);
                if (!string.IsNullOrEmpty(sql))
                {
                    cfgSQL[key] = sql;
                }
            }
            if (!string.IsNullOrEmpty(sql))
            {
                cfgSQL[key] = sql;
                return mapper.ExecuteQueryEntity<T>(sql, entity);
            }
            return null;
        }

        #endregion


    }
}
