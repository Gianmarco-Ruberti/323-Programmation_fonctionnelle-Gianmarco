using DataSeries;
using EsportApp;
using System.Linq;
using System.Text.RegularExpressions;

DataSeries<ValorantMatch> valorant;
DataSeries<Cs2Match> cs2;
DataSeries<LolMatch> lol;
valorant = DataSeries<ValorantMatch>.FromCsv("data/valorant.csv", ParseValorant);
cs2 = DataSeries<Cs2Match>.FromCsv("data/cs2.csv", ParseCS2);
lol = DataSeries<LolMatch>.FromCsv("data/lol.csv", ParseLoL);
void ExportCs2(DataSeries<Cs2Match> matches, string path)
{
    var header = "date,player,map,start_side,kills,deaths,assists,mvps,won";
    var lines = matches.value.Select(dp =>
        $"{dp.Timestamp:yyyy-MM-dd},{dp.Player},{dp.Map},{dp.StartSide}," +
        $"{dp.Kills},{dp.Deaths},{dp.Assists},{dp.Mvps},{dp.Won.ToString().ToLower()}"
    );
    File.WriteAllLines(path, lines.Prepend(header));
}
Cs2Match ParseCS2(string[] cols)
{
    return new Cs2Match
        (
                cols[0],
                cols[1],
                cols[2],
                cols[3],
                int.Parse(cols[4]),
                int.Parse(cols[5]),
                int.Parse(cols[6]),
                int.Parse(cols[7]),
                bool.Parse(cols[8])
        );
}

LolMatch ParseLoL(string[] cols)
{
    return new LolMatch
        (
                cols[0],
                cols[1],
                cols[2],
                cols[3],
                int.Parse(cols[4]),
                int.Parse(cols[5]),
                int.Parse(cols[6]),
                int.Parse(cols[7]),
                int.Parse(cols[8]),
                bool.Parse(cols[9])
        );
}
ValorantMatch ParseValorant(string[] cols)
{
    return new ValorantMatch
    (
    cols[0],              // date
    cols[1],              // player
    cols[2],              // agent
    int.Parse(cols[3]),   // kills
    int.Parse(cols[4]),   // deaths
    int.Parse(cols[5]),   // assists
    int.Parse(cols[6]),   // headshots
    int.Parse(cols[7]),   // roundsWon
    bool.Parse(cols[8])   // won
    );
}

var raphaelGenerated = MatchGenerator.GenerateCs2("Raphaël", 20);

foreach (var point in raphaelGenerated.value)
{
    Cs2Match match = point.Value;

    Console.WriteLine($"Date: {point.Timestamp:dd/MM/yyyy}");
    Console.WriteLine($"Carte: {match.Map} | Côté: {match.StartSide}");
    Console.WriteLine($"K/D/A: {match.Kills}/{match.Deaths}/{match.Assists}");
    Console.WriteLine($"Résultat: {(match.Won ? "Victoire" : "Défaite")}");
    Console.WriteLine(new string('-', 30));
}

Console.WriteLine(raphaelGenerated.Count);
Console.WriteLine($"Valorant : {valorant.Count} matchs");
Console.WriteLine($"CS2      : {cs2.Count} matchs");
Console.WriteLine($"LoL      : {lol.Count} matchs");
// Total : 75 matchs