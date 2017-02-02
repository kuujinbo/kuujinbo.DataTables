using System;
using System.Linq;
using System.Web;

namespace kuujinbo.DataTables.DataTable
{
    public class ViewAllPath
    {
        public const string SEGMENT = "view-all";
        public const string REQUEST_NULL = "request";

        public static bool All(Uri url)
        {
            return url != null
                ? url.Segments.Last().Equals(SEGMENT, StringComparison.OrdinalIgnoreCase)
                : false;
        }

        public static string MakeUrl(
            HttpRequestBase request,
            string controllerName = null)
        {
            if (request == null) throw new ArgumentNullException(REQUEST_NULL);

            var basePath = request.ApplicationPath.TrimEnd(new char[] { '/' });

            return controllerName == null
                ? string.Format("{0}/{1}", basePath, SEGMENT)
                : string.Format("{0}/{1}/{2}", basePath, controllerName, SEGMENT);
        }
    }
}