# Blood Bowl Action Calculator — Developer Reference

A probability calculator for Blood Bowl 2020. Players chain actions (dodges, blocks, passes, fouls, …) and the engine computes the exact success probability for every team-reroll budget.

---

## Working conventions

- **Never use git worktrees.** Always work directly in this repository.
- **Don't add comments to the code.** Make names carry the meaning, and explain reasoning in the PR or chat instead.
- **Warnings are errors.** `Directory.Build.props` sets `TreatWarningsAsErrors` for every project (NuGet vulnerability warnings NU1901–NU1904 excepted), so fix new warnings rather than suppressing them.
- Shared project settings (`TargetFramework`, `ImplicitUsings`, `Nullable`) live in `Directory.Build.props`; individual `.csproj` files only hold what is specific to them.

---

## Commands

```bash
# Build
dotnet build

# Run tests
dotnet test BloodBowl.sln

# Run web server
dotnet run --project ActionCalculator.Web/Server/ActionCalculator.Web.Server.csproj
# → http://localhost:5000
```

Engine tests are parametrised `[Theory]` cases in `ActionCalculator.Tests`; client page logic is tested in `ActionCalculator.Web.Client.Tests`. Running `dotnet test BloodBowl.sln` runs both, and CI does the same.

CI (`.github/workflows/master_bloodbowldave.yml`) runs the tests on every pull request to `master` and on every push to `master`; the client is only published and deployed to Azure Static Web Apps after the tests pass on `master`. Dependabot (`.github/dependabot.yml`) opens weekly, grouped NuGet and GitHub Actions update PRs.

---

## Project structure

| Project | Role |
|---|---|
| `ActionCalculator.Models` | Domain types: `Calculation`, `PlayerAction`, `Block`, `Player`, all enums (`ActionType`, `CalculatorSkills`, `Season`, …) |
| `ActionCalculator.Abstractions` | Shared interfaces: `ICalculator`, `IActionStrategy`, `ICalculationContext`, `IProHelper`, `IStrategyFactory`, … |
| `ActionCalculator.Utilities` | Small helpers: enum flag extensions, clamping, combinatorics |
| `ActionCalculator` | Core engine: all strategies, `Calculator`, `StrategyFactory`, parsers, `D6`, `ProHelper`, `ConsummateProfessionalHelper`, `CalculationContext` |
| `ActionCalculator.Tests` | xUnit engine tests (~330 parametrised cases across all action types, star players, validation and parse errors) |
| `ActionCalculator.Web/Client` | Blazor WASM UI: player editor, action builder, save/compare, season toggle. Page logic for editing calculations lives in `Editing/CalculationEditor` |
| `ActionCalculator.Web.Client.Tests` | xUnit tests for `CalculationEditor`, the page logic behind the Calculator and Advanced pages |
| `ActionCalculator.Web/Server` | Local development host only (serves the client on localhost); not part of the deployed site |

---

## Calculation notation

Calculations are encoded as compact strings used in URLs, cache keys, and test `InlineData`.

### Player and skill prefix
```
D,SF,L3,CP:R2,D3,C3
^^^^^^^^^^  player skills (comma-separated abbreviations)
           ^^^^^^^^ actions for that player
```
A `;` switches to a new player: `P1:2D2;P2:D3`. A star player's short name can replace the skill list: `Griff:R2,D3`. A `*` after a star player's name (`Grim*`, `Morg*`) means they are playing against their hated opponents; without it their Hatred is removed. A generic player with `H` always has Hatred available.

### Action tokens

