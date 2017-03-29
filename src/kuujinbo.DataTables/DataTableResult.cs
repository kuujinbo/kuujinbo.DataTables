using kuujinbo.DataTables.Json;
using Newtonsoft.Json;
/* ============================================================================
 * convenience class to write JqueryDataTables results in following formats:
 * -- JSON      => XHR call for MVC view
 * -- binary    => excel file dump
 * ============================================================================
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;
using kuujinbo.DataTables.Export;

namespace kuujinbo.DataTables
{
    public sealed class DataTableResult : ActionResult
    {
        public static readonly string CONTENT_TYPE = "text/csv";
        public static readonly string FILENAME = string.Format("{0:yyyyMMdd-HHmm}.csv", DateTime.Now);

        private ITable _table;
        public DataTableResult(ITable table)
        {
            if (table == null) throw new ArgumentNullException("table");
            _table = table;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            if (_table.SaveAs)
            {
                response.ContentType = CONTENT_TYPE;
                response.AddHeader(
                    "Content-Disposition",
                    string.Format("attachment; filename={0}", FILENAME)
                );

                byte[] bytes = new Csv().Export(_table.Data);
                response.OutputStream.Write(bytes, 0, bytes.Length);
            }
            else
            {
                response.ContentType = "application/json";
                response.Write(new JsonNetSerializer().Get(
                    new
                    {
                        draw = _table.Draw,
                        recordsTotal = _table.RecordsTotal,
                        recordsFiltered = _table.RecordsFiltered,
                        data = _table.Data
                    },
                    true
                ));            
            }
        }
    }
}