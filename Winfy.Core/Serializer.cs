using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace Winfy.Core {
    public static class Serializer {

        public static void SerializeToXml<T>(T instance, string filename) {
            File.WriteAllBytes(filename, SerializeToXml(instance));
        }
        public static byte[] SerializeToXml<T>(T instance) {
            using (var memoryStream = new MemoryStream()) {
                using (var writer = new StreamWriter(memoryStream, Encoding.UTF8)) {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(writer, instance);
                }
                return memoryStream.ToArray();
            }
        }

        public static T DeserializeFromXml<T>(string filename) {
            return DeserializeFromXml<T>(File.ReadAllBytes(filename));
        }
        public static T DeserializeFromXml<T>(byte[] data) {
            using (var memoryStream = new MemoryStream(data)) {
                using (var reader = new StreamReader(memoryStream, Encoding.UTF8)) {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(reader);
                }
            }
        }

        public static void SerializeToJson<T>(T instance, string filename) {
            if(string.IsNullOrEmpty(filename))
                throw new ArgumentException("filename");

            if (!Directory.Exists(Path.GetDirectoryName(filename)))
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

            File.WriteAllBytes(filename, SerializeToJson(instance));
        }
        public static byte[] SerializeToJson<T>(T instance) {
            using (var memoryStream = new MemoryStream()) {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(memoryStream, instance);
                return memoryStream.ToArray();
            }
        }

        public static T DeserializeFromJson<T>(string filename) {
            return DeserializeFromJson<T>(File.ReadAllBytes(filename));
        }
        public static T DeserializeFromJson<T>(byte[] data) {
            using (var memoryStream = new MemoryStream(data)) {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(memoryStream);
            }
        }
        public static T DeserializeFromJson<T>(Stream stream) {
            var serializer = new DataContractJsonSerializer(typeof (T));
            return (T)serializer.ReadObject(stream);
        }

    }
}