| Token | Action | Notes |
|---|---|---|
| `Rn` | Rush (Go For It) n+ | `R2` = 2+, `R3` = 3+, … |
| `Dn` | Dodge n+ | `D3"` = Diving Tackle applies, `D4¬` = don't use Break Tackle |
| `En` | Leap n+ | `E3"` = Diving Tackle applies |
| `Un` | Pick Up n+ | |
| `Pn` | Pass n+ | `P4-2` = pass with a −2 modifier; `P2;C3` = pass then catch; `P3'` = don't reroll an inaccurate pass |
| `Cn` | Catch n+ | |
| `In` | Interception n+ | |
| `nDm` | Block n dice, m success faces | `2D2`, `3D3`, `-2D2` (negative = fractional) |
| `Ln` | Dauntless n+ | `L2,2D2\|1D2` = Dauntless 2+, then a 2-dice block on success or a 1-die block on failure |
| `Fn` | Foul n+ armour | `F8` = 8+ to break |
| `Kn` | Armour Break n+ | `K8` = 8+ to break |
| `Jn` | Injury n+ | |
| `B` | Bribe | used after a foul |
| `An` | Argue the Call n+ | used after a foul |
| `Tn` | Stab n+ | |
| `Wn` | Chainsaw n+ | |
| `Hn` | Throw Teammate n+ | `H4-1` = −1 modifier; `H4'` = don't reroll an inaccurate throw |
| `Gn` | Landing n+ | |
| `Mn` | Hail Mary Pass n+ | |
| `Sn` | Shadowing (difference n) | `'` = don't reroll a failure |
| `Nn` | Tentacles (difference n) | `'` = don't reroll a failure |
| `Yn` | Hypnotic Gaze n+ | `'` = don't reroll a failure |
| `n` | Generic rerollable n+ | |
| `a/b` | Generic non-rerollable, a in b chance | `4/6`, `27/512` |

`*` on any action means "use Pro on this action" (see "Results and the Pro choice" below).

### Block modifiers (appended to block token)
```
2D2!2    frenzy (2 non-critical-failure results → follow-up trigger)
2D2*     use Pro
2D2^     use Brawler even when a team reroll looks better
2D2%     use Hatred even when a team reroll looks better (Hatred is otherwise used automatically, like Brawler)
2D2'     no reroll on non-critical failures (Stand Firm path)
```

### Branches
```
D3{D3}        success-branch (rerollable):  main action, then branch on non-critical failure
D3[D3]        success-branch (non-rerollable): kills calculation on branch failure
2D2!2{1D2}    frenzy: main block → follow-up on push
(1/3)(2/3)    alternate branches (mutually exclusive outcomes)
```

### Season suffix
```
2D3,D3~S2   use Season 2 rules (default is Season 3, no suffix needed)
```

---

## Results and the Pro choice

`CalculationResult.Results[k]` is the chance of success **without using more than k team rerolls**, for the reroll budget the user entered. `Results[0]` is therefore "how often you get through without needing a team reroll at all", not "the chance if you had no rerolls".

When a player has Pro (or a similar reroll skill) and a team reroll is available, the engine uses the team reroll first and keeps Pro as a backup, because it is more likely to succeed. This shows the safest line. The calculator cannot know what a team reroll is worth later in the half, so the user chooses the line that saves rerolls with the "Use Pro" override (`*`), and "Use Brawler" (`^`) works the same way for blocks. This is deliberate: don't "optimise" the choice inside the engine.

The FAQ page and a tooltip on the results table's "Rerolls Used" header explain this to users.

`Calculator.Calculate` throws `InvalidCalculationResultException` if any result is negative, above 1, or decreases from one column to the next. The client catches it and shows an error on the results card.

---

## Core architecture

### Calculator flow

```
Calculate(calculation)
  sets context.Season, _results[]
  → Resolve(p=1, r=Rerolls, i=-1, usedSkills=None)
      loops through PlayerActions
      → Execute(playerAction, …)
          → strategyFactory.GetActionStrategy(…)
          → strategy.Execute(p, r, i, playerAction, usedSkills)
              calls calculator.Resolve(p × outcome, r′, i, usedSkills′) for each branch
              end-of-calculation paths call WriteResult(p, r, …)
                  _results[Rerolls - r] += p
  AggregateResults()   // make cumulative: results[1] += results[0], etc.
  validate results     // throws InvalidCalculationResultException
```

### IActionStrategy.Execute parameters

