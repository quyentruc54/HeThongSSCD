using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NovaAlert.Common.Wpf
{
    public interface IPageControlContract
    {
        uint GetTotalCount();
        ICollection<object> GetRecordsBy(uint startingIndex, uint numberOfRecords, object filterTag);
    }
}
