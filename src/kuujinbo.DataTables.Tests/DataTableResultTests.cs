using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Xunit;

namespace kuujinbo.DataTables.Tests
{
    public class FakeController : Controller
    {
        public ActionResult GetResults(Table table)
        {
            return new DataTableResult(table);
        }
    }

    public class DataTableResultTests
    {
        private FakeController _fakeController;

        public DataTableResultTests()
        {
            _fakeController = new FakeController();
        }

        [Fact]
        public void ExecuteResult_WithNullObjectData_ThrowsArgumentNullException()
        {
            _fakeController.SetFakeControllerContext();

            var exception = Assert.Throws<ArgumentNullException>(
                () => _fakeController.GetResults((Table)null)
            );

            Assert.Equal<string>("table", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WithNullContext_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => _fakeController
                    .GetResults(new Table())
                    .ExecuteResult(_fakeController.ControllerContext)
            );

            Assert.Equal<string>("context", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WhenSaveAsFalse_ReturnsCorrectTypeAndHeaders()
        {
            _fakeController.SetFakeControllerContext();

            var result = _fakeController.GetResults(new Table());
            result.ExecuteResult(_fakeController.ControllerContext);

            Assert.Equal(
                DataTableResult.JSON_CONTENT_TYPE, 
                _fakeController.Response.ContentType
            );
        }

        [Fact]
        public void ExecuteResult_WhenSaveAsTrue_ReturnsCorrectTypeAndHeaders()
        {
            _fakeController.SetFakeControllerContext();

            var result = _fakeController.GetResults(new Table() 
            { 
                SaveAs = true,
                Data = new List<List<object>>()
                {
                    new List<object>() {1, 2, 3},
                    new List<object>() {4, 5, 6}
                }
            });
            result.ExecuteResult(_fakeController.ControllerContext);

            Assert.Equal(
                DataTableResult.CSV_CONTENT_TYPE, 
                _fakeController.Response.ContentType
            );
        }
    }
}