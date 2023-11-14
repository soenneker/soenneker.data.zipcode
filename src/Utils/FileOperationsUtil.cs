using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Soenneker.Data.ZipCode.Utils.Abstract;
using Soenneker.Extensions.String;
using Soenneker.Utils.File.Abstract;
using Soenneker.Utils.Json;

namespace Soenneker.Data.ZipCode.Utils;

///<inheritdoc cref="IFileOperationsUtil"/>
public class FileOperationsUtil : IFileOperationsUtil
{
    private readonly ILogger<FileOperationsUtil> _logger;
    private readonly IGitUtil _gitUtil;
    private readonly IFileUtil _fileUtil;

    public FileOperationsUtil(IFileUtil fileUtil, ILogger<FileOperationsUtil> logger, IGitUtil gitUtil)
    {
        _fileUtil = fileUtil;

        _logger = logger;
        _gitUtil = gitUtil;
    }

    public async ValueTask SaveToGitRepo(HashSet<string> hashSet)
    {
        string directory = _gitUtil.CloneToTempDirectory();

        string linesPath = Path.Combine(directory, "ZipCodes.txt");

        _fileUtil.DeleteIfExists(linesPath);

        await _fileUtil.WriteAllLines(linesPath, hashSet);

        string jsonPath = Path.Combine(directory, "ZipCodes.json");

        _fileUtil.DeleteIfExists(jsonPath);

        string? serialized = JsonUtil.Serialize(hashSet);

        await _fileUtil.WriteFile(jsonPath, serialized!);

        _gitUtil.AddIfNotExists(directory, "ZipCodes.txt");
        _gitUtil.AddIfNotExists(directory, "ZipCodes.json");

        if (_gitUtil.IsRepositoryDirty(directory))
        {
            string name = Environment.GetEnvironmentVariable("Name")!;
            string email = Environment.GetEnvironmentVariable("Email")!;
            string username = Environment.GetEnvironmentVariable("Username")!;
            string token = Environment.GetEnvironmentVariable("Token")!;

            if (name.IsNullOrEmpty() || email.IsNullOrEmpty() || username.IsNullOrEmpty() || token.IsNullOrEmpty())
                throw new Exception("Environmental variables (Name, Email, Username, and Token) are all required");

            _gitUtil.Commit(directory, "Automated update from USPS", name, email);

            _gitUtil.Push(directory, username, token);
        }
    }
}