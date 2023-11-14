using System;
using System.IO;
using System.Linq;
using LibGit2Sharp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Soenneker.Data.ZipCode.Utils.Abstract;

namespace Soenneker.Data.ZipCode.Utils;

///<inheritdoc cref="IGitUtil"/>
public class GitUtil : IGitUtil
{
    private readonly IConfiguration _config;
    private readonly ILogger<GitUtil> _logger;

    public GitUtil(IConfiguration config, ILogger<GitUtil> logger)
    {
        _config = config;
        _logger = logger;
    }

    public bool IsRepositoryDirty(string directory)
    {
        using (var repo = new Repository(directory))
        {
            RepositoryStatus status = repo.RetrieveStatus();
            return status.IsDirty;
        }
    }

    public string CloneToTempDirectory()
    {
        string dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

        Clone("https://github.com/soenneker/soenneker.data.zipcode", dir);

        return dir;
    }


    public bool IsRepository(string directory)
    {
        return Repository.IsValid(directory);
    }

    public void Clone(string uri, string directory)
    {
        _logger.LogInformation("Cloning uri ({uri}) into directory ({dir}) ...", uri, directory);

        try
        {
            Repository.Clone(uri, directory);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not clone uri ({uri}) into directory ({dir})", uri, directory);
        }

        _logger.LogDebug("Finished cloning uri ({uri}) into directory ({dir})", uri, directory);
    }

    public void Pull(string directory)
    {
        try
        {
            using (var repo = new Repository(directory))
            {
                Remote? remote = repo.Network.Remotes["origin"];

                _logger.LogInformation("Pulling from {url} in {directory}...", remote.Url, directory);

                MergeResult? mergeResult = Commands.Pull(repo, new Signature(_config["Package:Owner"], _config["Github:Email"], DateTimeOffset.UtcNow),
                    new PullOptions
                    {
                        FetchOptions = new FetchOptions(),
                        MergeOptions = new MergeOptions
                        {
                            FailOnConflict = true,
                            CommitOnSuccess = false
                        }
                    });

                if (mergeResult.Status == MergeStatus.Conflicts)
                    _logger.LogInformation("Conflicted, cannot merge");
                else
                    _logger.LogDebug("Completed pull");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not pull for directory {dir}", directory);
        }
    }

    public void Commit(string directory, string message, string name, string email)
    {
        try
        {
            if (!IsRepositoryDirty(directory))
            {
                _logger.LogInformation("No changes detected to commit, skipping");
                return;
            }

            using (var repo = new Repository(directory))
            {
                _logger.LogInformation("Committing changes in directory {directory}...", directory);

                var signature = new Signature(name, email, DateTimeOffset.UtcNow);

                // Adds files that are not indexed yet
                Commands.Stage(repo, "*");
                repo.Commit(message, signature, signature);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not commit for directory {dir}", directory);
        }
    }

    public void Push(string directory, string username, string token)
    {
        try
        {
            using (var repo = new Repository(directory))
            {
                Remote? remote = repo.Network.Remotes["origin"];

                _logger.LogInformation("Pushing changes to repo {url} in directory {directory}...", remote.Url, directory);

                if (!HasChangesToPush(repo))
                {
                    return;
                }

                var options = new PushOptions
                {
                    CredentialsProvider = (url, usernameFromUrl, types) =>
                        new UsernamePasswordCredentials
                        {
                            Username = username,
                            Password = token
                        }
                };

                Branch localMainBranch = repo.Branches["refs/heads/main"];

                repo.Network.Push(localMainBranch, options);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Could not push for directory {dir}", directory);
        }
    }

    public void AddIfNotExists(string directory, string relativeFilePath)
    {
        _logger.LogDebug("Adding file ({file}) to index if it doesn't exist ...", relativeFilePath);

        try
        {
            using (var repo = new Repository(directory))
            {
                if (IsFileInIndex(repo, relativeFilePath))
                    return;

                Commands.Stage(repo, relativeFilePath);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static bool IsFileInIndex(Repository repo, string relativeFilePath)
    {
        foreach (var entry in repo.Index)
        {
            if (entry.Path == relativeFilePath)
            {
                return true;
            }
        }

        return false;
    }

    private bool HasChangesToPush(IRepository repo)
    {
        Branch localMainBranch = repo.Branches["refs/heads/main"];

        Commit? localHeadCommit = localMainBranch.Commits.FirstOrDefault();

        Branch remoteMainBranch = repo.Branches["refs/remotes/origin/main"];

        Commit? remoteHeadCommit = remoteMainBranch.Commits.FirstOrDefault();

        if (remoteHeadCommit == null || localHeadCommit == null || localHeadCommit.Id == remoteHeadCommit.Id)
        {
            _logger.LogInformation("No changes detected, skipping push");
            return false;
        }

        return true;
    }
}