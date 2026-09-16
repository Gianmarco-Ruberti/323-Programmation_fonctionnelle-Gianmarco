using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DataSeries
{
    public class DataSeries<T>
    {
        private IEnumerable<T> _data;

        private DataSeries(IEnumerable<T> data) => _data = data;

        // Permet de créer une DataSeries à partir d'une liste de DataPoint<T>
        public static DataSeries<T> From(IEnumerable<T> source) => new DataSeries<T>(source);

        public IEnumerable<T> value => _data;
        public int Count => _data.Count();

        public DataSeries<T> Outliers(Func<T, bool> predicate)
            => DataSeries<T>.From(_data.Where(predicate));

        public void Sanitize(Func<T, bool> isInvalid)
        {
        _data = this.value.Where(item => !isInvalid(item)).ToList();
        }

        public static DataSeries<T> FromCsv(string path, Func<string[], T> parser)
        {
            var lines = File.ReadAllLines(path).Skip(1); // ignorer l'en-tête
            var items = lines.Select(line => parser(line.Split(',')));
            return new DataSeries<T>(items);
        }
    }
}