using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Data.ZipCode.Utils.Abstract;
using Soenneker.Git.Util.Abstract;
using Soenneker.Utils.Environment;
using Soenneker.Utils.File.Abstract;
using Soenneker.Utils.FileSync.Abstract;
using Soenneker.Utils.Json;

namespace Soenneker.Data.ZipCode.Utils;

///<inheritdoc cref="IFileOperationsUtil"/>
public class FileOperationsUtil : IFileOperationsUtil
{
    private readonly ILogger<FileOperationsUtil> _logger;
    private readonly IGitUtil _gitUtil;
    private readonly IFileUtil _fileUtil;
    private readonly IFileUtilSync _fileUtilSync;

    public FileOperationsUtil(IFileUtil fileUtil, ILogger<FileOperationsUtil> logger, IGitUtil gitUtil, IFileUtilSync fileUtilSync)
    {
        _fileUtil = fileUtil;
        _logger = logger;
        _gitUtil = gitUtil;
        _fileUtilSync = fileUtilSync;
    }
    
    public async ValueTask SaveToGitRepo(HashSet<string> hashSet)
    {
        string directory = _gitUtil.CloneToTempDirectory("https://github.com/soenneker/soenneker.data.zipcode");

        await WriteList(hashSet, directory);

        await WriteJson(hashSet, directory);
        
        _gitUtil.AddIfNotExists(directory, "zipcodes.txt");
        _gitUtil.AddIfNotExists(directory, "zipcodes.json");

        if (_gitUtil.IsRepositoryDirty(directory))
        {
            _logger.LogInformation("Changes have been detected in the repository, commiting and pushing...");

            string name = EnvironmentUtil.GetVariableStrict("Name");
            string email = EnvironmentUtil.GetVariableStrict("Email");
            string username = EnvironmentUtil.GetVariableStrict("Username");
            string token = EnvironmentUtil.GetVariableStrict("Token");

            _gitUtil.Commit(directory, "Automated update from USPS", name, email);

            await _gitUtil.Push(directory, username, token);
        }
        else
        {
            _logger.LogInformation("There are no changes to commit");
        }
    }

    private async ValueTask WriteJson(HashSet<string> hashSet, string directory)
    {
        string jsonPath = Path.Combine(directory, "zipcodes.json");

        _fileUtilSync.DeleteIfExists(jsonPath);

        string? serialized = JsonUtil.Serialize(hashSet);

        await _fileUtil.WriteFile(jsonPath, serialized!);
    }

    private async ValueTask WriteList(HashSet<string> hashSet, string directory)
    {
        string linesPath = Path.Combine(directory, "zipcodes.txt");

        _fileUtilSync.DeleteIfExists(linesPath);

        await _fileUtil.WriteAllLines(linesPath, hashSet);
    }
}