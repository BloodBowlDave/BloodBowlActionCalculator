using ActionCalculator.Utilities;

namespace ActionCalculator.Models
{
    public static class TeamLookups
    {
        private static readonly IReadOnlyList<(Team Team, StarPlayer StarPlayer)> _entries =
            new (Team, StarPlayer)[]
            {
                // Amazon
                (Team.Amazon, StarPlayer.AkhorneTheSquirrel),
                (Team.Amazon, StarPlayer.AnqiPanqi),
                (Team.Amazon, StarPlayer.BoaKonssstriktr),
                (Team.Amazon, StarPlayer.Crumbleberry),
                (Team.Amazon, StarPlayer.Dribl),
                (Team.Amazon, StarPlayer.Drull),
                (Team.Amazon, StarPlayer.EstelleLaVeneaux),
                (Team.Amazon, StarPlayer.GlotlStop),
                (Team.Amazon, StarPlayer.Grak),
                (Team.Amazon, StarPlayer.KarlaVonKill),
                (Team.Amazon, StarPlayer.MorgNThorg),
                (Team.Amazon, StarPlayer.ZolcathTheZoat),

                // Black Orc
                (Team.BlackOrc, StarPlayer.AkhorneTheSquirrel),
                (Team.BlackOrc, StarPlayer.BomberDribblesnot),
                (Team.BlackOrc, StarPlayer.Crumbleberry),
                (Team.BlackOrc, StarPlayer.FungusTheLoon),
                (Team.BlackOrc, StarPlayer.Grak),
                (Team.BlackOrc, StarPlayer.HtharkTheUnstoppable),
                (Team.BlackOrc, StarPlayer.MorgNThorg),
                (Team.BlackOrc, StarPlayer.NobblaBlackwart),
                (Team.BlackOrc, StarPlayer.RashnackBackstabber),
                (Team.BlackOrc, StarPlayer.RipperBolgrot),
                (Team.BlackOrc, StarPlayer.ScrappaSorehead),
                (Team.BlackOrc, StarPlayer.TheBlackGobbo),
                (Team.BlackOrc, StarPlayer.VaragGhoulChewer),

                // Bretonnian
                (Team.Bretonnian, StarPlayer.AkhorneTheSquirrel),
                (Team.Bretonnian, StarPlayer.BarikFarblast),
                (Team.Bretonnian, StarPlayer.CindyPiewhistle),
                (Team.Bretonnian, StarPlayer.Crumbleberry),
                (Team.Bretonnian, StarPlayer.FrankNStein),
                (Team.Bretonnian, StarPlayer.Grak),
                (Team.Bretonnian, StarPlayer.GriffOberwald),
                (Team.Bretonnian, StarPlayer.GrombrindaTheWhiteDwarf),
                (Team.Bretonnian, StarPlayer.HelmutWulf),
                (Team.Bretonnian, StarPlayer.IvarEriksson),
                (Team.Bretonnian, StarPlayer.JosefBugman),
                (Team.Bretonnian, StarPlayer.KarlaVonKill),
                (Team.Bretonnian, StarPlayer.MightyZug),
                (Team.Bretonnian, StarPlayer.MorgNThorg),
                (Team.Bretonnian, StarPlayer.PuggyBaconbreath),
                (Team.Bretonnian, StarPlayer.SkrorgSnowpelt),
                (Team.Bretonnian, StarPlayer.ThorssonStoutmead),

                // Chaos Chosen
                (Team.ChaosChosen, StarPlayer.AkhorneTheSquirrel),
                (Team.ChaosChosen, StarPlayer.Crumbleberry),
                (Team.ChaosChosen, StarPlayer.Grak),
                (Team.ChaosChosen, StarPlayer.GrashnakBlackhoof),
                (Team.ChaosChosen, StarPlayer.LordBorak),
                (Team.ChaosChosen, StarPlayer.MorgNThorg),

                // Chaos Dwarf
                (Team.ChaosDwarf, StarPlayer.AkhorneTheSquirrel),
                (Team.ChaosDwarf, StarPlayer.BomberDribblesnot),
                (Team.ChaosDwarf, StarPlayer.Crumbleberry),
                (Team.ChaosDwarf, StarPlayer.FungusTheLoon),
                (Team.ChaosDwarf, StarPlayer.Grak),
                (Team.ChaosDwarf, StarPlayer.HtharkTheUnstoppable),
                (Team.ChaosDwarf, StarPlayer.MorgNThorg),
                (Team.ChaosDwarf, StarPlayer.NobblaBlackwart),
                (Team.ChaosDwarf, StarPlayer.RashnackBackstabber),
                (Team.ChaosDwarf, StarPlayer.RipperBolgrot),
                (Team.ChaosDwarf, StarPlayer.ScrappaSorehead),
                (Team.ChaosDwarf, StarPlayer.TheBlackGobbo),
                (Team.ChaosDwarf, StarPlayer.VaragGhoulChewer),

                // Chaos Renegade
                (Team.ChaosRenegade, StarPlayer.AkhorneTheSquirrel),
                (Team.ChaosRenegade, StarPlayer.Crumbleberry),
                (Team.ChaosRenegade, StarPlayer.Grak),
                (Team.ChaosRenegade, StarPlayer.GrashnakBlackhoof),
                (Team.ChaosRenegade, StarPlayer.LordBorak),
                (Team.ChaosRenegade, StarPlayer.MorgNThorg),

                // Dark Elf
                (Team.DarkElf, StarPlayer.AkhorneTheSquirrel),
                (Team.DarkElf, StarPlayer.Crumbleberry),
                (Team.DarkElf, StarPlayer.EldrilSidewinder),
                (Team.DarkElf, StarPlayer.GlorielSummerbloom),
                (Team.DarkElf, StarPlayer.Grak),
                (Team.DarkElf, StarPlayer.JeremiahKool),
                (Team.DarkElf, StarPlayer.JordellFreshbreeze),
                (Team.DarkElf, StarPlayer.KirothKrakeneye),
                (Team.DarkElf, StarPlayer.LucienSwift),
                (Team.DarkElf, StarPlayer.MorgNThorg),
                (Team.DarkElf, StarPlayer.RoxannaDarknail),
                (Team.DarkElf, StarPlayer.ValenSwift),
                (Team.DarkElf, StarPlayer.ZolcathTheZoat),

                // Dwarf
                (Team.Dwarf, StarPlayer.AkhorneTheSquirrel),
                (Team.Dwarf, StarPlayer.BomberDribblesnot),
                (Team.Dwarf, StarPlayer.Crumbleberry),
                (Team.Dwarf, StarPlayer.FungusTheLoon),
                (Team.Dwarf, StarPlayer.Grak),
                (Team.Dwarf, StarPlayer.HtharkTheUnstoppable),
                (Team.Dwarf, StarPlayer.MorgNThorg),
                (Team.Dwarf, StarPlayer.NobblaBlackwart),
                (Team.Dwarf, StarPlayer.RashnackBackstabber),
                (Team.Dwarf, StarPlayer.RipperBolgrot),
                (Team.Dwarf, StarPlayer.ScrappaSorehead),
                (Team.Dwarf, StarPlayer.TheBlackGobbo),
                (Team.Dwarf, StarPlayer.VaragGhoulChewer),

                // Elven Union
                (Team.ElvenUnion, StarPlayer.AkhorneTheSquirrel),
                (Team.ElvenUnion, StarPlayer.Crumbleberry),
                (Team.ElvenUnion, StarPlayer.EldrilSidewinder),
                (Team.ElvenUnion, StarPlayer.GlorielSummerbloom),
                (Team.ElvenUnion, StarPlayer.Grak),
                (Team.ElvenUnion, StarPlayer.JeremiahKool),
                (Team.ElvenUnion, StarPlayer.JordellFreshbreeze),
                (Team.ElvenUnion, StarPlayer.KirothKrakeneye),
                (Team.ElvenUnion, StarPlayer.LucienSwift),
                (Team.ElvenUnion, StarPlayer.MorgNThorg),
                (Team.ElvenUnion, StarPlayer.RoxannaDarknail),
                (Team.ElvenUnion, StarPlayer.ValenSwift),
                (Team.ElvenUnion, StarPlayer.ZolcathTheZoat),

                // Gnome
                (Team.Gnome, StarPlayer.AkhorneTheSquirrel),
                (Team.Gnome, StarPlayer.CindyPiewhistle),
                (Team.Gnome, StarPlayer.Crumbleberry),
                (Team.Gnome, StarPlayer.Grak),
                (Team.Gnome, StarPlayer.GrombrindaTheWhiteDwarf),
                (Team.Gnome, StarPlayer.MorgNThorg),
                (Team.Gnome, StarPlayer.PuggyBaconbreath),
                (Team.Gnome, StarPlayer.RumbelowSheepskin),

                // Goblin
                (Team.Goblin, StarPlayer.AkhorneTheSquirrel),
                (Team.Goblin, StarPlayer.BomberDribblesnot),
                (Team.Goblin, StarPlayer.Crumbleberry),
                (Team.Goblin, StarPlayer.FungusTheLoon),
                (Team.Goblin, StarPlayer.Grak),
                (Team.Goblin, StarPlayer.HtharkTheUnstoppable),
                (Team.Goblin, StarPlayer.MorgNThorg),
                (Team.Goblin, StarPlayer.NobblaBlackwart),
                (Team.Goblin, StarPlayer.RashnackBackstabber),
                (Team.Goblin, StarPlayer.RipperBolgrot),
                (Team.Goblin, StarPlayer.ScrappaSorehead),
                (Team.Goblin, StarPlayer.TheBlackGobbo),
                (Team.Goblin, StarPlayer.VaragGhoulChewer),

                // Halfling
                (Team.Halfling, StarPlayer.AkhorneTheSquirrel),
                (Team.Halfling, StarPlayer.CindyPiewhistle),
                (Team.Halfling, StarPlayer.Crumbleberry),
                (Team.Halfling, StarPlayer.Grak),
                (Team.Halfling, StarPlayer.GrombrindaTheWhiteDwarf),
                (Team.Halfling, StarPlayer.MorgNThorg),
                (Team.Halfling, StarPlayer.PuggyBaconbreath),
                (Team.Halfling, StarPlayer.RumbelowSheepskin),

                // High Elf
                (Team.HighElf, StarPlayer.AkhorneTheSquirrel),
                (Team.HighElf, StarPlayer.Crumbleberry),
                (Team.HighElf, StarPlayer.EldrilSidewinder),
                (Team.HighElf, StarPlayer.GlorielSummerbloom),
                (Team.HighElf, StarPlayer.Grak),
                (Team.HighElf, StarPlayer.JeremiahKool),
                (Team.HighElf, StarPlayer.JordellFreshbreeze),
                (Team.HighElf, StarPlayer.KirothKrakeneye),
                (Team.HighElf, StarPlayer.LucienSwift),
                (Team.HighElf, StarPlayer.MorgNThorg),
                (Team.HighElf, StarPlayer.RoxannaDarknail),
                (Team.HighElf, StarPlayer.ValenSwift),
                (Team.HighElf, StarPlayer.ZolcathTheZoat),

                // Human
                (Team.Human, StarPlayer.AkhorneTheSquirrel),
                (Team.Human, StarPlayer.BarikFarblast),
                (Team.Human, StarPlayer.CindyPiewhistle),
                (Team.Human, StarPlayer.Crumbleberry),
                (Team.Human, StarPlayer.FrankNStein),
                (Team.Human, StarPlayer.Grak),
                (Team.Human, StarPlayer.GriffOberwald),
                (Team.Human, StarPlayer.GrombrindaTheWhiteDwarf),
                (Team.Human, StarPlayer.HelmutWulf),
                (Team.Human, StarPlayer.IvarEriksson),
                (Team.Human, StarPlayer.JosefBugman),
                (Team.Human, StarPlayer.KarlaVonKill),
                (Team.Human, StarPlayer.MightyZug),
                (Team.Human, StarPlayer.MorgNThorg),
                (Team.Human, StarPlayer.PuggyBaconbreath),
                (Team.Human, StarPlayer.SkrorgSnowpelt),
                (Team.Human, StarPlayer.ThorssonStoutmead),

                // Imperial Nobility
                (Team.ImperialNobility, StarPlayer.AkhorneTheSquirrel),
                (Team.ImperialNobility, StarPlayer.BarikFarblast),
                (Team.ImperialNobility, StarPlayer.CindyPiewhistle),
                (Team.ImperialNobility, StarPlayer.Crumbleberry),
                (Team.ImperialNobility, StarPlayer.FrankNStein),
                (Team.ImperialNobility, StarPlayer.Grak),
                (Team.ImperialNobility, StarPlayer.GriffOberwald),
                (Team.ImperialNobility, StarPlayer.GrombrindaTheWhiteDwarf),
                (Team.ImperialNobility, StarPlayer.HelmutWulf),
                (Team.ImperialNobility, StarPlayer.IvarEriksson),
                (Team.ImperialNobility, StarPlayer.JosefBugman),
                (Team.ImperialNobility, StarPlayer.KarlaVonKill),
                (Team.ImperialNobility, StarPlayer.MightyZug),
                (Team.ImperialNobility, StarPlayer.MorgNThorg),
                (Team.ImperialNobility, StarPlayer.PuggyBaconbreath),
                (Team.ImperialNobility, StarPlayer.SkrorgSnowpelt),
                (Team.ImperialNobility, StarPlayer.ThorssonStoutmead),

                // Khorne
                (Team.Khorne, StarPlayer.AkhorneTheSquirrel),
                (Team.Khorne, StarPlayer.Crumbleberry),
                (Team.Khorne, StarPlayer.Grak),
                (Team.Khorne, StarPlayer.GrashnakBlackhoof),
                (Team.Khorne, StarPlayer.LordBorak),
                (Team.Khorne, StarPlayer.MaxSpleenripper),
                (Team.Khorne, StarPlayer.MorgNThorg),
                (Team.Khorne, StarPlayer.ScylaAnfingrimm),

                // Lizardmen
                (Team.Lizardmen, StarPlayer.AkhorneTheSquirrel),
                (Team.Lizardmen, StarPlayer.AnqiPanqi),
                (Team.Lizardmen, StarPlayer.BoaKonssstriktr),
                (Team.Lizardmen, StarPlayer.Crumbleberry),
                (Team.Lizardmen, StarPlayer.Dribl),
                (Team.Lizardmen, StarPlayer.Drull),
                (Team.Lizardmen, StarPlayer.EstelleLaVeneaux),
                (Team.Lizardmen, StarPlayer.GlotlStop),
                (Team.Lizardmen, StarPlayer.Grak),
                (Team.Lizardmen, StarPlayer.KarlaVonKill),
                (Team.Lizardmen, StarPlayer.MorgNThorg),
                (Team.Lizardmen, StarPlayer.ZolcathTheZoat),

                // Necromantic Horror
                (Team.NecromanticHorror, StarPlayer.AkhorneTheSquirrel),
                (Team.NecromanticHorror, StarPlayer.CaptainKarinaVonRiesz),
                (Team.NecromanticHorror, StarPlayer.CountLuthorVonDrakenborg),
                (Team.NecromanticHorror, StarPlayer.Crumbleberry),
                (Team.NecromanticHorror, StarPlayer.FrankNStein),
                (Team.NecromanticHorror, StarPlayer.Grak),
                (Team.NecromanticHorror, StarPlayer.GretchenWachter),
                (Team.NecromanticHorror, StarPlayer.IvanTheAnimalDeathshroud),
                (Team.NecromanticHorror, StarPlayer.SkrullHalfheight),
                (Team.NecromanticHorror, StarPlayer.WilhelmChaney),

                // Norse
                (Team.Norse, StarPlayer.AkhorneTheSquirrel),
                (Team.Norse, StarPlayer.Crumbleberry),
                (Team.Norse, StarPlayer.Grak),
                (Team.Norse, StarPlayer.GrashnakBlackhoof),
                (Team.Norse, StarPlayer.LordBorak),
                (Team.Norse, StarPlayer.MaxSpleenripper),
                (Team.Norse, StarPlayer.MorgNThorg),
                (Team.Norse, StarPlayer.ScylaAnfingrimm),

                // Nurgle
                (Team.Nurgle, StarPlayer.AkhorneTheSquirrel),
                (Team.Nurgle, StarPlayer.BilerotVomitflesh),
                (Team.Nurgle, StarPlayer.Crumbleberry),
                (Team.Nurgle, StarPlayer.Grak),
                (Team.Nurgle, StarPlayer.GrashnakBlackhoof),
                (Team.Nurgle, StarPlayer.GufflePusmaw),
                (Team.Nurgle, StarPlayer.LordBorak),
                (Team.Nurgle, StarPlayer.MorgNThorg),
                (Team.Nurgle, StarPlayer.WithergraspDoubledrool),

                // Ogre
                (Team.Ogre, StarPlayer.AkhorneTheSquirrel),
                (Team.Ogre, StarPlayer.BomberDribblesnot),
                (Team.Ogre, StarPlayer.Crumbleberry),
                (Team.Ogre, StarPlayer.FungusTheLoon),
                (Team.Ogre, StarPlayer.Grak),
                (Team.Ogre, StarPlayer.HtharkTheUnstoppable),
                (Team.Ogre, StarPlayer.MorgNThorg),
                (Team.Ogre, StarPlayer.NobblaBlackwart),
                (Team.Ogre, StarPlayer.RashnackBackstabber),
                (Team.Ogre, StarPlayer.RipperBolgrot),
                (Team.Ogre, StarPlayer.ScrappaSorehead),
                (Team.Ogre, StarPlayer.TheBlackGobbo),
                (Team.Ogre, StarPlayer.VaragGhoulChewer),

                // Old World Alliance
                (Team.OldWorldAlliance, StarPlayer.AkhorneTheSquirrel),
                (Team.OldWorldAlliance, StarPlayer.BarikFarblast),
                (Team.OldWorldAlliance, StarPlayer.CindyPiewhistle),
                (Team.OldWorldAlliance, StarPlayer.Crumbleberry),
                (Team.OldWorldAlliance, StarPlayer.FrankNStein),
                (Team.OldWorldAlliance, StarPlayer.Grak),
                (Team.OldWorldAlliance, StarPlayer.GriffOberwald),
                (Team.OldWorldAlliance, StarPlayer.GrombrindaTheWhiteDwarf),
                (Team.OldWorldAlliance, StarPlayer.HelmutWulf),
                (Team.OldWorldAlliance, StarPlayer.IvarEriksson),
                (Team.OldWorldAlliance, StarPlayer.JosefBugman),
                (Team.OldWorldAlliance, StarPlayer.KarlaVonKill),
                (Team.OldWorldAlliance, StarPlayer.MightyZug),
                (Team.OldWorldAlliance, StarPlayer.MorgNThorg),
                (Team.OldWorldAlliance, StarPlayer.PuggyBaconbreath),
                (Team.OldWorldAlliance, StarPlayer.SkrorgSnowpelt),
                (Team.OldWorldAlliance, StarPlayer.ThorssonStoutmead),

                // Orc
                (Team.Orc, StarPlayer.AkhorneTheSquirrel),
                (Team.Orc, StarPlayer.BomberDribblesnot),
                (Team.Orc, StarPlayer.Crumbleberry),
                (Team.Orc, StarPlayer.FungusTheLoon),
                (Team.Orc, StarPlayer.Grak),
                (Team.Orc, StarPlayer.HtharkTheUnstoppable),
                (Team.Orc, StarPlayer.MorgNThorg),
                (Team.Orc, StarPlayer.NobblaBlackwart),
                (Team.Orc, StarPlayer.RashnackBackstabber),
                (Team.Orc, StarPlayer.RipperBolgrot),
                (Team.Orc, StarPlayer.ScrappaSorehead),
                (Team.Orc, StarPlayer.TheBlackGobbo),
                (Team.Orc, StarPlayer.VaragGhoulChewer),

                // Shambling Undead
                (Team.ShamblingUndead, StarPlayer.AkhorneTheSquirrel),
                (Team.ShamblingUndead, StarPlayer.CaptainKarinaVonRiesz),
                (Team.ShamblingUndead, StarPlayer.CountLuthorVonDrakenborg),
                (Team.ShamblingUndead, StarPlayer.Crumbleberry),
                (Team.ShamblingUndead, StarPlayer.FrankNStein),
                (Team.ShamblingUndead, StarPlayer.Grak),
                (Team.ShamblingUndead, StarPlayer.GretchenWachter),
                (Team.ShamblingUndead, StarPlayer.IvanTheAnimalDeathshroud),
                (Team.ShamblingUndead, StarPlayer.SkrullHalfheight),
                (Team.ShamblingUndead, StarPlayer.WilhelmChaney),

                // Skaven
                (Team.Skaven, StarPlayer.AkhorneTheSquirrel),
                (Team.Skaven, StarPlayer.BomberDribblesnot),
                (Team.Skaven, StarPlayer.Crumbleberry),
                (Team.Skaven, StarPlayer.FungusTheLoon),
                (Team.Skaven, StarPlayer.GlartSmarshrip),
                (Team.Skaven, StarPlayer.Grak),
                (Team.Skaven, StarPlayer.HakflemSkuttlespike),
                (Team.Skaven, StarPlayer.KreekTheVerminatorRustgouger),
                (Team.Skaven, StarPlayer.MorgNThorg),
                (Team.Skaven, StarPlayer.NobblaBlackwart),
                (Team.Skaven, StarPlayer.RipperBolgrot),
                (Team.Skaven, StarPlayer.ScrappaSorehead),
                (Team.Skaven, StarPlayer.SkitterStabStab),
                (Team.Skaven, StarPlayer.TheBlackGobbo),

                // Snotling
                (Team.Snotling, StarPlayer.AkhorneTheSquirrel),
                (Team.Snotling, StarPlayer.BomberDribblesnot),
                (Team.Snotling, StarPlayer.Crumbleberry),
                (Team.Snotling, StarPlayer.FungusTheLoon),
                (Team.Snotling, StarPlayer.GlartSmarshrip),
                (Team.Snotling, StarPlayer.Grak),
                (Team.Snotling, StarPlayer.HakflemSkuttlespike),
                (Team.Snotling, StarPlayer.KreekTheVerminatorRustgouger),
                (Team.Snotling, StarPlayer.MorgNThorg),
                (Team.Snotling, StarPlayer.NobblaBlackwart),
                (Team.Snotling, StarPlayer.RipperBolgrot),
                (Team.Snotling, StarPlayer.ScrappaSorehead),
                (Team.Snotling, StarPlayer.SkitterStabStab),
                (Team.Snotling, StarPlayer.TheBlackGobbo),

                // Tomb Kings
                (Team.TombKings, StarPlayer.AkhorneTheSquirrel),
                (Team.TombKings, StarPlayer.CaptainKarinaVonRiesz),
                (Team.TombKings, StarPlayer.CountLuthorVonDrakenborg),
                (Team.TombKings, StarPlayer.Crumbleberry),
                (Team.TombKings, StarPlayer.FrankNStein),
                (Team.TombKings, StarPlayer.Grak),
                (Team.TombKings, StarPlayer.GretchenWachter),
                (Team.TombKings, StarPlayer.IvanTheAnimalDeathshroud),
                (Team.TombKings, StarPlayer.SkrullHalfheight),
                (Team.TombKings, StarPlayer.WilhelmChaney),

                // Underworld Denizens
                (Team.UnderworldDenizens, StarPlayer.AkhorneTheSquirrel),
                (Team.UnderworldDenizens, StarPlayer.BomberDribblesnot),
                (Team.UnderworldDenizens, StarPlayer.Crumbleberry),
                (Team.UnderworldDenizens, StarPlayer.FungusTheLoon),
                (Team.UnderworldDenizens, StarPlayer.GlartSmarshrip),
                (Team.UnderworldDenizens, StarPlayer.Grak),
                (Team.UnderworldDenizens, StarPlayer.HakflemSkuttlespike),
                (Team.UnderworldDenizens, StarPlayer.KreekTheVerminatorRustgouger),
                (Team.UnderworldDenizens, StarPlayer.MorgNThorg),
                (Team.UnderworldDenizens, StarPlayer.NobblaBlackwart),
                (Team.UnderworldDenizens, StarPlayer.RipperBolgrot),
                (Team.UnderworldDenizens, StarPlayer.ScrappaSorehead),
                (Team.UnderworldDenizens, StarPlayer.SkitterStabStab),
                (Team.UnderworldDenizens, StarPlayer.TheBlackGobbo),

                // Vampire
                (Team.Vampire, StarPlayer.AkhorneTheSquirrel),
                (Team.Vampire, StarPlayer.CaptainKarinaVonRiesz),
                (Team.Vampire, StarPlayer.CountLuthorVonDrakenborg),
                (Team.Vampire, StarPlayer.Crumbleberry),
                (Team.Vampire, StarPlayer.FrankNStein),
                (Team.Vampire, StarPlayer.Grak),
                (Team.Vampire, StarPlayer.GretchenWachter),
                (Team.Vampire, StarPlayer.IvanTheAnimalDeathshroud),
                (Team.Vampire, StarPlayer.SkrullHalfheight),
                (Team.Vampire, StarPlayer.WilhelmChaney),

                // Wood Elf
                (Team.WoodElf, StarPlayer.AkhorneTheSquirrel),
                (Team.WoodElf, StarPlayer.Crumbleberry),
                (Team.WoodElf, StarPlayer.EldrilSidewinder),
                (Team.WoodElf, StarPlayer.GlorielSummerbloom),
                (Team.WoodElf, StarPlayer.Grak),
                (Team.WoodElf, StarPlayer.JeremiahKool),
                (Team.WoodElf, StarPlayer.JordellFreshbreeze),
                (Team.WoodElf, StarPlayer.KirothKrakeneye),
                (Team.WoodElf, StarPlayer.LucienSwift),
                (Team.WoodElf, StarPlayer.MorgNThorg),
                (Team.WoodElf, StarPlayer.RoxannaDarknail),
                (Team.WoodElf, StarPlayer.ValenSwift),
                (Team.WoodElf, StarPlayer.ZolcathTheZoat),
            };

        private static readonly IReadOnlyDictionary<Team, IReadOnlyList<StarPlayer>> _byTeam =
            _entries
                .GroupBy(e => e.Team)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<StarPlayer>)g.Select(e => e.StarPlayer).ToList());

        private static readonly IReadOnlyDictionary<StarPlayer, IReadOnlyList<Team>> _byStarPlayer =
            _entries
                .GroupBy(e => e.StarPlayer)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyList<Team>)g.Select(e => e.Team).ToList());

        public static IReadOnlyList<StarPlayer> GetStarPlayers(Team team) =>
            _byTeam.TryGetValue(team, out var players) ? players : Array.Empty<StarPlayer>();

        public static IReadOnlyList<Team> GetTeams(StarPlayer starPlayer) =>
            _byStarPlayer.TryGetValue(starPlayer, out var teams) ? teams : Array.Empty<Team>();

        public static StarPlayerRule GetRule(StarPlayer starPlayer) =>
            StarPlayerRules.ByStarPlayer[starPlayer];
    }
}
