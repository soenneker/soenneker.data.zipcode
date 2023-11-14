using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Soenneker.Data.ZipCode.Utils.Abstract;

namespace Soenneker.Data.ZipCode;

public class ConsoleHostedService : IHostedService
{
    private readonly ILogger<ConsoleHostedService> _logger;

    private readonly IHostApplicationLifetime _appLifetime;
    private readonly IGitUtil _gitUtil;
    private readonly IConfiguration _config;
    private readonly IFileOperationsUtil _fileOperationsUtil;
    private readonly IExcelFileReaderUtil _excelFileReaderUtil;
    private readonly IUspsDownloadUtil _uspsDownloadUtil;

    private int? _exitCode;

    public ConsoleHostedService(ILogger<ConsoleHostedService> logger, IHostApplicationLifetime appLifetime, IGitUtil gitUtil, IConfiguration config, 
        IFileOperationsUtil fileOperationsUtil, IExcelFileReaderUtil excelFileReaderUtil, IUspsDownloadUtil uspsDownloadUtil)
    {
        _logger = logger;
        _appLifetime = appLifetime;
        _gitUtil = gitUtil;
        _config = config;
        _fileOperationsUtil = fileOperationsUtil;
        _excelFileReaderUtil = excelFileReaderUtil;
        _uspsDownloadUtil = uspsDownloadUtil;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _appLifetime.ApplicationStarted.Register(() =>
        {
            Task.Run(async () =>
            {
                _logger.LogInformation("Running console hosted service ...");

                try
                {
                    string fileName = await _uspsDownloadUtil.Download();
                    HashSet<string> hashSet = _excelFileReaderUtil.GetZipCodesFromXls(fileName);

                    await _fileOperationsUtil.SaveToGitRepo(hashSet);

                    _logger.LogInformation("Complete!");

                    _exitCode = 0;
                }
                catch (Exception e)
                {
                    if (Debugger.IsAttached)
                        throw;

                    _logger.LogError(e, "Unhandled exception");

                    await Task.Delay(2000);
                    _exitCode = 1;
                }
                finally
                {
                    // Stop the application once the work is done
                    _appLifetime.StopApplication();
                }
            }, cancellationToken);
        });

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Exiting with return code: {exitCode}", _exitCode);

        // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        return Task.CompletedTask;
    }
}