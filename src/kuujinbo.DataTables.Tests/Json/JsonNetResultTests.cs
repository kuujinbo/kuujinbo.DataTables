using kuujinbo.DataTables.Tests;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Xunit;
using kuujinbo.DataTables.Json;

namespace kuujinbo.DataTables.Tests.Json
{
    public class FakeController : Controller
    {
        public ActionResult JsonData(object obj)
        {
            return new JsonNetResult(obj);
        }
    }

    public class JsonNetResultTests
    {
        private FakeController _fakeController;

        public JsonNetResultTests()
        {
            _fakeController = new FakeController();
        }

        private string GetDataFromJsonIndented(string json)
        {
            return json.Split(
                    new string[] { Environment.NewLine },
                    StringSplitOptions.None)
                .Where(x => x != "{" && x != "}")
                .ElementAt(0);
        }

        [Fact]
        public void ExecuteResult_WithNullObjectData_ThrowsArgumentNullException()
        {
            _fakeController.SetFakeControllerContext();

            var exception = Assert.Throws<ArgumentNullException>(
                () => _fakeController
                    .JsonData((object)null)
            );

            Assert.Equal<string>("data", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WithNullContext_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => _fakeController
                    .JsonData("")
                    .ExecuteResult(_fakeController.ControllerContext)
            );

            Assert.Equal<string>("context", exception.ParamName);
        }

        [Fact]
        public void ExecuteResult_WithData_ReturnsCorrectTypeAndHeaders()
        {
            _fakeController.SetFakeControllerContext();

            var result = _fakeController.JsonData(1);
            result.ExecuteResult(_fakeController.ControllerContext);

            Assert.Equal("application/json", _fakeController.Response.ContentType);
            Assert.IsType<JsonNetResult>(result);
        }

        [Fact]
        public void ExecuteResult_WithData_WritesJsonString()
        {
            var json = string.Empty;
            var fakeContext = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> response = new Mock<HttpResponseBase>();
            response.Setup(
                x => x.Write(It.IsAny<string>()))
                    .Callback<string>(y => { json = y; }
            );
            fakeContext.Setup(ctx => ctx.Response).Returns(response.Object);
            _fakeController.ControllerContext = new ControllerContext(
                new RequestContext(fakeContext.Object, new RouteData()),
                _fakeController
            );

            _fakeController
                .JsonData(new Dictionary<string, string> { { "one", "1" } })
                .ExecuteResult(_fakeController.ControllerContext);
            var data = GetDataFromJsonIndented(json);

            Assert.Equal<int>(json.Count(x => x == '"'), 4);
            Assert.Contains("one", json);
            Assert.Equal<int>(json.Count(x => x == ':'), 1);
            Assert.Contains("1", json);
        }

        [Fact]
        public void ExecuteResult_WithDataAndDateFormat_WritesJsonString()
        {
            var json = string.Empty;
            var date = DateTime.Today;
            var fakeContext = new Mock<HttpContextBase>();
            Mock<HttpResponseBase> response = new Mock<HttpResponseBase>();
            response.Setup(
                x => x.Write(It.IsAny<string>()))
                    .Callback<string>(y => { json = y; }
            );
            fakeContext.Setup(ctx => ctx.Response).Returns(response.Object);
            _fakeController.ControllerContext = new ControllerContext(
                new RequestContext(fakeContext.Object, new RouteData()),
                _fakeController
            );

            _fakeController
                .JsonData(date)
                .ExecuteResult(_fakeController.ControllerContext);
            var dateParts = GetDataFromJsonIndented(json)
                .Trim(new char[] { '"' })
                .Split(new char[] { '-' }, StringSplitOptions.None)
                .Select(x => Int32.Parse(x))
                .ToArray();

            Assert.Equal<int>(date.Year, dateParts[0]);
            Assert.Equal<int>(date.Month, dateParts[1]);
            Assert.Equal<int>(date.Day, dateParts[2]);
        }
    }
}