using Soenneker.Tests.FixturedUnit;
using Xunit;

namespace Soenneker.Data.ZipCode.Tests;

[Collection("Collection")]
public class DefaultTests : FixturedUnitTest
{
    public DefaultTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
    }

    [Fact]
    public void Default()
    {

    }
}
