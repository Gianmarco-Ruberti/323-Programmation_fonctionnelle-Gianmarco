using System;
using System.Collections.Generic;
using System.Linq;

namespace EsportApp
{
    /// <summary>
    /// Classe de données stockant la configuration et les options
    /// passées par l'utilisateur lors du lancement en ligne de commande.
    /// </summary>
    public class CliOptions
    {
        public string Game { get; set; } = "all";             // Jeu ciblé : "valorant", "cs2", "lol" ou "all"
        public string? Player { get; set; } = null;           // Nom du joueur à analyser (null = tous les joueurs)
        public string Filter { get; set; } = "all";           // Filtre de résultat : "wins", "losses" ou "all"
        public string Stat { get; set; } = "kda";             // Statistique calculée : "kda", "kills" ou "assists"
        public bool Normalize { get; set; } = false;          // Indique s'il faut normaliser les données entre 0.0 et 1.0
        public int Smooth { get; set; } = 1;                  // Taille de la fenêtre de lissage (1 = aucun lissage)
        public string? GenerateTarget { get; set; } = null;   // Cible pour la génération de données ("joueur", "all" ou null)
        public string ErrorMode { get; set; } = "soft";       // Mode de gestion des erreurs/anomalies : "strict", "soft" ou "hard"
    }

    /// <summary>
    /// Classe principale de gestion de la ligne de commande (CLI).
    /// </summary>
    public static class Cli
    {
        // Version actuelle de l'application
        private const string VersionText = "EsportApp 0.4";

        /// <summary>
        /// Point d'entrée principal pour traiter les arguments de la ligne de commande.
        /// </summary>
        public static void Run(string[] args)
        {
            // Si aucun argument n'est fourni, on affiche l'aide et on arrête l'exécution
            if (args.Length == 0)
            {
                ShowHelp();
                return;
            }

            // Instanciation des options avec leurs valeurs par défaut
            var options = new CliOptions();

            // Parcours et validation des arguments transmis
            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                // On compare l'argument en minuscules pour ignorer la casse
                switch (arg.ToLower())
                {
                    // Option d'aide
                    case "--help":
                    case "-h":
                        ShowHelp();
                        return;

                    // Option d'affichage de la version
                    case "--version":
                    case "-v":
                        ShowVersion();
                        return;

                    // Option pour sélectionner le jeu
                    case "--game":
                    case "-g":
                        // Récupère la valeur suivante et vérifie qu'elle fait partie des jeux autorisés
                        if (!TryGetNextArg(args, ref i, out string gameVal) ||
                            !new[] { "valorant", "cs2", "lol" }.Contains(gameVal.ToLower()))
                        {
                            Console.WriteLine($"Erreur : Valeur invalide pour --game. Attendu : valorant|cs2|lol.");
                            return;
                        }
                        options.Game = gameVal.ToLower();
                        break;

                    // Option pour spécifier le joueur
                    case "--player":
                        if (!TryGetNextArg(args, ref i, out string playerVal))
                        {
                            Console.WriteLine("Erreur : --player nécessite un nom de joueur.");
                            return;
                        }
                        options.Player = playerVal;
                        break;

                    // Option de filtrage des matchs (victoires, défaites, tous)
                    case "--filter":
                        if (!TryGetNextArg(args, ref i, out string filterVal) ||
                            !new[] { "wins", "losses", "all" }.Contains(filterVal.ToLower()))
                        {
                            Console.WriteLine($"Erreur : Valeur invalide '{filterVal}' pour --filter. Attendu : wins|losses|all.");
                            return;
                        }
                        options.Filter = filterVal.ToLower();
                        break;

                    // Option pour la statistique à calculer
                    case "--stat":
                        if (!TryGetNextArg(args, ref i, out string statVal) ||
                            !new[] { "kda", "kills", "assists" }.Contains(statVal.ToLower()))
                        {
                            Console.WriteLine($"Erreur : Valeur invalide '{statVal}' pour --stat. Attendu : kda|kills|assists.");
                            return;
                        }
                        options.Stat = statVal.ToLower();
                        break;

                    // Flag (indicateur) pour activer la normalisation
                    case "--normalize":
                        options.Normalize = true;
                        break;

                    // Option de lissage des données (moyenne glissante)
                    case "--smooth":
                        // Récupère la valeur, la convertit en entier et s'assure qu'elle est >= 1
                        if (!TryGetNextArg(args, ref i, out string smoothVal) ||
                            !int.TryParse(smoothVal, out int window) || window < 1)
                        {
                            Console.WriteLine($"Erreur : Valeur invalide '{smoothVal}' pour --smooth. Un entier positif (>= 1) est attendu.");
                            return;
                        }
                        options.Smooth = window;
                        break;

                    // Option pour la génération automatique de matchs
                    case "--generate":
                        if (!TryGetNextArg(args, ref i, out string genVal))
                        {
                            Console.WriteLine("Erreur : --generate nécessite un argument <joueur|all>.");
                            return;
                        }
                        options.GenerateTarget = genVal;
                        break;

                    // Option pour configurer le mode de traitement des erreurs
                    case "--error":
                        if (!TryGetNextArg(args, ref i, out string errorVal) ||
                            !new[] { "strict", "soft", "hard" }.Contains(errorVal.ToLower()))
                        {
                            Console.WriteLine($"Erreur : Valeur invalide '{errorVal}' pour --error. Attendu : strict|soft|hard.");
                            return;
                        }
                        options.ErrorMode = errorVal.ToLower();
                        break;
                    case "--extract":
                        if(!TryGetNextArg(args, ref i, out string extractVal))
                        {
                            Console.WriteLine("oui");
                            return;
                        }
                        break;

                    // Gestion des options non reconnues
                    default:
                        Console.WriteLine($"Erreur : Flag inconnu '{arg}'.");
                        Console.WriteLine("Utilisez --help pour afficher les options disponibles.");
                        return;
                }
            }

