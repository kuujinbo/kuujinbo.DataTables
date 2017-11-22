using System;

namespace kuujinbo.DataTables
{
    /// <summary>
    /// All buttons use bootstrap CSS, with following action types:
    /// -- 'bulk' action => MVC action processes one or more selected records
    ///     in the DataTable.
    /// -- simple hyperlink => e.g to a Create action
    /// -- jQuery modal => informational text or partial view
    /// </summary>
    public sealed class ActionButton
    {
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
        /// Button performs batch action on more than one record.
        /// default => true (see constructor)
        /// </summary>
        public bool Batch { get; set; }

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
            Batch = true;
            CssClass = Success;
        }
    }
}