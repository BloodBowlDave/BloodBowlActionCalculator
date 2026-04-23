using System.ComponentModel;

namespace ActionCalculator.Models
{
    public enum StarPlayerSkill
    {
        [Description("Akhorne may re-roll the D6 when rolling for the Dauntless skill.")]
        BlindRage = 1,

        [Description("Once per game, when Anqi performs a Block, he may re-roll any number of the Block dice.")]
        SavageBlow,

        [Description("Whenever Barik makes a Hail Mary Pass, he may re-roll any Scatter results to determine where the ball lands. Team-mates attempting to catch also gain +1 to the roll.")]
        BlastIt,

        [Description("Once per half, Bilerot may use Projectile Vomit as a Special Action. This can be used even if he has already performed a Block this turn.")]
        PutridRegurgitation,

        [Description("Once per game, if Boa starts his activation Marking an opponent carrying the ball, roll a D6. On a 2+ Boa steals the ball and his activation immediately ends.")]
        LookIntoMyEyes,

        [Description("No description available.")]
        Kaboom,

        [Description("Once per game, when Karina fails a Bloodlust roll, she may bite an opposition player with ST 3 or lower as if they were a Thrall team-mate. Cannot target Star Players.")]
        TastyMorsel,

        [Description("Once per game, Cindy may throw two Bombs instead of one, committing to both before the first throw. After the second, roll a D6: on a 1, 2 or 3 Cindy is immediately Sent-off.")]
        AllYouCanEat,

        [Description("Once per game, when Count Luthor scores a Touchdown, his coach gains one Team Re-roll valid until end of the following Drive. Unused re-rolls are lost.")]
        StarOfTheShow,

        [Description("Must be hired as a pair. Once per half, if Grak begins activation adjacent to Crumbleberry, Grak may carry Crumbleberry. Remove Crumbleberry from the pitch and place Crumbleberry in an adjacent empty square at the end of Grak's activation.")]
        IllCarryYou,

        [Description("Must be hired as a pair. Whenever either Dribl or Drull fouls or Stabs an opponent who is Marked by both of them, they gain +1 to the roll.")]
        ASneakyPair,

        [Description("Once per half, Eldril may re-roll the dice when performing a Hypnotic Gaze.")]
        MesmerisingDance,

        [Description("Once per game at the start of her activation, Estelle picks an opponent within 5 squares and rolls a D6. On a 2+ that player becomes Distracted and cannot be activated during the opponent's next turn.")]
        BalefulHex,

        [Description("No description available.")]
        BrutalBlock,

        [Description("Once per activation, Fungus may re-roll the direction die that determines his Ball & Chain movement.")]
        WhirlingDervish,

        [Description("Once per half, when Glart declares a Blitz he gains Frenzy for that activation. He cannot use Grab in the same turn he uses this rule.")]
        FrenziedRush,

        [Description("Once per game, Gloriel gains the Hail Mary Pass skill for the duration of her activation.")]
        ShotToNothing,

        [Description("Once per game, when Glotl fails an Animal Savagery roll, he may redirect his lash-out to an opposition player instead of a team-mate.")]
        PrimalSavagery,

        [Description("Once per game, when Grashnak Blitzes, he may roll one extra Block die against the target regardless of ST (maximum three total). If Frenzy triggers a second Block, that Block also benefits.")]
        GoredByTheBull,

        [Description("Once per game, for the rest of her activation Gretchen does not need to make Dodge rolls when leaving an opposition Tackle Zone.")]
        Incorporeal,

        [Description("Once per game, apply a +1 modifier to any Agility test Griff has rolled. This can be applied after seeing the roll's result.")]
        ConsummateProfessional,

        [Description("Once per game when activated, Grombrindal grants a team-mate within 2 squares one of the following skills until end of turn: Break Tackle, Dauntless, Mighty Blow, or Sure Feet.")]
        WisdomOfTheWhiteDwarf,

        [Description("Once per game, if an opponent Guffle is Marking catches the ball, Guffle may immediately make an Armour roll against them. If the armour breaks, Guffle takes possession. No Turnover is caused.")]
        QuickBite,

        [Description("When H'thark performs a Block as part of a Blitz, he may re-roll one Block die.")]
        UnstoppableMomentum,

        [Description("Once per game, if Hakflem is activated adjacent to a team-mate carrying the ball, he may steal it. The team-mate is immediately knocked down. This does not cause a Turnover even if a Casualty results.")]
        Treacherous,

        [Description("Once per game, Helmut may use his Pro skill to re-roll a single die from an Armour roll. Pro's usual restrictions still apply.")]
        OldPro,

        [Description("Once per game, when Ivan knocks down an opponent with a Block, gain +1 to the Armour or Injury roll. Against Dwarf players, this bonus is +2 instead. Can be applied after the roll.")]
        DwarfenScourge,

        [Description("Once per Drive at the start of his activation, Ivar may move one Open team-mate within 5 squares 1 square, so long as they end that move Marking an opponent.")]
        RaidingParty,

        [Description("Once per game at activation start, Jeremiah may Stab an opponent he is Marking, then follow up with a Move action before his activation ends.")]
        TheFlashingBlade,

        [Description("Once per game, Jordell may automatically pass a Dodge, Leap, or Rush test on a 2+ regardless of any modifiers.")]
        SwiftAsTheBreeze,

        [Description("Once per game, when Josef's armour is broken by an Armour roll, re-roll that Armour roll.")]
        DwarfenGrit,

        [Description("Once per game, when Karla successfully activates Dauntless, she may increase her ST to double the target's ST for that Block.")]
        Indomitable,

