namespace Soenneker.Data.ZipCode.Utils.Abstract;

public interface IGitUtil
{
    bool IsRepositoryDirty(string directory);

    void Commit(string directory, string message, string name, string email);

    void Push(string directory, string username, string token);

    void Clone(string uri, string directory);

    string CloneToTempDirectory();

    void AddIfNotExists(string directory, string relativeFilePath);
}