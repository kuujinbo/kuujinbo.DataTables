using System.Diagnostics.CodeAnalysis;
using System.Web.Optimization;

namespace kuujinbo.DataTables
{
    [ExcludeFromCodeCoverage]
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/bootstrap.css",
                "~/Content/dataTables.bootstrap.css",
                "~/Content/themes/base/jquery-ui.css",
                "~/Content/octicons/octicons.css",
                "~/Content/site.css"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery")
                .Include("~/scripts/jquery-{version}.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                "~/Scripts/jquery.dataTables.js",
                "~/Scripts/dataTables.bootstrap.js",
                "~/Scripts/jquery-ui-1.12.1.js",
                "~/Scripts/binary/FileSaver.js",
                "~/Scripts/binary/jquery.binarytransport.js",
                "~/Scripts/binary/jquery-binary.js",
                "~/Scripts/TableConfig.js",
                "~/Scripts/Table.js"
            ));

        }
    }
}