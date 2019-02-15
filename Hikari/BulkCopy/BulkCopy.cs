using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Hikari.BulkCopy
{
    /*
     * 说明：当前该类是用反射实现，这样可以不引用具体的数据库客户端
     * 但是反射会慢，后期会修改成EMIT方式
     * 当前.NETStandard2.0不支持EMIT，所以等新版发布后再修改
     * 如果你打算使用并且无法满足你的需求，请自己下载版本修改，不要提交
     * 
     * */
    public delegate void WriteRow(params object[] values);

    /// <summary>
    /// 根据底层驱动调用Bulk类
    /// </summary>
    internal class DBBulkCopy : IBulkCopy
    {
    

        public HikariConnection Connection { get; set; }

        /// <summary>
        /// 驱动的处理类
        /// </summary>
        public Type BulkCls { get; set; }

        private const int BatchSize = 1000;

        private const string SqlDir = "~dbtemp";

        private int fileid = 0;
        //.NET 流默认是4096
        /*
         * 5400转的笔记本硬盘：50-90MB每秒
           7200转的台式机硬盘：90-190MB每秒
           SSD固态硬盘由于工作原理与机械硬盘不同并无较为准确的范围，
           200MB每秒~500MB每秒
         * 
         * */
        private const int MaxBuffer = 10*1024*1024;//每10M写入一次,占用约五分之一写速
       
        public void BulkCopy(DataTable dt)
        {
            try
            {
                if (BulkCls == null)
                {
                    //说明不支持，监测是否是NpgsqlCopy
                    if (Connection.DbConnection.GetType().Name.StartsWith("Npgsql"))
                    {
                        NpgsqlCopy(dt);
                    }
                    else
                    {
                        throw new BulkCopyException("驱动不支持该功能");
                    }
                }
                else if (BulkCls.Name.StartsWith("MySqlBulk"))
                {
                    MySqlCopy(dt);
                }
                else
                {
                    CommonCopy(dt);
                }
            }
            catch(BulkCopyException ex)
            {
                throw ex;
            }
            finally
            {
                this.Close();
            }

        }

        /// <summary>
        /// 按照SqlServer驱动的SqlBulkCopy类一样的方式视为一般方法
        /// </summary>
        /// <param name="dt"></param>
        public void CommonCopy(DataTable dt)
        {
            
            object bulk = Activator.CreateInstance(BulkCls, Connection.DbConnection);
            var SizeProperty = BulkCls.GetProperty("BatchSize");
            var Table = BulkCls.GetProperty("DestinationTableName");
            var ColumnMap = BulkCls.GetProperty("ColumnMappings");
            var method = BulkCls.GetMethod("WriteToServer");
            if (SizeProperty != null)
            {
                SizeProperty.SetValue(bulk, BatchSize);
            }
            if (Table != null)
            {
                Table.SetValue(bulk, dt.TableName);
            }
            if (ColumnMap != null)
            {
                var mem = ColumnMap.PropertyType.GetMethod("Add", new Type[] { typeof(string), typeof(string) });
                if (mem != null)
                {
                    foreach (DataColumn column in dt.Columns)
                    {

                        mem.Invoke(ColumnMap, new string[] { column.ColumnName, column.ColumnName });
                    }
                }
            }
            if (method != null)
            {
                //只调用一次不创建委托
                method.Invoke(bulk, new object[] { dt });
            }
            else
            {
                throw new BulkCopyException("驱动不支持该功能或者查找Bulk类错误");
            }
        }


        /// <summary>
        /// 将datatable数据转成文件
        /// </summary>
        /// <param name="dt"></param>
        /// <returns>文件路径</returns>
        private string WriteFile(DataTable dt)
        {
            string fileName = dt.TableName + Interlocked.Add(ref fileid, 1);

            FileStream fs = new FileStream(Path.Combine(SqlDir,fileName), FileMode.Create, FileAccess.Write,FileShare.ReadWrite, MaxBuffer);
            StreamWriter sw = new StreamWriter(fs, Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            return Path.Combine(SqlDir, fileName);
        }

        /// <summary>
        /// Mysql该类实际和Mysql导入文件是一种方式
        /// 是文件导入的方式
        /// </summary>
        /// <param name="dt"></param>
        private void MySqlCopy(DataTable dt)
        {
            //按照csv格式
            string fileName = "";
            object bulk = Activator.CreateInstance(BulkCls, Connection.DbConnection);
            var FieldTerminator = BulkCls.GetProperty("FieldTerminator");//获取或设置字段终止符
            var FieldQuotationCharacter = BulkCls.GetProperty("FieldQuotationCharacter");//获取或设置字段引号字符。
            var EscapeCharacter = BulkCls.GetProperty("EscapeCharacter");//获取或设置转义字符。
            var LineTerminator = BulkCls.GetProperty("LineTerminator");//获取或设置行终止符。
            var FileName = BulkCls.GetProperty("FileName");//获取或设置文件的名称。
            var NumberOfLinesToSkip = BulkCls.GetProperty("NumberOfLinesToSkip");//获取或设置要跳过的行数。
            var TableName = BulkCls.GetProperty("TableName"); //获取或设置表的名称。
            var Column = BulkCls.GetProperty("Columns"); //获取或设置表的名称。
            var con = BulkCls.GetProperty("Connection");
            var method = BulkCls.GetMethod("Load"); 
            if(method==null)
            {
                throw new BulkCopyException("该驱动不支持或者查找Bulk类错误");
            }
           //另外还可以设置超时
            if (FieldTerminator != null)
            {
                FieldTerminator.SetValue(bulk, ",");
            }
            if (FieldQuotationCharacter != null)
            {
                FieldQuotationCharacter.SetValue(bulk, '"');
            }
            if (EscapeCharacter != null)
            {
                EscapeCharacter.SetValue(bulk, '"');
            }
            if (LineTerminator != null)
            {
                LineTerminator.SetValue(bulk, "\r\n");
            }
            if (NumberOfLinesToSkip != null)
            {
                NumberOfLinesToSkip.SetValue(bulk, 0);
               
            }
            if (TableName != null)
            {
                TableName.SetValue(bulk, dt.TableName);

            }
            if (con != null)
            {
                con.SetValue(bulk, Convert.ChangeType(Connection.DbConnection, con.PropertyType));
            }
            if (FileName != null)
            {
                //这里在准备文件
                fileName = WriteFile(dt);
                FileName.SetValue(bulk, fileName);
            }
            if (Column!=null)
            {
                var columns = dt.Columns.Cast<DataColumn>().Select(colum => colum.ColumnName).ToList();
                List<string> lst= Column.GetValue(bulk) as List<string>;
                if(lst!=null)
                {
                    lst.AddRange(columns);
                }
            }
            if (method != null)
            {
                method.Invoke(bulk,null);
            }
           //用完文件后删除
           if(File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        /// <summary>
        /// Npgsql批量方法
        /// </summary>
        /// <param name="dt"></param>
        private void NpgsqlCopy(DataTable dt)
        {
            var Import = Connection.DbConnection.GetType().GetMethod("BeginBinaryImport");
            if (Import != null)
            {
                var commandFormat = string.Format(CultureInfo.InvariantCulture, "COPY {0} FROM STDIN BINARY", dt.TableName);
                object writer = Import.Invoke(Connection.DbConnection, new object[] { commandFormat });
                var writeFun = writer.GetType().GetMethod("WriteRow");
                var del = (WriteRow)Delegate.CreateDelegate(typeof(WriteRow), writer, writeFun);
                foreach (DataRow item in dt.Rows)
                {
                    del.Invoke(item.ItemArray);
                }
            }
        }


        /// <summary>
        /// 关闭使用的连接
        /// </summary>
        private void Close()
        {
            if(Connection!=null)
            {
                Connection.Dispose();
            }
        }

    }
}
