using System.Web;
using System.Web.Optimization;

namespace SimpleMonitor.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js", "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //jqgrid
            bundles.Add(new ScriptBundle("~/bundles/jqgrid").Include(
                     "~/Scripts/i18n/grid.locale-en.js",
                     "~/Scripts/jquery.jqGrid.min.js"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            //jqgrid
            bundles.Add(new StyleBundle("~/Content/jqgrid").Include(
                      "~/Content/themes/sunny/jquery-ui.css",
                      "~/Content/jquery.jqGrid/ui.jqgrid.css"));
        }
    }
}
