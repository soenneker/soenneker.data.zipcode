using System.Threading.Tasks;
using Soenneker.Data.ZipCode.Utils.Abstract;
using Soenneker.Facts.Local;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Data.ZipCode.Tests.Utils;

[Collection("Collection")]
public class UspsDownloadUtilTests : FixturedUnitTest
{
    private readonly IUspsDownloadUtil _util;

    public UspsDownloadUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IUspsDownloadUtil>();
    }

    [LocalFact]
    public async Task Download_should_download()
    {
       string result = await _util.Download();
    }
}