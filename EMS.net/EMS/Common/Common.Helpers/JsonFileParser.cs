using System.IO;
using Newtonsoft.Json;

namespace Common.Helpers
{
    public static class JsonFileParser
    {
        public static T Parse<T>(string fileName) where T : class
        {
            using (var metadataFileStream = new FileStream(fileName, FileMode.Open))
            using (var reader = new StreamReader(metadataFileStream))
            {
                var jsonString = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
        }
    }
}
