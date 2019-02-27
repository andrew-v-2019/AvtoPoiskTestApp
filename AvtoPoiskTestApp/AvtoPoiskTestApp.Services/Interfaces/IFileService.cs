
namespace AvtoPoiskTestApp.Services.Interfaces
{
    public interface IFileService
    {
        T ReadFromFile<T>(string path);
        void SaveToFile<T>(T item, string path);
        bool Exists(string path);
    }
}
