/* ============================================================================
 * json that System.Web.Script.Serialization.JavaScriptSerializer can't
 * deal with - i.e. JavaScript dates.
 * ----------------------------------------------------------------------------
 * USAGE - in MVC controller action:
 *      return new JsonNetResult(OBJECT);
 * ============================================================================
 */
using System;
using System.Web;
using System.Web.Mvc;

namespace kuujinbo.DataTables.Json
{
    public class JsonNetResult : ContentResult
    {
        public object Data { get; private set; }

        public JsonNetResult(object data)
        {
            if (data == null) throw new ArgumentNullException("data");

            Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            HttpResponseBase response = context.HttpContext.Response;
            response.ContentType = "application/json";
            response.Write(new JsonNetSerializer().Get(Data));
        }
    }
}