using DataSeries;
using EsportApp;

DataSeries<ValorantMatch> valorant;
DataSeries<Cs2Match> cs2;
DataSeries<LolMatch> lol;
valorant = DataSeries<ValorantMatch>.FromCsv("data/valorant.csv", ParseValorant);
cs2 = DataSeries<Cs2Match>.FromCsv("data/cs2.csv", ParseCS2);
lol = DataSeries<LolMatch>.FromCsv("data/lol.csv", ParseLoL);
// Valorant : filtre les statistiques aberrantes
valorant.Sanitize(m =>
    m.Kills < 0 || m.Kills > 50 ||
    m.Deaths < 0 || m.Deaths > 30 ||
    m.Assists < 0
);

// CS2 : filtre les K+A impossibles ou décès négatifs
cs2.Sanitize(m =>
    m.Kills + m.Assists > 50 ||
    m.Deaths < 0
);

// LoL : filtre les valeurs anormales
lol.Sanitize(m =>
    m.Kills > 10 ||
    m.Deaths < 1 ||
    m.Assists < 0 ||
    m.Cs < 0
);

// Si des arguments sont transmis en ligne de commande, exécuter le CLI
if (args.Length > 0)
{
    Cli.Run(args);
    return;
}

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
var baaad = valorant.Outliers(m => m.Kills < 0);


foreach (var point in raphaelGenerated.value)
{
    Cs2Match match = point;

    Console.WriteLine($"Date: {point.Timestamp:dd/MM/yyyy}");
    Console.WriteLine($"Carte: {match.Map} | Côté: {match.StartSide}");
    Console.WriteLine($"K/D/A: {match.Kills}/{match.Deaths}/{match.Assists}");
    Console.WriteLine($"Résultat: {(match.Won ? "Victoire" : "Défaite")}");
    Console.WriteLine(new string('-', 30));
}

Console.WriteLine(raphaelGenerated.Count);
Console.WriteLine($"Valorant : {valorant.Count} matchs");
valorant.Sanitize(m => m.Kills < 9);
Console.WriteLine(valorant.Count); // 24

Console.WriteLine($"CS2      : {cs2.Count} matchs");
Console.WriteLine($"LoL      : {lol.Count} matchs");
// Total : 75 matchs