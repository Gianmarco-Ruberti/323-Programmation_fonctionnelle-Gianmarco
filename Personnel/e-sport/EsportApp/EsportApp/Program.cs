using DataSeries;
using EsportApp;
DataSeries<DataPoint<ValorantMatch>> valorant;
DataSeries<DataPoint<Cs2Match>> cs2;
DataSeries<DataPoint<LolMatch>> lol;
valorant = DataSeries< DataPoint<ValorantMatch>>.FromCsv("data/valorant.csv", ParseValorant);
cs2 = DataSeries<DataPoint<Cs2Match>>.FromCsv("data/cs2.csv", ParseCS2);
lol = DataSeries<DataPoint<LolMatch>>.FromCsv("data/lol.csv", ParseLoL);
DataPoint<Cs2Match> ParseCS2(string[] cols)
{
    return new DataPoint<Cs2Match>
        (
            DateTime.Parse(cols[0]), 
            new Cs2Match
            (
                cols[1], 
                cols[2], 
                cols[3], 
                int.Parse(cols[4]), 
                int.Parse(cols[5]), 
                int.Parse(cols[6]), 
                int.Parse(cols[7]),
                bool.Parse(cols[8])
            )
        );
}

DataPoint<LolMatch> ParseLoL(string[] cols)
{
    return new DataPoint<LolMatch>
        (
            DateTime.Parse(cols[0]), 
            new LolMatch
            (
                cols[1], 
                cols[2], 
                int.Parse(cols[4]), 
                int.Parse(cols[5]), 
                int.Parse(cols[6]), 
                int.Parse(cols[7]), 
                int.Parse(cols[8]),
                bool.Parse(cols[8])
            )   
        );
}
DataPoint<ValorantMatch> ParseValorant(string[] cols)
{
    ValorantMatch match = new ValorantMatch(
    cols[1],              // player
    cols[2],              // agent
    int.Parse(cols[3]),   // kills
    int.Parse(cols[4]),   // deaths
    int.Parse(cols[5]),   // assists
    int.Parse(cols[6]),   // headshots
    int.Parse(cols[7]),   // roundsWon
    bool.Parse(cols[8])   // won
    );
    DateTime date = DateTime.Parse(cols[0]);
    return new DataPoint<ValorantMatch>(date, match);
}

Console.WriteLine($"Valorant : {valorant.Count} matchs");
Console.WriteLine($"CS2      : {cs2.Count} matchs");
Console.WriteLine($"LoL      : {lol.Count} matchs");
// Total : 75 matchs