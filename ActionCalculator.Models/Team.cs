using System.ComponentModel;

namespace ActionCalculator.Models
{
    public enum Team
    {
        [Description("Amazon")]               Amazon = 1,
        [Description("Black Orc")]            BlackOrc,
        [Description("Bretonnian")]           Bretonnian,
        [Description("Chaos Chosen")]         ChaosChosen,
        [Description("Chaos Dwarf")]          ChaosDwarf,
        [Description("Chaos Renegade")]       ChaosRenegade,
        [Description("Dark Elf")]             DarkElf,
        [Description("Dwarf")]               Dwarf,
        [Description("Elven Union")]          ElvenUnion,
        [Description("Gnome")]               Gnome,
        [Description("Goblin")]              Goblin,
        [Description("Halfling")]            Halfling,
        [Description("High Elf")]            HighElf,
        [Description("Human")]              Human,
        [Description("Imperial Nobility")]   ImperialNobility,
        [Description("Khorne")]             Khorne,
        [Description("Lizardmen")]          Lizardmen,
        [Description("Necromantic Horror")] NecromanticHorror,
        [Description("Norse")]              Norse,
        [Description("Nurgle")]             Nurgle,
        [Description("Ogre")]               Ogre,
        [Description("Old World Alliance")] OldWorldAlliance,
        [Description("Orc")]                Orc,
        [Description("Shambling Undead")]   ShamblingUndead,
        [Description("Skaven")]             Skaven,
        [Description("Snotling")]           Snotling,
        [Description("Tomb Kings")]         TombKings,
        [Description("Underworld Denizens")]UnderworldDenizens,
        [Description("Vampire")]            Vampire,
        [Description("Wood Elf")]           WoodElf,
    }
}
