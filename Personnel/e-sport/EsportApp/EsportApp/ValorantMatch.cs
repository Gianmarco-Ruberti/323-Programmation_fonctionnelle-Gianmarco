using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EsportApp
{
    public class ValorantMatch
    {
        public ValorantMatch(string timestamp, string player, string agent, int kills, int deaths, int assists, int headshots, int rounds_won, bool won)
        {
            Timestamp = DateTime.Parse(timestamp);
            Player = player;
            Agent = agent;
            Kills = kills;
            Deaths = deaths;
            Assists = assists;
            Headshots = headshots;
            Rounds_won = rounds_won;
            Won = won;
        }

        public DateTime Timestamp { get; }
        public string Player { get; }
        public string Agent { get; }
        public int Kills { get; }
        public int Deaths { get; }
        public int Assists { get; }
        public int Headshots { get; }
        public int Rounds_won { get; }
        public bool Won { get; }
    }
}