            // Lancement du traitement applicatif avec les options lues
            ExecutePipeline(options);
        }

        /// <summary>
        /// Tente de récupérer la valeur associée à une option (l'élément suivant dans le tableau d'arguments).
        /// Incrémente l'index si une valeur valide est trouvée.
        /// </summary>
        private static bool TryGetNextArg(string[] args, ref int index, out string value)
        {
            // Vérifie qu'il reste un élément et que cet élément ne commence pas par "--" (ce qui indiquerait un autre flag)
            if (index + 1 < args.Length && !args[index + 1].StartsWith("--"))
            {
                index++; // Avance le curseur d'index
                value = args[index];
                return true;
            }

            value = string.Empty;
            return false;
        }

        /// <summary>
        /// Affiche l'aide et les commandes disponibles de l'application dans la console.
        /// </summary>
        private static void ShowHelp()
        {
            Console.WriteLine(@"Usage: EsportApp [options]

  Analyse des performances de Team Helvetia (Valorant, CS2, LoL).

Sélection des données
  --game   valorant|cs2|lol    Jeu à analyser              (défaut : les trois)
  --player <nom>               Restreindre à un joueur     (défaut : tous)
  --filter wins|losses|all     Issue des matchs retenus    (défaut : all)

Analyse
  --stat   kda|kills|assists   Indicateur calculé/affiché  (défaut : kda)
  --normalize                  Ramène l'indicateur dans [0.0, 1.0]
  --smooth <n>                 Moyenne glissante sur n valeurs
                                 (normalisation puis lissage, dans cet ordre)

Données
  --generate <joueur|all>      Simule et exporte les matchs manquants, puis quitte
  --error  strict|soft|hard    Traitement des valeurs aberrantes (défaut : soft)
                                 strict : les affiche et s'arrête
                                 soft   : les élimine et continue
                                 hard   : les élimine, sauve le CSV nettoyé, continue

Divers
  --help                       Affiche cette aide
  --version                    Affiche la version");
        }

        /// <summary>
        /// Affiche la version applicative actuelle.
        /// </summary>
        private static void ShowVersion()
        {
            Console.WriteLine(VersionText);
        }

        /// <summary>
        /// Exécute les opérations métiers en fonction du paramétrage configuré.
        /// </summary>
        private static void ExecutePipeline(CliOptions options)
        {
            // Cas particulier : si la génération est demandée, on effectue la génération puis on arrête tout
            if (options.GenerateTarget != null)
            {
                Console.WriteLine($"Génération des matchs pour : {options.GenerateTarget}...");
                
                return;
            }

            // Cas standard : Affichage récapitulatif de la configuration retenue avant analyse
            Console.WriteLine($"Exécution du pipeline :");
            Console.WriteLine($"  - Jeu       : {options.Game}");
            Console.WriteLine($"  - Joueur    : {options.Player ?? "Tous"}");
            Console.WriteLine($"  - Filtre    : {options.Filter}");
            Console.WriteLine($"  - Stat      : {options.Stat}");
            Console.WriteLine($"  - Normalize : {options.Normalize}");
            Console.WriteLine($"  - Smooth    : {options.Smooth}");
            Console.WriteLine($"  - ErrorMode : {options.ErrorMode}");

            // Insérer ici le code d'analyse des données réelles
        }
    }
}