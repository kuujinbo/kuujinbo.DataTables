using kuujinbo.DataTables.Json;
using kuujinbo.DataTables.Mvc.Example.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace kuujinbo.DataTables.Mvc.Example.Controllers
{
    public class HomeController : Controller
    {
        private static ICollection<TestModel> _data;

        /* ############################################################################
         * setup DataTable instance for initial HTTP request
         * ############################################################################
         */
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "jQuery DataTables Test";
            var table = InitDataTable(Url);

            if (_data == null)
            {
                string dataFile = Server.MapPath("~/app_data/TableData.json");
                _data = JsonConvert.DeserializeObject<ICollection<TestModel>>(
                    System.IO.File.ReadAllText(dataFile)
                );
            }
            return View("_dataTable", table);
        }

        private Table InitDataTable(UrlHelper url)
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton(url.Action("Create"), "Create")
                    { 
                        BulkAction = false
                    }
                    ,
                    new ActionButton(url.Action("Report"), "Report")
                    { 
                        CssClass = ActionButton.Primary,
                        BulkAction = false,
                        Modal = true
                    },
                    new ActionButton(url.Action("Delete"), "Delete")
                    { 
                        CssClass = ActionButton.Danger,
                    }
                },
                DataUrl = url.Action("GetResults"),
                InfoRowUrl = url.Action("Info"),
                EditRowUrl = url.Action("Update"),
                DeleteRowUrl = url.Action("DeleteOne"),
                ScriptPaths = new string[] { url.Content("~/scripts/dataTablesHome.js") }
            };
            table.SetColumns<TestModel>();

            return table;
        }

        /* ############################################################################
         * all subsequent HTTP requests are done via XHR to update DataTable
         * ############################################################################
        */
        /* ====================================================================
         * back-end database query via Entity Framework
         * ====================================================================
         */
        [HttpPost]
        public ActionResult GetResults(Table table)
        {
            table.ExecuteRequest<TestModel>(_data);
            return new DataTableResult(table);
        }

        /* ====================================================================
         * hyperlink action button(s)
         * ====================================================================
         */
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        /* ====================================================================
         * modal button(s)
         * ====================================================================
         */
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult Report()
        {
            return PartialView();
        }


        /* ====================================================================
         * 'bulk' action button(s); UI checked rows
         * ====================================================================
         */
        [HttpPost]
        public ActionResult Delete(IEnumerable<int> ids)
        {
            var success = true;
            foreach (var id in ids) success = success && Delete(id);

            if (success)
            {
                return new JsonNetResult(GetBatchUpdateResponseObject(ids));
            }
            else
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "There was a problem deleting the record. Please try again."
                );
            }
        }

        // simple JSON result for all 'bulk' actions
        private object GetBatchUpdateResponseObject(IEnumerable<int> ids)
        {
            return string.Format(
                "XHR sent to:{2}[{0}]{2}with POST data [{1}]{2}succeeded!",
                Request.Url, string.Join(", ", ids), Environment.NewLine
            );
        }

        /* ====================================================================
         * per-row actions; three icon links at far right of each record's row
         * ====================================================================
         */
        // details view
        [HttpGet]
        public ActionResult Info(int id)
        {
            return View(id);
        }

        // update view
        [HttpGet]
        public ActionResult Update(int id)
        {
            return View(id);
        }

        // delete
        [HttpPost]
        public ActionResult DeleteOne(int id)
        {
            if (Delete(id))
            {
                return new JsonNetResult(GetBatchUpdateResponseObject(new int[] { id }));
            }
            else
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "There was a problem deleting the record. Please try again."
                );
            }
        }

        /* ====================================================================
         * actions that support per-view custom JavaScript:
         * JqueryDataTables.Table.ScriptPaths
         * ====================================================================
         */
        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult CustomPositionFilter()
        {
            try
            {
                //System.Threading.Thread.Sleep(500);
                // throw new Exception("ERROR");
                return new JsonNetResult(
                    _data.OrderBy(x => x.Position)
                        .Select(x => x.Position)
                        .Distinct()
                        .ToArray()
                );
            }
            catch
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "There was a problem looking up Offices. Please try again."
                );
            }
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult CustomOfficeFilter()
        {
            try
            {
                // throw new Exception("ERROR");
                return new JsonNetResult(
                    _data.OrderBy(x => x.Office)
                        .Select(x => x.Office)
                        .Distinct()
                        .ToArray()
                );
            }
            catch
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "There was a problem looking up Offices. Please try again."
                );
            }
        }

        /* ====================================================================
         * controller helper methods
         * ====================================================================
         */
        private bool Delete(int id)
        {
            try
            {
                var toDelete = _data.SingleOrDefault(x => x.Id == id);
                if (toDelete != null)
                {
                    _data.Remove(toDelete);
                    return true;
                }
            }
            catch { }
            return false;
        }
    }
}