```csharp
void Execute(
    decimal p,                     // cumulative probability reaching this point
    int r,                         // team rerolls still available
    int i,                         // index of current PlayerAction in the list
    PlayerAction playerAction,     // action + player
    CalculatorSkills usedSkills,   // bitmask of skills consumed so far
    bool nonCriticalFailure = false
);
```

A strategy resolves outcomes by calling `calculator.Resolve(p * fraction, r′, i, usedSkills′)`. It never writes results directly.

### usedSkills bitmask rules

`usedSkills` is a `[Flags] ulong` bitmask tracking which skills have been spent this calculation path.

**Reset on player switch** — only these flags carry across players:
```csharp
// Calculator.GetUsedSkills — everything else is cleared on player switch
usedSkills & (DivingTackle | BlastIt | CloudBurster)
```

**Once-per-game skills** — mark themselves so they can never fire twice:
- `SavageBlow` — passed as `usedSkills | SavageBlow`
- `ConsummateProfessional` (Season 3) — resolved as `usedSkills | Pro` (shares the Pro slot)

**Shared slot** — `OldPro` and `ConsummateProfessional` both map to `Pro` as their underlying skill in `Player.CanUseSkill`, so they can never both fire in the same calculation.

**Per-action skills that pass `None`** (do not prevent reuse):
`Brawler`, `Hatred` — each block action resets whether they are available; they use `CalculatorSkills.None` in their `usedSkills` argument so the flag is never set.

### ICalculationContext

```csharp
public interface ICalculationContext
{
    Season Season { get; set; }
    ActionType? PreviousActionType { get; set; }
}
```

Registered as singleton. `Calculator.Calculate()` sets `Season` before execution begins, and `Calculator.Resolve` sets `PreviousActionType` before each action. Strategies inject `ICalculationContext` to branch on season or on the previous action (for example `StrategyFactory` picks `FoulInjuryStrategy` after a foul).

---

## Season differences

| Behaviour | Season 2 | Season 3 |
|---|---|---|
| **Consummate Professional** | Pro reroll with 100 % success — fires anywhere ProHelper is called | +1 modifier to one agility test (Dodge, Leap, Catch, Pick-Up, Landing); adds one face (`1/6`) only when the target after modifiers (Diving Tackle, Break Tackle, an inaccurate landing) is 3+ to 6+ |
| **Hatred** | Not available to generic players | Available in player editor |
| **Lone Fouler** | Not available | Available |
| **Mighty Blow / Dirty Player** | Up to +2 | +1 only (except Dwarven Scourge) |
| **BreakTackle max value** | Capped at 2 in UI | No cap |
| **Working in Tandem, Woodland Fury, Lord of Chaos** | Not applied | Applied (`BlockSkillsHelper`) |
| **The Ballista on a pass** | Rerolls the pass | Not applied |

`StarPlayerRules.ByShortName(season)` also adjusts some star players per season (for example Varag gets Crushing Blow instead of Krump and Smash in Season 2, and Jordell loses Steady Footing).

**ProHelper:** in Season 2, Consummate Professional counts as a Pro-style reroll. In Season 3 it does not; it is handled as a modifier instead.

**Consummate Professional in Season 3** lives in `ConsummateProfessionalHelper.GetAgilityRollSuccess`. The agility strategies call it and resolve the extra chance with `usedSkills | CalculatorSkills.Pro`:
```csharp
var consummateProfessionalSuccess = consummateProfessionalHelper.GetAgilityRollSuccess(usedSkills, action.Roll, canUseSkill);
if (consummateProfessionalSuccess > 0)
{
    calculator.Resolve(p * consummateProfessionalSuccess, r, i, usedSkills | CalculatorSkills.Pro);
}
failure -= consummateProfessionalSuccess;
```

---

## CalculatorSkills enum

`[Flags] ulong`. Skills marked `[HideFromPlayerEditor]` are star-player-only or internal.

