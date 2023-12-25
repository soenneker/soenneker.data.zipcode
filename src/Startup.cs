using Microsoft.Extensions.DependencyInjection;
using Soenneker.Data.ZipCode.Utils;
using Soenneker.Data.ZipCode.Utils.Abstract;
using Soenneker.Git.Util.Registrars;
using Soenneker.Utils.File.Registrars;
using Soenneker.Utils.FileSync.Registrars;
using Soenneker.Utils.HttpClientCache.Registrar;

namespace Soenneker.Data.ZipCode;

/// <summary>
/// Console type startup
/// </summary>
public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    public static void ConfigureServices(IServiceCollection services)
    {
        SetupIoC(services);
    }

    public static void SetupIoC(IServiceCollection services)
    {
        services.AddHttpClientCache();
        services.AddHostedService<ConsoleHostedService>();
        services.AddFileUtilAsScoped();
        services.AddFileUtilSyncAsScoped();
        services.AddGitUtilAsSingleton();
        services.AddSingleton<IExcelFileReaderUtil, ExcelFileReaderUtil>();
        services.AddSingleton<IUspsDownloadUtil, UspsDownloadUtil>();
        services.AddSingleton<IFileOperationsUtil, FileOperationsUtil>();
    }
}