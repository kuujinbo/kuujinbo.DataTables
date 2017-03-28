using Newtonsoft.Json;
/* !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
 * see jQuery DataTables API and class tests for examples how HTTP form 
 * parameters are sent via XHR
 * !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! */
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace kuujinbo.DataTables
{
    public class ModelBinder : DefaultModelBinder
    {
        /* ===================================================================
         * custom data - **NOT** part of jQuery DataTables API
         * ================================================================ */
        public const string CHECK_COLUMN = "checkColumn";
        public const string SAVE_AS = "saveAs";
        public const string COLUMN_NAMES = "columnNames";
        /* ===================================================================
         * everything from here => jQuery DataTables API
         * ================================================================ */
        public const string DRAW = "draw";
        public const string START = "start";
        public const string LENGTH = "length";

        public const string SEARCH_VALUE = "search[value]";

        public const string ORDER_ASC = "asc";
        public const string ORDER_DIR = "order[{0}][dir]";
        public const string ORDER_COLUMN = "order[{0}][column]";

        public const string COLUMNS_SEARCHABLE = "columns[{0}][searchable]";
        public const string COLUMNS_ORDERABLE = "columns[{0}][orderable]";
        public const string COLUMNS_DATA = "columns[{0}][data]";
        public const string COLUMNS_NAME = "columns[{0}][name]";
        public const string COLUMNS_SEARCH_VALUE = "columns[{0}][search][value]";

        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            base.BindModel(controllerContext, bindingContext);
            var request = controllerContext.HttpContext.Request.Form;

            var draw = Convert.ToInt32(request[DRAW]);
            var start = Convert.ToInt32(request[START]);
            var length = Convert.ToInt32(request[LENGTH]);

            /* ===============================================================
             * jQuery DataTables regex **NOT** implemented - there's a reason 
             * the .NET Regex constructor has an overload with a timeout....
             * ============================================================ */
            var search = new Search
            {
                Value = request[SEARCH_VALUE],
            };
            /* ----------------------------------------------------------------
             * SHIFT+CLICK multi-column [de|a]scending sort request
             * ------------------------------------------------------------- */
            var order = new List<SortOrder>();
            for (int i = 0; ; ++i)
            {
                var colOrder = request[string.Format(ORDER_COLUMN, i)];
                if (colOrder == null) break;

                order.Add(new SortOrder
                {
                    // account for checkbox column offset
                    ColumnIndex = Convert.ToInt32(colOrder) - 1,
                    Direction = request[string.Format(ORDER_DIR, i)]
                });
            }
            /* ----------------------------------------------------------------
             * [1] create Table Column collection
             * [2] (potential) multi-column search requests
             * ------------------------------------------------------------- */
            var columns = new List<Column>();
            // start at 1 to **IGNORE** checkbox column
            for (int i = 1; ; ++i)
            {
                var colName = request[string.Format(COLUMNS_NAME, i)];
                if (colName == null) break;

                var searchable = Convert.ToBoolean(request[string.Format(COLUMNS_SEARCHABLE, i)]);
                var orderable = Convert.ToBoolean(request[string.Format(COLUMNS_ORDERABLE, i)]);

                columns.Add(new Column
                {
                    Data = request[string.Format(COLUMNS_DATA, i)],
                    Name = request[string.Format(COLUMNS_NAME, i)],
                    IsSearchable = searchable,
                    IsSortable = orderable,
                    Search = searchable ? new Search()
                    {
                        // account for checkbox column offset
                        ColumnIndex = i - 1,
                        // jQuery DataTables POST is 1 behind when checkbox
                        // column is hidden
                        Value = request[string.Format(COLUMNS_SEARCH_VALUE, i)]
                    } : null
                });
            }

            return new Table
            {
                Draw = draw,
                Start = start,
                Length = length,
                Search = search,
                SortOrders = order,
                Columns = columns,
                CheckboxColumn = Convert.ToBoolean(request[CHECK_COLUMN]),
                SaveAs = Convert.ToBoolean(request[SAVE_AS]),
                ColumnNames = !string.IsNullOrWhiteSpace(request[COLUMN_NAMES]) 
                                ? JsonConvert.DeserializeObject<string[]>(request[COLUMN_NAMES])
                                : null
            };
        }
    }
}