| Abbreviation | Skill | Editor |
|---|---|---|
| `D` | Dodge | ✓ |
| `BT` | BreakTackle | ✓ |
| `SH` | SureHands | ✓ |
| `SF` | SureFeet | ✓ |
| `PA` | Pass | ✓ |
| `CB` | CloudBurster | ✓ |
| `C` | Catch | ✓ |
| `DC` | DivingCatch | ✓ |
| `B` | Brawler | ✓ |
| `CL` | Claw | ✓ |
| `MB` | MightyBlow | ✓ |
| `P` | Pro | ✓ |
| `L` | Loner | ✓ |
| `DP` | DirtyPlayer | ✓ |
| `SG` | SneakyGit | ✓ |
| `SFO` | SteadyFooting | ✓ |
| `H` | Hatred | ✓ (S3 only) |
| `LF` | LoneFouler | ✓ (S3 only) |
| `DT` | DivingTackle | hidden |
| `CP` | ConsummateProfessional | hidden |
| `OP` | OldPro | hidden |
| `TB` | TheBallista | hidden |
| `MD` | MesmerisingDance | hidden |
| `R` | Ram | hidden |
| `BB` | BrutalBlock | hidden |
| `SM` | SavageMauling | hidden |
| `CR` | CrushingBlow | hidden |
| `S` | Slayer | hidden |
| `BR` | BlindRage | hidden |
| `BI` | BlastIt | hidden |
| `WD` | WhirlingDervish | hidden |
| `SB` | SavageBlow | hidden |
| `UM` | UnstoppableMomentum | hidden |
| `DS` | DwarvenScourge | hidden |
| `LC` | LordOfChaos | hidden |
| `HL` | HalflingLuck | hidden |
| `TMT` | ThinkingMansTroll | hidden |
| `BL` | BoundingLeap | hidden |
| `MA` | MasterAssassin | hidden |
| `WIT` | WorkingInTandem | hidden |
| `KS` | KrumpAndSmash | hidden |
| `WF` | WoodlandFury | hidden |
| `SPG` | StrongPassingGame | hidden |

`Loner` and `BreakTackle` store a numeric value (the roll threshold) appended to their abbreviation: `L4`, `BT2`.  
`DirtyPlayer` and `MightyBlow` store a modifier value: `DP1`, `MB1`.

---

## ActionType enum

Each value is the character used as its parser prefix.

| Value | Parser prefix |
|---|---|
| `ArgueTheCall` | `A` |
| `Bribe` | `B` |
| `Catch` | `C` |
| `Dodge` | `D` |
| `Leap` | `E` |
| `Foul` | `F` |
| `Landing` | `G` |
| `ThrowTeammate` | `H` |
| `Interception` | `I` |
| `Injury` | `J` |
| `ArmourBreak` | `K` |
| `Dauntless` | `L` |
| `HailMaryPass` | `M` |
| `Tentacles` | `N` |
| `Pass` | `P` |
| `Rush` | `R` |
| `Shadowing` | `S` |
| `Stab` | `T` |
| `PickUp` | `U` |
| `Chainsaw` | `W` |
| `Hypnogaze` | `Y` |
| `Block` | `nD` (any token with a `D` after the first character) |
| `NonRerollable` | `a/b` (any token containing `/`) |
| `Rerollable` | bare number |

---

## Star players

Star players are addressed by short name (e.g. `Griff:R2,2D2,D3`). Their skills are pre-built from `StarPlayerRules.ByShortName(season)`.

