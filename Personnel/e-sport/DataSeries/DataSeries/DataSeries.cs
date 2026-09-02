using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSeries
{
    public class DataSeries<T>
    {
        private readonly IEnumerable<DataPoint<T>> _data;

        private DataSeries(IEnumerable<DataPoint<T>> data) => _data = data;

        // Permet de créer une DataSeries à partir d'une liste de T (en créant un DataPoint par défaut)
        public static DataSeries<T> From(IEnumerable<T> source)
        {
            var points = source.Select(item => new DataPoint<T>(DateTime.Now, item));
            return new DataSeries<T>(points);
        }

        // Permet de créer une DataSeries à partir d'une liste de DataPoint<T>
        public static DataSeries<T> From(IEnumerable<DataPoint<T>> source) => new DataSeries<T>(source);

        public IEnumerable<T> Values => _data.Select(dp => dp.Value);
        public IEnumerable<DataPoint<T>> DataPoints => _data;
        public int Count => _data.Count();

        public static DataSeries<T> FromCsv(string path, Func<string[], T> parser)
        {
            var lines = File.ReadAllLines(path).Skip(1); // ignorer l'en-tête
            var items = lines.Select(line => new DataPoint<T>(DateTime.Now, parser(line.Split(','))));
            return new DataSeries<T>(items);
        }
    }
}