using System.Collections.Generic;
using Soenneker.Data.ZipCode.Utils.Abstract;
using Soenneker.Facts.Local;
using Soenneker.Tests.FixturedUnit;
using Xunit;
using Xunit.Abstractions;

namespace Soenneker.Data.ZipCode.Tests.Utils;

[Collection("Collection")]
public class ExcelFileReaderUtilTests : FixturedUnitTest
{
    private readonly IExcelFileReaderUtil _util;

    public ExcelFileReaderUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<IExcelFileReaderUtil>();
    }

    [LocalFact]
    public void GetZipCodesFromXls_should_parse()
    {
       HashSet<string>? result = _util.GetZipCodesFromXls("C:\\Users\\Jake\\Downloads\\ZIP_Locale_Detail.xls");
    }
}