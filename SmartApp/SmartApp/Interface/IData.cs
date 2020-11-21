using System.Threading.Tasks;

namespace SmartApp.Interface
{
    public interface IData
    {
        string GetData(string filename);
        byte[] ReadBinary(string filename);
    }
}