        [Description("Once per game at the start of an activation, Kiroth Distracts an opponent he is Marking. That player stays Distracted until they are next activated.")]
        BlackInk,

        [Description("The first time Kreek would be Sent-off for Secret Weapon, he stays on the pitch instead. His coach may not Argue the Call when this rule activates.")]
        IllBeBack,

        [Description("Once per game, re-roll one Block die when Lord Borak performs a Block action.")]
        LordOfChaos,

        [Description("Must be hired alongside each other. If Lucien blocks an opponent who is also Marked by Valen, Lucien may re-roll one Block die.")]
        WorkingInTandem,

        [Description("Once per game, after performing a Chainsaw Attack, Max may immediately make another Chainsaw Attack against a different opponent.")]
        MaximumCarnage,

        [Description("Once per game, when Zug knocks down an opponent with a Block, gain +1 to the Armour roll. Can be applied after the roll.")]
        CrushingBlow,

        [Description("Once per game, Morg may re-roll the Passing Ability test for a Throw Team-mate action.")]
        TheBallista,

        [Description("Once per game, Nobbla may use the Chainsaw Attack against a Prone or Stunned opponent. This is not a Foul action, so he cannot be Sent-off for it.")]
        KickEmWhileTheyreDown,

        [Description("Once per game, re-roll any single die or one die from a multi-die roll. Cannot be used on Armour, Injury, or Casualty rolls.")]
        HalflingLuck,

        [Description("Once per game, when Rashnak breaks an opponent's armour with a Stab, gain +1 to the Injury roll. Can be applied after the roll.")]
        ToxinConnoisseur,

        [Description("Once per half, re-roll any single die or one die from a multi-die roll. Cannot be used on Armour, Injury, or Casualty rolls.")]
        ThinkingMansTroll,

        [Description("Once per half, when Roxanna declares a Blitz, she gains Claws for that activation.")]
        SlashingNails,

        [Description("Once per game, when Rumbelow knocks down an opponent with a Block, gain +1 to either the Armour or Injury roll. Can be applied after the roll.")]
        Ram,

        [Description("Once per game, if Scyla rolls a 1 on his Unchannelled Fury check after declaring a Block, he may instead perform two Block actions. The first must be fully resolved (including Frenzy) before the second.")]
        FuryOfTheBloodGod,

        [Description("Once per game, when Scrappa attempts an Interception, roll a D6. On a 2+ he automatically intercepts without needing to pass the normal test.")]
        Yoink,

        [Description("Once per game, when Skitter performs a Stab, he may re-roll the Armour roll.")]
        MasterAssassin,

        [Description("Once per game, when Skrorg causes a Casualty via a Block, his coach gains one Team Re-roll until the end of the current Drive. If unused by end of Drive, it is lost.")]
        PumpUpTheCrowd,

        [Description("Once per game, when Skrull makes a Pass, he may boost the Agility test result by up to his ST value, to a maximum result of 6.")]
        StrongPassingGame,

        [Description("Your team may declare two Foul Actions per turn, but one of them must be performed by the Black Gobbo himself.")]
        SneakiestOfTheLot,

        [Description("Once per Drive at activation start, pick an opponent within 3 squares and roll a D6: on 3+ they're knocked down; on 2 nothing happens; on 1 Thorsson falls over instead. His activation ends immediately after.")]
        BeerBarrelBash,

        [Description("Once per game, when Varag knocks down an opponent with a Block, re-roll the Armour roll.")]
        KrumpAndSmash,

        [Description("Once per game, when Wilhelm makes an Injury roll against an opponent, he may re-roll the result.")]
        SavageMauling,

        [Description("The first time each Drive that Withergrasp is targeted by a Block action, he counts as having the Dodge skill.")]
        WatchOut,

        [Description("Once per game when activated, Zolcath selects an opposition player within 3 squares. That player immediately becomes Distracted.")]
        ExcuseMeAreYouAZoat,

        [Description("If Deeproot makes a Fumbled Throw when performing a Throw Team-mate Action, the player that was being thrown will Bounce as normal but will automatically land safely.")]
        Reliable,

        [Description("Once per game, when an opposition Big Guy is Knocked Down as a result of a Block Action performed by Grim, you may apply an additional +1 modifier to either the Armour Roll or the Injury Roll. This modifier may be applied after the roll has been made.")]
        Slayer,

        [Description("Once per half, when Maple declares a Block Action he may do so against an opposition player who is 2 squares away, following all the normal rules for performing a Block Action, though he may not follow up.")]
        ViciousVines,

        [Description("Once per half, if Rodney is Standing and begins his activation within 3 squares of a ball which is on the ground he may roll a D6. On a 3+, Rodney immediately gains possession of the ball.")]
        CatchOfTheDay,

        [Description("Once per game, after declaring that she will Leap but before rolling any dice, Rowana may choose to use this special rule. If she does, Rowana suffers no negative modifiers for the Agility Test to Leap and may choose to re-roll the result.")]
        BoundingLeap,

        [Description("Once per half, so long as she is Standing at the start of her activation, Swiftvine can place herself adjacent to a Standing opposition player within 3 squares and immediately make a Stab Special Action against them.")]
        FuriousOutburst,

        [Description("Once per game, when Willow performs a Block Action that would result in her being Knocked Down, she can choose to re-roll a single Block Dice.")]
        WoodlandFury,

        [Description("Once per half, at the start of his activation, Zzharg may select a Standing opposition player within 3 squares and roll a D6. On a 3+, the selected player is hit and an Armour Roll is made against them.")]
        BlastinSolvesEverything,
    }
}
