using System;
using System.Web;
using Xunit;

namespace kuujinbo.DataTables.Tests
{
    public class ViewAllPathTests
    {
        [Fact]
        public void All_WithNullUri_ReturnsFalse()
        {
            Assert.False(ViewAllPath.All(null));
        }

        [Fact]
        public void All_WithOutViewAllSegmentInUri_ReturnsTrue()
        {
            Assert.False(ViewAllPath.All(new Uri("http://test.test")));
        }

        [Fact]
        public void All_WithViewAllSegmentInUri_ReturnsTrue()
        {
            var uri = new Uri(new Uri("http://test.test"), ViewAllPath.SEGMENT);
            Assert.True(ViewAllPath.All(uri));
        }

        [Fact]
        public void MakeUrl_MissingHttpRequestBase_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => ViewAllPath.MakeUrl(null)
            );

            Assert.Equal<string>(ViewAllPath.REQUEST_NULL, exception.ParamName);
        }


        [Fact]
        public void MakeUrl_WithTrailingSlashAndNoControllerName_ReturnsBasePathPlusSegment()
        {
            var appPath = "/";
            var mock = new Moq.Mock<HttpRequestBase>();
            mock.Setup(x => x.ApplicationPath).Returns(appPath);

            Assert.Equal(
                string.Format("/{0}", ViewAllPath.SEGMENT),
                ViewAllPath.MakeUrl(mock.Object)
            );
        }

        [Fact]
        public void MakeUrl_WithoutTrailingSlashAndWithControllerName_ReturnsBasePathControllerNameSegment()
        {
            var appPath = "/virtualDirectory";
            var mock = new Moq.Mock<HttpRequestBase>();

            mock.Setup(x => x.ApplicationPath).Returns(appPath);
            var name = "controllerName";

            Assert.Equal(
                string.Format("{0}/{1}/{2}", appPath, name, ViewAllPath.SEGMENT),
                ViewAllPath.MakeUrl(mock.Object, name)
            );
        }
    }
}