| Short name | Special skill | Calculator skills |
|---|---|---|
| `Akhorne` | Blind Rage | `D,CL,L4,BR` |
| `Anqi` | Savage Blow | `L4,SB` |
| `Barik` | Blast It | `SH,PA,L4,BI` |
| `Borak` | Lord of Chaos | `MB,L3,DP1,SG,LC` (S2: `L4,DP2`) |
| `Dribl` | A Sneaky Pair¹ | `D,L4,DP1,SG` |
| `Drull` | A Sneaky Pair¹ | `D,L4` |
| `Eldril` | Mesmerising Dance | `D,C,L4,MD` |
| `Frank` | Brutal Block | `BT,MB,L4,BB` |
| `Fungus` | Whirling Dervish | `MB,L4,WD` |
| `Gloriel` | Shot to Nothing¹ | `D,SH,PA,L3` |
| `Gretchen` | Incorporeal¹ | `D,L4` |
| `Griff` | Consummate Professional | `D,SF,L3,CP` |
| `H'thark` | Unstoppable Momentum | `BT,SF,L4,UM` |
| `Helmut` | Old Pro | `P,L4,OP` |
| `Ivan` | Dwarven Scourge | `L4,DS,H` |
| `Jordell` | Swift as the Breeze | `D,DC,L4,SFO` (S2: no `SFO`) |
| `Lucien` | Working in Tandem | `MB,L4,WIT` |
| `Morg` | The Ballista | `MB,L4,TB,H` (S2: no `H`) |
| `Puggy` | Halfling Luck | `D,L3,HL` (S2: `L4`) |
| `Ripper` | Thinking Man's Troll | `MB,L4,TMT` |
| `Rowana` | Bounding Leap | `D,L4,BL` |
| `Rumbelow` | Ram | `L4,R` |
| `Skitter` | Master Assassin | `D,L4,MA` |
| `Skrull` | Strong Passing Game | `SH,PA,L4,SPG` |
| `Varag` | Krump and Smash | `MB,L4,H,KS` (S2: `CR` instead of `KS`) |
| `Wilhelm` | Savage Mauling | `C,CL,L4,SM` |
| `Willow` | Woodland Fury | `L4,WF` |
| `Zug` | Crushing Blow | `MB,L4,CR` |

¹ Not modelled as a calculator skill (no dice-roll mechanic the calculator handles). The star player uses only the listed skills.

The remaining star players (68 in total) have no special calculator skill and use their listed skill abbreviations (Dodge, Loner, Mighty Blow, etc. as applicable).

---

## Key implementation notes

### Frenzy (CalculationEditor.SetPushes)
A Frenzy follow-up block is inserted as a second `PlayerAction` immediately after the main block with `RequiresNonCriticalFailure = true`. When removing Frenzy, `IsFrenzyFollowUp` checks `NumberOfNonCriticalFailures > 0` on the *previous* block — always check this **before** setting `NumberOfNonCriticalFailures = 0`, or the check will fail.

### Savage Blow (BlockStrategy / FractionalDiceBlockStrategy)
Anqi's once-per-game block reroll. Uses `usedSkills | CalculatorSkills.SavageBlow` so it can never fire twice. For fractional dice (`-2D` / `-3D`), the number of failures determines which helper (`SavageBlowOneFailure`, `SavageBlowTwoFailures`, `SavageBlowThreeFailures`) to call.

### OldPro vs ConsummateProfessional
Both share `Pro` as their underlying skill in `Player.CanUseSkill`. Only one can fire per calculation path. In Season 3, CP is handled as a modifier (not via ProHelper), and ProHelper will not treat CP as a Pro reroll.

### Unstoppable Momentum and Working in Tandem (BlockStrategy)
H'thark's per-block free reroll of one die, and Lucien's equivalent in Season 3. Implemented in `BlockStrategy.RerollOneDice`, which rerolls the lowest-value die across all 6 possible outcomes. Placed after Hatred and before Pro in `ProcessRoll`. Uses `CalculatorSkills.None` in the `usedSkills` argument (like Brawler and Hatred) so it fires on every block action. Does not chain into a team reroll or Pro after failing.

### SneakyGit and doubles
`FoulStrategy` skips the "sent off on a double" roll entirely when SneakyGit is active — `var rollDouble = canUseSneakyGit ? 0 : d6.RollDouble(roll, 12)`.

### Parse errors
`PlayerActionsBuilder.Build` throws `CalculationParseException` with a short message (for example `Could not read the action "D3x"`) instead of returning an empty calculation. The Advanced page catches it and shows the message under the calculation field.

### Season enum vs UI strings
`Calculation.Season` is a `Season` enum (`Season2` / `Season3`). The Blazor UI stores and passes season as a `string` ("Season 2" / "Season 3") via `[CascadingParameter]` and local storage. Convert at the boundary using `ParseSeason(string)` in `Index.razor.cs`. Never store the string in `Calculation`.
