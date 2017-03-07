using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Butler
{
    internal class QueryProvider
    {
        private static readonly Lazy<QueryProvider> Lazy = new Lazy<QueryProvider>(() => new QueryProvider());
        private readonly string _datafile = Path.Combine(Environment.CurrentDirectory, @"Data\Query.xml");

        private QueryProvider()
        {
        }

        public List<QueryInfoModel> Data { get; set; }

        public static QueryProvider Instance
        {
            get { return Lazy.Value; }
        }

        public event EventHandler Changed;

        private void OnChanged(EventArgs e)
        {
            var handler = Changed;

            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void Load()
        {
            Data = DeserializeFromXml<List<QueryInfoModel>>(_datafile);
        }

        public void Save()
        {
            OnChanged(EventArgs.Empty);
            SerializeToXml(Data, _datafile);
        }

        private static void SerializeToXml<T>(T obj, string fileName)
        {
            using (var fileStream = new FileStream(fileName, FileMode.Create))
            {
                var ser = new XmlSerializer(typeof (T));
                ser.Serialize(fileStream, obj);
            }
        }

        private static T DeserializeFromXml<T>(string fileName)
        {
            T result;
            using (var fileStream = new FileStream(fileName, FileMode.Open))
            {
                var ser = new XmlSerializer(typeof (T));
                result = (T) ser.Deserialize(fileStream);
            }
            return result;
        }
    }
}