using Xunit;
using Xunit.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Newtonsoft.Json.Linq;
using System.Diagnostics.CodeAnalysis;

namespace kuujinbo.DataTables.Tests
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class TableWriterTests
    {
        private readonly ITestOutputHelper _output;

        public TableWriterTests(ITestOutputHelper output)
        {
            _output = output;
        }

        #region ActionButton
        [Fact]
        public void GetButtonMarkup_BulkAction_ReturnsButtonHtml()
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton("url", "text")
                }
            };

            var xElement = XElement.Parse(table.GetButtonMarkup().ToHtmlString());

            Assert.Equal(2, xElement.Nodes().Count());
            Assert.Equal("button", xElement.Name);
            Assert.Equal("url", xElement.Attribute("data-url").Value);
            Assert.Equal(
                "text",
                string.Concat(
                    xElement.Nodes().OfType<XText>().Select(x => x.Value.Trim())
                )
            );
            Assert.Equal(1, xElement.Elements("span").Count());
        }

        [Fact]
        public void GetButtonMarkup_Modal_ReturnsButtonHtml()
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton("url", "text") { Batch = false, Modal = true }
                }
            };

            var xElement = XElement.Parse(table.GetButtonMarkup().ToHtmlString());

            Assert.Equal(2, xElement.Nodes().Count());
            Assert.Equal("button", xElement.Name);
            Assert.Equal("url", xElement.Attribute("data-url").Value);
            Assert.Equal(
                "text",
                string.Concat(
                    xElement.Nodes().OfType<XText>().Select(x => x.Value.Trim())
                )
            );
            Assert.Equal(1, xElement.Elements("span").Count());
            Assert.NotNull(xElement.Attribute(ActionButton.ModalAttribute));
        }

        [Fact]
        public void GetButtonMarkup_NotBulkAction_ReturnsHyperlinkHtml()
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton("url", "text") { Batch = false }
                }
            };

            var xElement = XElement.Parse(table.GetButtonMarkup().ToHtmlString());

            Assert.Equal(0, xElement.Elements().Count());
            Assert.Equal("a", xElement.Name);
            Assert.Equal("url", xElement.Attribute("href").Value);
            Assert.Equal(
                "text",
                string.Concat(
                    xElement.Nodes().OfType<XText>().Select(x => x.Value.Trim())
                )
            );
        }

        [Fact]
        public void RenderActionButtons_NoActionButton_WritesEmptyString()
        {
            var table = new Table();

            var result = table.RenderActionButtons();

            Assert.Equal(0, table.ActionButtons.Count);
            Assert.Equal(string.Empty, result.ToHtmlString());
        }

        [Fact]
        public void RenderActionButtons_ActionButton_WritesHtml()
        {
            var table = new Table()
            {
                ActionButtons = new List<ActionButton>()
                {
                    new ActionButton("/Create", "Create"),
                    new ActionButton("/Delete", "Delete")
                }
            };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderActionButtons()
            ));

            Assert.Equal(2, table.ActionButtons.Count);
            Assert.Equal(2, xElement.Nodes().Count());
        }
        #endregion

        #region thead and tfoot (TableHtmlWriter.cs partial class)
        [Fact]
        public void RenderTable_NullColumns_ThrowsArgumentNullException()
        {
            var table = new Table();
            var exception = Assert.Throws<ArgumentNullException>(
                () => table.RenderTable()
            );

            Assert.Equal<string>("Columns", exception.ParamName);
        }

        [Fact]
        public void RenderTable_EmptyColumns_ThrowsArgumentNullException()
        {
            var table = new Table() { Columns = new List<Column>() };
            var exception = Assert.Throws<ArgumentNullException>(
                () => table.RenderTable()
            );

            Assert.Equal<string>("Columns", exception.ParamName);
        }

        [Fact]
        public void RenderTable_IsSearchableFalse_AddsEmptyDataSetAttribute()
        {
            var columns = new List<Column>() { new Column() };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderTable()
            ));
            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");

            Assert.Equal(table.Columns.ElementAt(0).IsSearchable, false);
            Assert.Equal(
                "", expectedDataSet.Attribute("data-is-searchable").Value
            );
        }

        [Fact]
        public void RenderTable_IsSearchableTrue_AddsDataSetAttributeValue()
        {
            var columns = new List<Column>() { new Column() { IsSearchable = true } };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderTable()
            ));
            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");

            _output.WriteLine("{0}", expectedDataSet);
            Assert.Equal(
                "true", expectedDataSet.Attribute("data-is-searchable").Value
            );
        }

        [Fact]
        public void RenderTable_ColumnDisplayWidth_AddsInlineStyleToThead()
        {
            var columns = new List<Column>()
        {
            new Column() { Display = true } ,
            new Column() { Display = true, DisplayWidth = 20 }
        };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderTable()
            ));

            var theads = xElement.XPathSelectElements("//thead/tr/th");

            // first column is checkbox, last for per-row actions
            Assert.Equal(4, theads.Count());
            // first **data** column should not have inline style: DisplayWidth == 0
            Assert.Null(theads.ElementAt(1).Attribute("style"));

            Assert.Equal(
                string.Format("width:{0}%", columns[1].DisplayWidth),
                theads.ElementAt(2).Attribute("style").Value
            );
        }

        [Fact]
        public void RenderTable_BoolPropertyType_AddsSelectFilterToTfoot()
        {
            var columns = new List<Column>() { new Column() { Type = typeof(bool) } };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderTable()
            ));
            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");

            Assert.Equal(table.Columns.ElementAt(0).IsSearchable, false);
            Assert.Equal(
                "", expectedDataSet.Attribute("data-is-searchable").Value
            );

            var select = xElement.XPathSelectElement("//select");
            var options = select.Nodes().OfType<XElement>();

            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("class").Value));
            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("data-column-number").Value));

            Assert.Equal(3, options.Count());
            Assert.Equal("", options.ElementAt(0).Attribute("value").Value);
            Assert.Equal("selected", options.ElementAt(0).Attribute("selected").Value);
            Assert.Equal("", options.ElementAt(0).Value);
            Assert.Equal("true", options.ElementAt(1).Attribute("value").Value);
            Assert.Equal("Yes", options.ElementAt(1).Value);
            Assert.Equal("false", options.ElementAt(2).Attribute("value").Value);
            Assert.Equal("No", options.ElementAt(2).Value);
        }

        [Fact]
        public void RenderTable_NullableBoolPropertyType_AddsSelectFilterToTfoot()
        {
            var columns = new List<Column>()
        {
            new Column() { Type = typeof(bool?), IsSearchable = true }
        };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderTable()
            ));

            var expectedDataSet = xElement.XPathSelectElement("//th[@data-is-searchable]");
            _output.WriteLine("{0}", expectedDataSet);
            Assert.Equal(
                "true", expectedDataSet.Attribute("data-is-searchable").Value
            );

            var select = xElement.XPathSelectElement("//select");
            var options = select.Nodes().OfType<XElement>();

            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("class").Value));
            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("data-column-number").Value));

            Assert.Equal(3, options.Count());
            Assert.Equal("", options.ElementAt(0).Attribute("value").Value);
            Assert.Equal("selected", options.ElementAt(0).Attribute("selected").Value);
            Assert.Equal("", options.ElementAt(0).Value);
            Assert.Equal("true", options.ElementAt(1).Attribute("value").Value);
            Assert.Equal("Yes", options.ElementAt(1).Value);
            Assert.Equal("false", options.ElementAt(2).Attribute("value").Value);
            Assert.Equal("No", options.ElementAt(2).Value);
        }

        public enum TestEnum { OneTwo, ThreeFour }
        [Fact]
        public void RenderTable_EnumPropertyType_AddsSelectFilterToTfoot()
        {
            var columns = new List<Column>()
        {
            new Column() { Type = typeof(TestEnum) },
            new Column() { Type = typeof(TestEnum), IsSearchable = true }
        };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderTable()
            ));

            var expectedDataSet = xElement.XPathSelectElements("//th[@data-is-searchable]");
            Assert.Equal(table.Columns.ElementAt(0).IsSearchable, false);
            Assert.Equal(table.Columns.ElementAt(1).IsSearchable, true);
            Assert.Equal(2, xElement.Nodes().Count());
            Assert.Equal(
                "", expectedDataSet.ElementAt(0).Attribute("data-is-searchable").Value
            );
            Assert.Equal(
                "true", expectedDataSet.ElementAt(1).Attribute("data-is-searchable").Value
            );

            var select = xElement.XPathSelectElement("//select");
            var options = select.Nodes().OfType<XElement>();
            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("class").Value));
            Assert.False(string.IsNullOrWhiteSpace(select.Attribute("data-column-number").Value));

            Assert.Equal(3, options.Count());
            Assert.Equal("", options.ElementAt(0).Attribute("value").Value);
            Assert.Equal("selected", options.ElementAt(0).Attribute("selected").Value);
            Assert.Equal("", options.ElementAt(0).Value);
            Assert.Equal(TestEnum.OneTwo.ToString(), options.ElementAt(1).Attribute("value").Value);
            Assert.Equal("One Two", options.ElementAt(1).Value);
            Assert.Equal(TestEnum.ThreeFour.ToString(), options.ElementAt(2).Attribute("value").Value);
            Assert.Equal("Three Four", options.ElementAt(2).Value);
        }

        [Fact]
        public void RenderTable_AnyOtherPropertyType_AddsTextInputFiltersToTfoot()
        {
            var columns = new List<Column>() { new Column() };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderTable()
            ));

            var input = xElement.XPathSelectElement("//input[@type='text']");

            Assert.Equal("", input.Value);
            Assert.False(string.IsNullOrWhiteSpace(input.Attribute("style").Value));
            Assert.False(string.IsNullOrWhiteSpace(input.Attribute("data-column-number").Value));
            Assert.False(string.IsNullOrWhiteSpace(input.Attribute("class").Value));
            Assert.False(string.IsNullOrWhiteSpace(input.Attribute("placeholder").Value));
        }

        [Fact]
        public void RenderTable_LastTh_AddsSpansToTfoot()
        {
            var columns = new List<Column>() { new Column() };
            var table = new Table() { Columns = columns };

            var xElement = XElement.Parse(string.Format(
                "<div>{0}</div>", table.RenderTable()
            ));

            var lastTh = xElement.XPathSelectElement("//tfoot/tr/th[last()]");
            var spans = lastTh.Nodes().OfType<XElement>();
            var spanCount = 3;

            Assert.False(string.IsNullOrWhiteSpace(lastTh.Attribute("style").Value));
            Assert.Equal(spanCount, spans.Count());
            for (int i = 0; i < spanCount; ++i)
            {
                Assert.False(string.IsNullOrWhiteSpace(spans.ElementAt(i).Attribute("class").Value));
                Assert.False(string.IsNullOrWhiteSpace(spans.ElementAt(i).Attribute("title").Value));
                Assert.Equal("", spans.ElementAt(i).Value);
            }
            Assert.False(string.IsNullOrWhiteSpace(spans.ElementAt(spanCount - 1).Attribute("id").Value));
        }
        #endregion

        [Fact]
        public void GetJavaScriptConfig_DataUrlIsNull_ThrowsArgumentNullException()
        {
            var table = new Table();
            var exception = Assert.Throws<ArgumentNullException>(
                () => new Table().RenderJavaScriptConfig()
            );

            Assert.Equal<string>("DataUrl", exception.ParamName);
        }

        [Fact]
        public void GetJavaScriptConfig_DataUrlIsEmpty_ThrowsArgumentNullException()
        {
            var table = new Table() { DataUrl = string.Empty };

            var exception = Assert.Throws<ArgumentNullException>(
                () => table.RenderJavaScriptConfig()
            );

            Assert.Equal<string>("DataUrl", exception.ParamName);
        }

        [Fact]
        public void GetJavaScriptConfig_DataUrlNotWhiteSpaceAndOtherPropertiesNotSet_ReturnsJson()
        {
            var table = new Table() { DataUrl = "/" };

            var json = table.RenderJavaScriptConfig();
            dynamic d = JObject.Parse(json.ToHtmlString());
            Assert.Equal<string>(table.DataUrl, d.dataUrl.ToString());
            Assert.Equal<string>("", d.infoRowUrl.ToString());
            Assert.Equal<string>("", d.deleteRowUrl.ToString());
            Assert.Equal<string>("", d.editRowUrl.ToString());
            Assert.Equal(table.ShowCheckboxColumn(), d.showCheckboxColumn.ToObject<bool>());
            Assert.Equal<string>("", d.columnNames.ToString());
            Assert.Equal<char>(table.MultiValueFilterSeparator, Convert.ToChar(d.multiValueFilterSeparator));
        }

        [Fact]
        public void GetJavaScriptConfig_OtherSimplePropertiesSet_ReturnsJson()
        {
            var table = new Table()
            {
                DataUrl = "/",
                InfoRowUrl = "/i",
                DeleteRowUrl = "/d",
                EditRowUrl = "/e",
                MultiValueFilterSeparator = '?'
            };

            var json = table.RenderJavaScriptConfig();
            dynamic d = JObject.Parse(json.ToHtmlString());
            Assert.Equal<string>(table.InfoRowUrl, d.infoRowUrl.ToString());
            Assert.Equal<string>(table.DeleteRowUrl, d.deleteRowUrl.ToString());
            Assert.Equal<string>(table.EditRowUrl, d.editRowUrl.ToString());
            Assert.Equal<char>(table.MultiValueFilterSeparator, d.multiValueFilterSeparator.ToObject<char>());
        }

        [Fact]
        public void GetJavaScriptConfig_SaveAsTrue_AddsColumnNames()
        {
            var table = new Table()
            {
                SaveAs = true,
                DataUrl = "/"
            };
            table.SetColumns<TestModel>();

            var json = table.RenderJavaScriptConfig();
            dynamic d = JObject.Parse(json.ToHtmlString());
            var names = d.columnNames.ToObject<string[]>();

            Assert.Equal(table.ColumnNames[0], names[0]);
            Assert.Equal(table.ColumnNames[1], names[1]);
            Assert.Equal(table.ColumnNames[2], names[2]);
            Assert.Equal(table.ColumnNames[3], names[3]);
            Assert.Equal(table.ColumnNames[4], names[4]);
        }

        [Fact]
        public void GetScriptElements_ScriptPathsNull_ReturnsStringEmpty()
        {
            var table = new Table();

            Assert.Equal(string.Empty, table.RenderCustomScriptPaths().ToHtmlString());
        }

        [Fact]
        public void GetScriptElements_ScriptPathsEmptyReturnsStringEmpty()
        {
            var table = new Table() { CustomScriptPaths = new string[] { } };

            Assert.Equal(string.Empty, table.RenderCustomScriptPaths().ToHtmlString());
        }

        [Fact]
        public void GetScriptElements_ScriptPathsNotEmptyReturnsScriptTags()
        {
            var scripts = new string[] { "0.js", "1.js", "2.js", "3.js", "4.js" };
            var table = new Table() { CustomScriptPaths = scripts };

            var result = table.RenderCustomScriptPaths().ToHtmlString().Split(
                new string[] { "\n" },
                StringSplitOptions.RemoveEmptyEntries
            );

            Assert.Equal(scripts.Length, result.Length);
            for (int i = 0; i < scripts.Length; ++i)
            {
                var xElement = XElement.Parse(result[i]);

                Assert.Equal("script", xElement.Name.ToString());
                Assert.Equal(scripts[i], xElement.Attribute("src").Value);
                Assert.Equal(string.Empty, xElement.Value);
            }
        }
    }
}