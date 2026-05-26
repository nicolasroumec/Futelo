using Futelo.Shared.Enums;

namespace Futelo.Client.Shared;

public static class AchievementMeta
{
    public record BadgeInfo(string Name, string Icon, string Description, string Category);

    public static readonly IReadOnlyDictionary<AchievementType, BadgeInfo> All =
        new Dictionary<AchievementType, BadgeInfo>
        {
            // Títulos
            [AchievementType.FirstTitle]           = new("Primer Título",              "🏆", "Tu primer campeonato",                                          "Títulos"),
            [AchievementType.Treble]               = new("El Triplete",                "🥇", "Ganaste la Liga, la Copa y la Supercopa en una misma temporada", "Títulos"),
            [AchievementType.HatTrickTitles]       = new("Hat-Trick de Títulos",       "🎩", "Ganaste el mismo campeonato tres veces",                        "Títulos"),
            [AchievementType.Dynasty]              = new("Dinastía",                   "👑", "Cinco títulos en total",                                        "Títulos"),
            [AchievementType.ChampionOfChampions]  = new("Campeón de Campeones",       "⚡", "Ganaste la Liga, la Copa y la Supercopa al menos una vez",      "Títulos"),
            [AchievementType.Cinderella]           = new("La Cenicienta",              "🔮", "Ganaste la Liga partiendo con el ELO más bajo",                 "Títulos"),

            // Temporada perfecta
            [AchievementType.Invincible]           = new("Invicto",                    "🛡️", "Ninguna derrota en toda la liga",                              "Temporada perfecta"),
            [AchievementType.PerfectSeason]        = new("Temporada Perfecta",         "⭐", "Ganaste todos los partidos de liga",                            "Temporada perfecta"),
            [AchievementType.BrickWall]            = new("Muro de Acero",              "🧱", "Cero goles en contra en toda la liga",                          "Temporada perfecta"),
            [AchievementType.UnderdogStory]        = new("Historia del Underdog",      "📈", "Ganaste la liga con ELO por debajo de la media",               "Temporada perfecta"),

            // Rachas
            [AchievementType.OnFire]               = new("En Llamas",                  "🔥", "5 victorias consecutivas",                                      "Rachas"),
            [AchievementType.Unstoppable]          = new("Imparable",                  "💥", "10 victorias consecutivas",                                     "Rachas"),
            [AchievementType.ComebackKing]         = new("Rey del Comeback",           "💪", "5 victorias seguidas tras 3 derrotas consecutivas",             "Rachas"),
            [AchievementType.MrConsistent]         = new("Mr. Consistente",            "📊", "Liga con 60%+ victorias y máximo 2 derrotas seguidas",          "Rachas"),

            // ELO / Ranking
            [AchievementType.ElitePlayer]          = new("Jugador de Élite",           "🔵", "ELO histórico superior a 1600",                                 "ELO / Ranking"),
            [AchievementType.TopPlayer]            = new("Top Jugador",                "🟣", "ELO histórico superior a 1700",                                 "ELO / Ranking"),
            [AchievementType.Legend]               = new("Leyenda",                    "⭐", "ELO histórico superior a 1800",                                 "ELO / Ranking"),
            [AchievementType.KingOfTheHill]        = new("Rey de la Colina",           "🏔️", "10 cambios de ELO seguidos siendo número 1",                  "ELO / Ranking"),
            [AchievementType.MeteoricRise]         = new("Ascenso Meteórico",          "🚀", "Temporada con +100 de ELO ganado",                              "ELO / Ranking"),
            [AchievementType.Phoenix]              = new("Ave Fénix",                  "🦅", "Caíste a 1450 ELO y te recuperaste hasta 1600",                "ELO / Ranking"),

            // Goles
            [AchievementType.GoldenBoot]           = new("Bota de Oro",               "👟", "Máximo goleador de la temporada con 20+ goles",                 "Goles"),
            [AchievementType.Poacher]              = new("Punta de Lanza",             "⚽", "5 o más goles en un solo partido",                              "Goles"),
            [AchievementType.Sniper]               = new("El Francotirador",           "🎯", "10 victorias exactas por 1-0",                                  "Goles"),

            // Defensivos
            [AchievementType.CleanSheetStreak3]    = new("La Pared",                   "🧱", "3 partidos consecutivos sin encajar gol",                       "Defensivos"),
            [AchievementType.CleanSheetStreak5]    = new("El Bastión",                 "🏰", "5 partidos consecutivos sin encajar gol",                       "Defensivos"),
            [AchievementType.CleanSheetStreak7]    = new("La Muralla China",           "🏯", "7 partidos consecutivos sin encajar gol",                       "Defensivos"),
            [AchievementType.FinalCleanSheet]      = new("El Candado",                 "🔒", "Ganaste una final sin encajar gol",                             "Defensivos"),

            // Curiosos
            [AchievementType.IceVeins]             = new("Sangre Fría",               "❄️", "Ganaste 3 partidos por penales",                                "Curiosos"),
            [AchievementType.GiantKiller]          = new("Matagigantas",              "⚔️", "Venciste a alguien con 200+ ELO más que tú",                   "Curiosos"),
            [AchievementType.Veteran]              = new("El Veterano",               "🎖️", "100 partidos disputados en el vault",                          "Curiosos"),
            [AchievementType.NeverGiveUp]          = new("El que Nunca se Rinde",     "💫", "Ganaste la liga tras haber tenido 3 derrotas seguidas",         "Curiosos"),
            [AchievementType.TheEqualizer]         = new("El Igualador",              "⚖️", "5 empates consecutivos",                                       "Curiosos"),
        };

    public static string GetName(AchievementType type)
        => All.TryGetValue(type, out var info) ? info.Name : type.ToString();

    public static string GetIcon(AchievementType type)
        => All.TryGetValue(type, out var info) ? info.Icon : "🏅";

    public static string GetDescription(AchievementType type)
        => All.TryGetValue(type, out var info) ? info.Description : string.Empty;

    public static string GetCategory(AchievementType type)
        => All.TryGetValue(type, out var info) ? info.Category : "Otros";
}
