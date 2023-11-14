using System.Collections.Generic;
using System.Threading.Tasks;

namespace Soenneker.Data.ZipCode.Utils.Abstract;

public interface IFileOperationsUtil
{
    ValueTask SaveToGitRepo(HashSet<string> hashSet);
}