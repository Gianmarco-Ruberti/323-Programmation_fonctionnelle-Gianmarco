// See https://aka.ms/new-console-template for more information
using DataSeries;
using ESportApp;

var valorant = DataSerie<ValorantMatch>.FromCsv(@"C:\Users\px05iql\Documents\Github\323-MASTER\exos\fil-rouge\esport\data\valorant.csv", ParseValorant);
var cs2 = DataSerie<Cs2Match>.FromCsv(@"C:\Users\px05iql\Documents\Github\323-MASTER\exos\fil-rouge\esport\data\cs2.csv", ParseCS2);
var lol = DataSerie<LolMatch>.FromCsv(@"C:\Users\px05iql\Documents\Github\323-MASTER\exos\fil-rouge\esport\data\Lol.csv", ParseLoL);

Console.WriteLine($"Il y a  {valorant.Count} matches dans la série Valorant"); // 3
Console.WriteLine($"Il y a  {cs2.Count} matches dans la série CS2"); // 3
Console.WriteLine($"Il y a  {lol.Count} matches dans la série LoL"); // 3

Console.ReadKey();

ValorantMatch ParseValorant(string[] cols)
{
    return new ValorantMatch(cols[1], cols[2], int.Parse(cols[3]), int.Parse(cols[4]), int.Parse(cols[5]), int.Parse(cols[6]), int.Parse(cols[7]), cols[8] == "TRUE");
}
Cs2Match ParseCS2(string[] cols)
{
    return new Cs2Match(cols[1], cols[2], cols[3], int.Parse(cols[4]), int.Parse(cols[5]), int.Parse(cols[6]), int.Parse(cols[7]), cols[8] == "TRUE");
}
LolMatch ParseLoL(string[] cols)
{
    return new LolMatch(cols[1], cols[2], int.Parse(cols[4]), int.Parse(cols[5]), int.Parse(cols[6]), int.Parse(cols[7]), int.Parse(cols[8]), cols[9] == "TRUE");
}
