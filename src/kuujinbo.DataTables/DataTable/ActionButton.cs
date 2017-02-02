using System;

namespace kuujinbo.DataTables.DataTable
{
    public sealed class ActionButton
    {
        /* --------------------------------------------------------------------
         * all buttons styled with bootstrap CSS, with following action types:
         * -- 'bulk' action => MVC action acts on one or more selected records
         *    in the DataTable.
         * 
         * -- simple hyperlink => e.g to a Create action
         * 
         * -- jQuery modal => informational text or partial view
         * --------------------------------------------------------------------
         */
        // darker blue
        public const string Primary = "btn btn-primary";
        // green
        public const string Success = "btn btn-success";
        // teal
        public const string Info = "btn btn-info";
        // orange
        public const string Warning = "btn btn-warning";
        // red
        public const string Danger = "btn btn-danger";
        // white
        public const string Secondary = "btn btn-secondary";

        /// <summary>
        /// DOM attribute => flag a MVC partial view should be displayed
        /// in client-side modal
        /// </summary>
        public const string ModalAttribute = "data-modal";

        /// <summary>
        /// button performs 'bulk' action on one or more records
        /// </summary>
        /// <remarks>
        /// default => true (see constructor)
        /// </remarks>
        public bool BulkAction { get; set; }

        /// <summary>
        /// button class: default => Success (see constructor)
        /// </summary>
        public string CssClass { get; set; }
        public string Text { get; set; }
        public string Url { get; set; }
        public bool Modal { get; set; }

        public ActionButton(string url, string text)
        {
            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException("url");
            if (string.IsNullOrWhiteSpace(text)) throw new ArgumentNullException("text");

            Url = url;
            Text = text;
            BulkAction = true;
            CssClass = Success;
        }

        /// <summary>
        /// generate button markup
        /// </summary>
        /// <returns>HTML markup</returns>
        public string GetHtml()
        {
            if (BulkAction)
            {
                return string.Format(
                    "<button class='{0}' data-url='{1}'>{2} <span></span></button>\n",
                    CssClass, Url, Text
                );
            }
            else if (Modal)
            {
                return string.Format(
                    "<button class='{0}' data-url='{1}' {2}=''>{3} <span></span></button>\n",
                    CssClass, Url, ModalAttribute, Text
                );
            }
            else
            {
                return string.Format(
                    "<a class='{0}' href='{1}'>{2}</a>\n",
                    CssClass, Url, Text
                );
            }
        }
    }
}