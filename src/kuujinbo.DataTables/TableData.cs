using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace kuujinbo.DataTables
{
    [ModelBinder(typeof(ModelBinder))]
    public partial class TableData : ITableData
    {
        public TableData(IEnumerable<IColumnData> columns)
        {
            ActionButtons = new List<ActionButton>();
            SortOrders = new List<SortOrder>();
            MultiValueFilterSeparator = '|';
            Columns = columns;
            SetColumnNames();
        }

        private void SetColumnNames()
        {
            ColumnNames = new string[Columns.Count()];
            for (int i = 0; i < ColumnNames.Length; ++i)
            {
                ColumnNames[i] = Columns.ElementAt(i).Name;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public IList<ActionButton> ActionButtons { get; set; }

        #region interface property implementation
        public int Draw { get; set; }

        public int RecordsTotal { get; set; }

        public int RecordsFiltered { get; set; }

        public bool SaveAs { get; set; }

        public string[] ColumnNames { get; set; }

        public IEnumerable<IColumnData> Columns { get; set; }

        public List<List<object>> Data { get; set; }
        #endregion

        #region URLs
        /// <summary>
        /// URL that returns table data
        /// </summary>
        public string TableDataUrl { get; set; }

        /// <summary>
        /// Row delete URL
        /// </summary>
        public string RowDeleteUrl { get; set; }

        /// <summary>
        /// Row edit URL
        /// </summary>
        public string RowEditUrl { get; set; }

        /// <summary>
        /// Row details URL
        /// </summary>
        public string RowDetailsUrl { get; set; }
        #endregion

        /// <summary>
        /// separator character; exact match multi-value filter(s)
        /// </summary>
        /// <remarks>
        /// multi-value filters perform **EXACT** match; EF/LINQ translates
        /// to SQL 'IN' operator.
        /// </remarks>
        public char MultiValueFilterSeparator { get; set; } 

        public bool CheckboxColumn { get; set; }

        /// <summary>
        /// Script paths to custom client-side table processing.
        /// </summary>
        public string[] CustomScriptPaths { get; set; }

        /* --------------------------------------------------------------------
         * Start && Length values depend on whether the HTTP request wants:
         * -- JSON data for web UI => left as-is from original request to
         *    return a **PAGED** result set
         * -- Binary content (Excel) => explicity set to return a **FULL**
         *    result set
         * --------------------------------------------------------------------
         */
        private int _start;
        public int Start
        {
            get { return SaveAs ? 0 : _start; }
            set { _start = value; }
        }

        private int _length;
        public int Length
        {
            // TODO: add default start value and Settings
            get { return SaveAs ? TableSettings.Settings.MaxSaveAs : _length; }
            set { _length = value; }
        }



        // global search
        public Search Search { get; set; }

        public IEnumerable<SortOrder> SortOrders { get; set; }

        /// <summary>
        /// first column only shown in **web UI** when one or more 'bulk'
        /// action button(s) exist
        /// </summary>
        /// <returns></returns>
        public bool ShowCheckboxColumn()
        {
            return ActionButtons.Where(x => x.Batch).Count() > 0;
        }

        /// <summary>
        /// execute client-side XHR and populate table instance properties, 
        /// most importantly [Data].
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <seealso cref="ModelBinder" />
        /// <remarks>
        /// this code is called in an MVC controller action, with a Table
        /// instance. the ModelBinder maps the HTTP request form 
        /// values to the Table instance properties, and the 
        /// </remarks>
        public void ExecuteRequest<T>(IEnumerable<T> entities)
            where T : class, IIdentifier
        {
            // get count **BEFORE** any search filter(s) applied
            RecordsTotal = entities.Count();

            IEnumerable<Tuple<PropertyInfo, ColumnAttribute>> typeInfo = GetTypeInfo(typeof(T));

            // per column search
            for (int i = 0; i < Columns.Count(); ++i)
            {
                var column = Columns.ElementAt(i);
                if (column.Search != null
                    && column.Searchable
                    && !string.IsNullOrWhiteSpace(column.Search.Value))
                {
                    var tuple = typeInfo.ElementAt(column.Search.ColumnIndex);
                    // multi-value filters perform **EXACT** match; EF/LINQ 
                    // translates to SQL 'IN' operator. 
                    var inElements = column.Search.Value.Split(
                        new char[] { MultiValueFilterSeparator },
                        StringSplitOptions.RemoveEmptyEntries
                    );

                    // single-value search terms => **PARTIAL** match
                    if (inElements.Length <= 1)
                    {
                        entities = entities.Where(e =>
                        {
                            var value = GetPropertyValue(e, tuple.Item1, tuple.Item2);
                            return value != null
                                && value.ToString().IndexOf(
                                    column.Search.Value,
                                    StringComparison.OrdinalIgnoreCase
                                ) != -1;
                        });
                    }
                    // multi-value filter terms => **EXACT** match; 
                    // EF/LINQ translates to SQL 'IN' operator.
                    else
                    {   // make multi-value filter terms case-insensitive
                        for (int j = 0; j < inElements.Length; j++) inElements[j] = inElements[j].ToLower();
                        entities = entities.Where(e =>
                        {
                            var value = GetPropertyValue(e, tuple.Item1, tuple.Item2);
                            return value != null && inElements.Contains(value.ToString().ToLower());
                        });
                    }
                }
            }

            var sortedData = entities.OrderBy(r => "");
            foreach (var sortOrder in SortOrders)
            {
                var column = Columns.ElementAt(sortOrder.ColumnIndex);
                if (column.Sortable)
                {
                    var tuple = typeInfo.ElementAt(sortOrder.ColumnIndex);
                    sortedData = sortOrder.Direction == ModelBinder.ORDER_ASC
                        ? sortedData.ThenBy(e =>
                            { return GetPropertyValue(e, tuple.Item1, tuple.Item2); }
                          )
                        : sortedData = sortedData.ThenByDescending(e =>
                            { return GetPropertyValue(e, tuple.Item1, tuple.Item2); }
                          ); 
                }
            }

            var pagedData = sortedData.Skip(Start).Take(Length);
            var tableData = SaveAs ? InitColumnNames() : new List<List<object>>();
            foreach (var entity in pagedData)
            {
                var row = new List<object>();
                // include record id when XHR, not requesting save as file....
                if (!SaveAs) row.Add(entity.Id);
                foreach (var info in typeInfo)
                {
                    row.Add(GetPropertyValue(entity, info.Item1, info.Item2));
                }
                tableData.Add(row);
            }

            RecordsFiltered = entities.Count();
            Data = tableData;
        }

        /// <summary>
        /// when DB results saved to file, initialize the **first** element
        /// in Data property to the column names
        /// </summary>
        /// <returns></returns>
        private List<List<object>> InitColumnNames()
        {
            var result = new List<List<object>>();
            result.Add(ColumnNames.ToList<object>());
            return result;
        }

        /// <summary>
        /// Type information for entity used to build the DataTable
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Tuple</returns>
        /// <remarks>
        /// method used internally by only two methods, so return a Tuple 
        /// instead of creating a custom DTO object/class 
        /// </remarks>
        private IEnumerable<Tuple<PropertyInfo, ColumnAttribute>> GetTypeInfo(Type type)
        {
            return type.GetProperties().Select(
                p => new
                {
                    prop = p,
                    col = p.GetCustomAttributes(
                        typeof(ColumnAttribute), true)
                        .SingleOrDefault() as ColumnAttribute
                })
                .Where(p => p.col != null)
                .OrderBy(p => p.col.DisplayOrder)
                .Select(p => new Tuple<PropertyInfo, ColumnAttribute>(p.prop, p.col));
        }

        /// <summary>
        /// get model property value, including when the value is a reference
        /// property or collection
        /// </summary>
        /// <typeparam name="T">type parameter</typeparam>
        /// <param name="entity">model/entity instance</param>
        /// <param name="propertyInfo">PropertyInfo</param>
        /// <param name="columnAttribute">ColumnAttribute</param>
        /// <param name="cache"><property name, <objectId, property value>></param>
        /// <returns>
        /// entity property value object - LINQ to Entities can properly sort
        /// and search by object value. 
        /// </returns>
        private object GetPropertyValue<T>(
            T entity,
            PropertyInfo propertyInfo,
            ColumnAttribute columnAttribute
        ) where T : class, IIdentifier
        {
            object data;

            if (IsColection(propertyInfo.PropertyType))
            {
                var lastField = columnAttribute.FieldAccessor.Split('.').Last();
                var collection = propertyInfo.GetValue(entity) as IEnumerable<object>;
                var csvValues = new SortedSet<string>();
                foreach (var element in collection)
                {
                    var value = element.GetType().GetProperty(lastField).GetValue(element);
                    if (value != null)
                    {
                        var stringVal = value.ToString(); 
                        if (!string.IsNullOrWhiteSpace(stringVal)) csvValues.Add(stringVal);
                    }
                }
                data = string.Join(", ", csvValues);
            }
            else if (columnAttribute.FieldAccessor != null)
            {
                var value = propertyInfo.GetValue(entity);
                if (value != null)
                {
                    var fields = columnAttribute.FieldAccessor.Split('.');
                    foreach (var field in fields)
                    {
                        value = value.GetType().GetProperty(field).GetValue(value);

                        if (value == null) break;
                    }
                }
                data = value;
            }
            else
            {
                data = propertyInfo.GetValue(entity);
            }

            return data;
        }

        private bool IsColection(Type type)
        {
            return type != typeof(string) && type.GetInterfaces().Any(
                x => x.IsGenericType
                && x.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            );
        }
    }
}