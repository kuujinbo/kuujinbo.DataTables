using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace kuujinbo.DataTables.DataTable.Export
{
    public interface IExport
    {
        byte[] Export(object data);
    }
}