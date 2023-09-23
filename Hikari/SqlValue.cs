namespace Hikari
{
    /// <summary>
    /// 数据信息
    /// </summary>
    public class SqlValue
    {
        /// <summary>
        /// 值
        /// </summary>
        public object Value { get; set; }   

        /// <summary>
        /// C#参数类型
        /// </summary>
        public string Type { get; set; }    
    }
}