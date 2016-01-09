using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SteamPaver.Common
{
    public interface ICacheable
    {
        string InstanceID { get; }
    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class CacheLocationAttribute : Attribute
    {
        readonly string cacheFolderName;

        public CacheLocationAttribute(string cacheFolderName)
        {
            this.cacheFolderName = cacheFolderName;

        }

        public string CacheFolderName
        {
            get { return cacheFolderName; }
        }

    }


    public static class CacheManager
    {
        private static JsonSerializer _serializer = JsonSerializer.Create();
        
        public static IEnumerable<T> LoadFromCache<T>()
            where T : ICacheable
        {
            string folderPath = GetAndCreateFolder<T>();


            var itemPaths = Directory.GetFiles(folderPath, "*.json");
            foreach (var itemPath in itemPaths)
            {
                var json = File.ReadAllText(itemPath);
                using (var reader = new StringReader(json))
                {

                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var instance = _serializer.Deserialize<T>(jsonReader);

                        yield return instance;
                    }
                }
            }
        }


        public static T LoadFromCache<T>(string instanceID)
            where T : ICacheable
        {
            string folderPath = GetAndCreateFolder<T>();
            string path = Path.Combine(folderPath, $"{instanceID}.json");

            if (File.Exists(path))
            {

                var json = File.ReadAllText(path);
                using (var reader = new StringReader(json))
                {

                    using (var jsonReader = new JsonTextReader(reader))
                    {
                        var instance = _serializer.Deserialize<T>(jsonReader);

                        return instance;
                    }
                }
            }
            return default(T);
        }

        public static void SaveToCache<T>(this T obj)
            where T : ICacheable
        {

            string folderPath = GetAndCreateFolder<T>();
            string path = Path.Combine(folderPath, $"{obj.InstanceID}.json");


            using (var fs = File.OpenWrite(path))
            {
                using (var writer = new StreamWriter(fs))
                {
                    using (var jsonWriter = new JsonTextWriter(writer))
                    {
                        _serializer.Serialize(jsonWriter, obj);
                    }
                }
            }
        }

        public static void RemoveFromCache<T>(this T obj)
            where T : ICacheable
        {

            string folderPath = GetAndCreateFolder<T>();
            
            var itemPaths = Directory.EnumerateFiles(folderPath, $"{obj.InstanceID}.*");

            foreach(var itemPath in itemPaths)
            {
                File.Delete(itemPath);
            }

            if (!Directory.EnumerateFiles(folderPath).Any())
                Directory.Delete(folderPath);
        }

        private static string GetAndCreateFolder<T>()
        {

            var type = typeof(T);
            var atr = type.GetAtribute<CacheLocationAttribute>();
            string folderName = atr?.CacheFolderName ?? type.Name;

            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SteamPaver.Common", "Cache", folderName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
