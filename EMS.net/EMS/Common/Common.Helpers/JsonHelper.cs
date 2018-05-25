using System;
using System.IO;
using Newtonsoft.Json;

namespace Common.Helpers
{
    public static class JsonHelper
    {
        /// <summary>
        /// Десереализует объект из файла. ( Абсолютный путь у файла)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T Deserialize<T>(string fileName) where T : class
        {
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            using (var reader = new StreamReader(fileStream))
            {
                var jsonString = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }
        }

        /// <summary>
        /// Сериализует данные в файл. (Абсолютный путь у файла)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string Serialize<T>(string filename, T data) where T:class 
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var serializedData = JsonConvert.SerializeObject(data);
            using (var writer = new StreamWriter(filename))
            {
                writer.Write(serializedData);
            }

            return filename;
        }
    }
}
