using System.Threading.Tasks;

namespace Soenneker.Data.ZipCode.Utils.Abstract;

public interface IUspsDownloadUtil
{
    ValueTask<string> Download();

    ValueTask GetData();
}