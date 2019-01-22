using System;

namespace Hikari.Integration.Models
{

    [AttributeUsage(AttributeTargets.Property)]
    public class DataFieldAttribute:Attribute
    {

        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName { set; get; }

        public DataFieldAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }
    }
}