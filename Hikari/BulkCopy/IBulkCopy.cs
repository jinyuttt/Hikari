using System.Data;

namespace Hikari.BulkCopy
{
    /// <summary>
    /// 批量插入
    /// </summary>
    public interface IBulkCopy
    {
        void BulkCopy(DataTable dt);

    }
}
