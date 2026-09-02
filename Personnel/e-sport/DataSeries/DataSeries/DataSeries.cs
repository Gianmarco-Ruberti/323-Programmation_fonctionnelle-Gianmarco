using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataSeries
{
    public class DataSeries<T>
    {
        private readonly IEnumerable<T> _data;

        private DataSeries(IEnumerable<T> data) => _data = data;

        public static DataSeries<T> From(IEnumerable<T> source) => new DataSeries<T>(source);

        public int Count => _data.Count();
        public IEnumerable<T> Values => _data;

        public static DataSeries<T> FromCsv(string path, Func<string[], T> parser)
        {
            {
                var lines = File.ReadAllLines(path).Skip(1); // ignorer l'en-tête
                return new DataSeries<T>(lines.Select(line => parser(line.Split(','))));
            }
        }
    }
}
