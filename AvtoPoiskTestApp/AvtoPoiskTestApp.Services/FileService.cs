using System.IO;
using AvtoPoiskTestApp.Services.Interfaces;
using Newtonsoft.Json;

namespace AvtoPoiskTestApp.Services
{
    public class FileService: IFileService
    {
        public T ReadFromFile<T>(string path)
        {
            using (var r = new StreamReader(path))
            {
                var json = r.ReadToEnd();
                var items = GetObjectsFromJson<T>(json);
                return items;
            }
        }

        public void SaveToFile<T>(T item, string path)
        {
            using (var file = File.CreateText(path))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, item);
            }
        }


        private static T GetObjectsFromJson<T>(string json)
        {
            var items = JsonConvert.DeserializeObject<T>(json);
            return items;
        }
    }
}
