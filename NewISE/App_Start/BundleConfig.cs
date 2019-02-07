using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace NewISE.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js",
                        "~/Scripts/jquery.unobtrusive-ajax.min.js",
                        "~/Scripts/bootstrap-modal.js*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            //bundles.Add(new ScriptBundle("~/bundles/moment").Include(
            //            "~/Scripts/moment*"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryBlock").Include(
                        "~/Scripts/jquery.blockUI.js"));

            bundles.Add(new ScriptBundle("~/bundles/globalize").Include(
                        "~/Scripts/globalize.0.1.3/globalize.js*",
                        "~/Scripts/globalize.0.1.3/cultures/globalize.culture.it-IT.js"));


            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Content/jquery-ui-1.12.1.custom/jquery-ui.min.js"));

            //bundles.Add(new StyleBundle("~/Content/jqueryuiCSS").Include(
            //          "~/Content/jquery-ui-1.12.1.custom/jquery-ui.min.css",
            //          "~/Content/jquery-ui-1.12.1.custom/jquery-ui.structure.min.css",
            //          "~/Content/jquery-ui-1.12.1.custom/jquery-ui.theme.min.css"));

            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                      "~/Content/font-awesome.min.css"));



            bundles.Add(new StyleBundle("~/Content/css/select2CSS").Include(
                "~/Content/css/select2.min.css"));

            bundles.Add(new ScriptBundle("~/Scripts/select2").Include(
                        "~/Scripts/select2.min.js",
                        "~/Scripts/i18n/it.js"));

            bundles.Add(new ScriptBundle("~/bundles/unobtrusive").Include(
               "~/Scripts/jquery.unobtrusive*"));

            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
               "~/Scripts/ckeditor/ckeditor.js"));

        }
    }
}