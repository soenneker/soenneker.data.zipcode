using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Soenneker.Data.ZipCode.Utils.Abstract;
using Soenneker.Utils.FileSync;
using Soenneker.Utils.HttpClientCache.Abstract;

namespace Soenneker.Data.ZipCode.Utils;

///<inheritdoc cref="IUspsDownloadUtil"/>
public class UspsDownloadUtil : IUspsDownloadUtil
{
    private readonly ILogger<UspsDownloadUtil> _logger;
    private readonly IHttpClientCache _httpClientCache;

    public UspsDownloadUtil(IHttpClientCache httpClientCache, ILogger<UspsDownloadUtil> logger)
    {
        _httpClientCache = httpClientCache;
        _logger = logger;
    }

    public async ValueTask GetData()
    {
        HttpClient client = await _httpClientCache.Get(nameof(UspsDownloadUtil));
        HttpResponseMessage message = await client.GetAsync("https://postalpro.usps.com/ZIP_Locale_Detail");
        string html = await message.Content.ReadAsStringAsync();

        var date = await GetDateFromPage(html);
    }

    public async ValueTask<DateTime?> GetDateFromPage(string html)
    {
        try
        {
            HtmlDocument htmlDoc = new();
            htmlDoc.LoadHtml(html);

            HtmlNode myDivNode = htmlDoc.DocumentNode.SelectSingleNode("//div[@class='mb-2']");

            return Convert.ToDateTime(myDivNode.InnerText);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error parsing page");
        }

        return null;
    }

    public async ValueTask<string> Download()
    {
        HttpClient client = await _httpClientCache.Get(nameof(UspsDownloadUtil));

        var uri = $"https://postalpro.usps.com/mnt/glusterfs/{GetDirectory()}/ZIP_Locale_Detail.xls";

        _logger.LogInformation("Downloading file from uri ({uri}) ...", uri);

        HttpResponseMessage response = await client.GetAsync(uri);

        string tempFile = FileUtilSync.GetTempFileName() + ".xls";

        using (var fs = new FileStream(tempFile, FileMode.CreateNew))
        {
            await response.Content.CopyToAsync(fs);
        }

        _logger.LogDebug("Finished downloading file from uri ({uri})", uri);

        return tempFile;
    }

    public static string GetDirectory()
    {
        var result = DateTime.UtcNow.ToString("yyyy-MM");

        return result;
    }
}