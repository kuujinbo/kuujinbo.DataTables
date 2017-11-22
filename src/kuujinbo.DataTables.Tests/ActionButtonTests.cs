using System;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace kuujinbo.DataTables.Tests
{
    // M$ code coverage is too stupid to ignore successful Exception testing 
    [ExcludeFromCodeCoverage]
    public class ActionButtonTests
    {
        [Fact]
        public void Constructor_NullOrWhiteSpaceUrl_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => new ActionButton(null, "text")
             );

            Assert.Equal<string>("url", exception.ParamName);
        }

        [Fact]
        public void Constructor_NullOrWhiteSpaceText_ThrowsArgumentNullException()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                 () => new ActionButton("url", null)
             );

            Assert.Equal<string>("text", exception.ParamName);
        }
    }
}