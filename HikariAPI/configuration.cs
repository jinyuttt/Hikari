using System;
using System.Threading.Tasks;
using System.Xml;
namespace HikariAPI
{
    public class Configuration
    {
        private const string DefaulSql = "Sql";
        string cfgSqlFile;
        public Configuration(string file)
        {
            cfgSqlFile = file;
        }

        /// <summary>
        /// 查找节点SQL
        /// </summary>
        /// <param name="name"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public async Task<string> ReadAsync(string name,string node=null)
        {
            if (string.IsNullOrEmpty(node))
            {
                node = DefaulSql;
            }
            //
            string[] xpath = name.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
           
            using(XmlReader reader= XmlReader.Create(cfgSqlFile))
            {
                reader.MoveToContent();
                while(reader.Read())
                {
                    reader.MoveToContent();

                    //避免大文件，截取部分处理
                    if (!reader.IsEmptyElement && reader.LocalName == xpath[0])
                    {
                       // string v = reader.ReadInnerXml();
                        var v = await reader.ReadInnerXmlAsync();
                        string key = FindChild(v, name, node);
                        if (!string.IsNullOrEmpty(key))
                        {
                            return key;
                        }
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// 查找子节点
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private string FindChild(string xml, string path,string nodeSQL)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            var node = document.SelectSingleNode(path);
            if (node != null&&node.HasChildNodes)
            {
                //不允许再间隔
                var child=   node.SelectSingleNode(nodeSQL);
                if (child == null && nodeSQL != DefaulSql)
                {
                    child = node.SelectSingleNode(DefaulSql);
                }
                if(child!=null)
                return child.InnerText;
            }
            return null;
        }
    }
}
