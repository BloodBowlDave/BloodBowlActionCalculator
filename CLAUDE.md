# Blood Bowl Action Calculator — Developer Reference

A probability calculator for Blood Bowl 2020. Players chain actions (dodges, blocks, passes, fouls, …) and the engine computes the exact success probability for every team-reroll budget.

---

## Commands

```bash
# Build
dotnet build

# Run tests
dotnet test ActionCalculator.Tests

# Run web server
dotnet run --project ActionCalculator.Web/Server/ActionCalculator.Web.Server.csproj
# → http://localhost:5000
```

All tests are parametrised `[Theory]` cases — no test names to remember. Running `dotnet test` is always sufficient.

---

## Project structure

| Project | Role |
|---|---|
| `ActionCalculator.Models` | Domain types: `Calculation`, `PlayerAction`, `Block`, `Player`, all enums (`ActionType`, `CalculatorSkills`, `Season`, …) |
| `ActionCalculator.Abstractions` | Shared interfaces: `ICalculator`, `IActionStrategy`, `ICalculationContext`, `IProHelper`, `IStrategyFactory`, … |
| `ActionCalculator.Utilities` | Small helpers: enum flag extensions, `D6`, clamping, combinatorics |
| `ActionCalculator` | Core engine: all strategies, `Calculator`, `StrategyFactory`, parsers, `ProHelper`, `CalculationContext` |
| `ActionCalculator.Tests` | xUnit tests (~230 parametrised cases across all action types and star players) |
| `ActionCalculator.Web.Client` | Blazor WASM UI: player editor, action builder, save/compare, season toggle |

---

## Calculation notation

Calculations are encoded as compact strings used in URLs, cache keys, and test `InlineData`.

### Player and skill prefix
```
D,SF,L3,CP:R2,D3,C3
^^^^^^^^  player skills (comma-separated abbreviations)
          ^^^^^^^^^^ actions for that player
```
A `;` switches to a new player: `P1:2D2;P2:D3`

### Action tokens

| Token | Action | Notes |
|---|---|---|
| `Rn` | Rush (Go For It) n+ | `R2` = 2+, `R3` = 3+, … |
| `Dn` | Dodge n+ | `D3` = 3+, `D4¬` = forced BT |
| `Un` | Pick Up n+ | |
| `Pn` | Pass n+ | `P2;C3` = pass then catch |
| `Cn` | Catch n+ | |
| `nDm` | Block n dice, m success faces | `2D2`, `3D3`, `-2D2` (negative = fractional) |
| `Fn` | Foul n+ armour | `F8` = 8+ to break |
| `Bn` | Armour Break n+ | |
| `Jn` | Injury n+ | |
| `Tn` | Stab n+ | |
| `Wn` | Chainsaw n+ | |
| `Hn` | Throw Teammate n+ | |
| `Gn` | Landing n+ | |
| `Mn` | Hail Mary Pass n+ | |
| `Sn` | Shadowing n+ | |
| `Nn` | Tentacles n+ | |
| `Yn` | Hypnotic Gaze n+ | |
| `n` | Generic rerollable n+ | |
| `Xn` | Generic non-rerollable n+ | |

### Block modifiers (appended to block token)
```
2D2!2    frenzy (2 non-critical-failure results → follow-up trigger)
2D2*     Pro was used
2D2^     Brawler was used
2D2%     Hatred was used
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

## Core architecture

### Calculator flow

```
Calculate(calculation)
  sets _context.Season, _results[]
  → Resolve(p=1, r=Rerolls, i=-1, usedSkills=None)
      loops through PlayerActions
      → Execute(playerAction, …)
          → _strategyFactory.GetActionStrategy(…)
          → strategy.Execute(p, r, i, playerAction, usedSkills)
              calls _calculator.Resolve(p × outcome, r′, i, usedSkills′) for each branch
              end-of-calculation paths call WriteResult(p, r, …)
                  _results[Rerolls - r] += p
  AggregateResults()   // make cumulative: results[1] += results[0], etc.
```

`CalculationResult.Results` is therefore indexed by *rerolls consumed*: `Results[0]` = P(success using zero team rerolls), `Results[1]` = P(success using at most one), etc.

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

A strategy resolves outcomes by calling `_calculator.Resolve(p * fraction, r′, i, usedSkills′)`. It never writes results directly.

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
    Season Season { get; set; }   // Season.Season2 | Season.Season3
}
```

Registered as singleton. `Calculator.Calculate()` sets `_context.Season = calculation.Season` before execution begins. Strategies inject `ICalculationContext` to branch on season.

---

## Season differences

| Behaviour | Season 2 | Season 3 |
|---|---|---|
| **Consummate Professional** | Pro reroll with 100 % success — fires anywhere ProHelper is called | +1 modifier to one agility test (Dodge, Catch, Pick-Up, Landing); roll of 1 always fails; adds exactly `1/6` to success when `roll > 2` |
| **Hatred** | Not available to generic players | Available in player editor |
| **Lone Fouler** | Not available | Available |
| **BreakTackle max value** | Capped at 2 in UI | No cap |

**ProHelper (Season 3):** CP is excluded from the Pro reroll check. The `cpIsProReroll` guard:
```csharp
var cpIsProReroll = _context.Season != Season.Season3;
```

**CP Season 3 pattern** (same in Dodge / Catch / PickUp / Landing):
```csharp
var cpS3Success = _context.Season == Season.Season3
    && canUseSkill(CalculatorSkills.ConsummateProfessional, usedSkills)
    && action.Roll > 2
    ? 1m / 6 : 0m;
if (cpS3Success > 0)
    _calculator.Resolve(p * cpS3Success, r, i, usedSkills | CalculatorSkills.Pro);
failure -= cpS3Success;
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
| `ASP` | ASneakyPair | hidden |
| `UM` | UnstoppableMomentum | hidden |
| `LC` | LordOfChaos | hidden |

`Loner` and `BreakTackle` store a numeric value (the roll threshold) appended to their abbreviation: `L4`, `BT2`.  
`DirtyPlayer` and `MightyBlow` store a modifier value: `DP1`, `MB1`.

---

## ActionType enum

| Value | Parser prefix |
|---|---|
| `Dodge` | `D` |
| `Rush` | `R` |
| `PickUp` | `U` |
| `Pass` | `P` |
| `Catch` | `C` |
| `Block` | `nD` |
| `Foul` | `F` |
| `ArmourBreak` | `B` |
| `Injury` | `J` |
| `Stab` | `T` |
| `Chainsaw` | `W` |
| `ThrowTeammate` | `H` |
| `Landing` | `G` |
| `HailMaryPass` | `M` |
| `Shadowing` | `S` |
| `Tentacles` | `N` |
| `Hypnogaze` | `Y` |
| `Dauntless` | `L` |
| `Interference` | `I` |
| `ArgueTheCall` | `A` |
| `Bribe` | `E` |
| `Rerollable` | bare number |
| `NonRerollable` | `X` |

---

## Star players

Star players are addressed by short name (e.g. `Griff:R2,2D2,D3`). Their skills are pre-built from `StarPlayerRules.ByShortName`.

| Short name | Special skill | Calculator skills |
|---|---|---|
| `Anqi` | Savage Blow | `L4,SB` |
| `Barik` | Blast It | `SH,PA,L4,BI` |
| `Dribl` | A Sneaky Pair | `D,L4,DP1,SG,ASP` |
| `Drull` | A Sneaky Pair | `D,L4,ASP` |
| `Eldril` | Mesmerising Dance | `D,C,L4,MD` |
| `Frank` | Brutal Block | `BT,MB,L4,BB` |
| `Fungus` | Whirling Dervish | `MB,L4,WD` |
| `Gloriel` | Shot to Nothing | `D,SH,PA,L3` |
| `Griff` | Consummate Professional | `D,SF,L3,CP` |
| `Gretchen` | Incorporeal¹ | `D,L4` |
| `Helmut` | Old Pro | `P,L4,OP` |
| `Morg` | The Ballista | `MB,L4,TB,H` |
| `Puggy` | Halfling Luck | `D,L3` |
| `H'thark` | Unstoppable Momentum | `BT,SF,L4,UM` |
| `Borak` | Lord of Chaos | `MB,L3,DP1,SG,LC` |
| `Rumbelow` | Ram | `L4,R` |
| `Wilhelm` | Savage Mauling | `C,CL,L4,SM` |
| `Zug` | Crushing Blow | `MB,L4,CR` |

¹ Incorporeal is not modelled as a calculator skill (the skill has no dice-roll mechanic to calculate). Gretchen is treated as `D,L4`.

All 57 remaining star players with no special calculator skill default to their listed skill abbreviations (Block, Dodge, Loner, Mighty Blow, etc. as applicable).

---

## Key implementation notes

### Frenzy (PushesChanged in Index.razor.cs)
A Frenzy follow-up block is inserted as a second `PlayerAction` immediately after the main block with `RequiresNonCriticalFailure = true`. When removing Frenzy, `IsFrenzyFollowUp` checks `NumberOfNonCriticalFailures > 0` on the *previous* block — always check this **before** setting `NumberOfNonCriticalFailures = 0`, or the check will fail.

### Savage Blow (BlockStrategy / FractionalDiceBlockStrategy)
Anqi's once-per-game block reroll. Uses `usedSkills | CalculatorSkills.SavageBlow` so it can never fire twice. For fractional dice (`-2D` / `-3D`), the number of failures determines which helper (`SavageBlowOneFailure`, `SavageBlowTwoFailures`, `SavageBlowThreeFailures`) to call.

### A Sneaky Pair (FoulStrategy / StabStrategy)
Dribl & Drull: +1 to foul armour roll and stab roll. Applied as `roll - 1` on the base roll before DirtyPlayer stacks on top, so both bonuses are additive on the target number.

### OldPro vs ConsummateProfessional
Both share `Pro` as their underlying skill in `Player.CanUseSkill`. Only one can fire per calculation path. In Season 3, CP is handled as a modifier (not via ProHelper), and ProHelper will not treat CP as a Pro reroll.

### Unstoppable Momentum (BlockStrategy)
H'thark's per-block free reroll of one die. Implemented in `BlockStrategy` as `UnstoppableMomentum()`, which rerolls the lowest-value die across all 6 possible outcomes. Placed after Hatred and before Pro in `ProcessRoll`. Uses `CalculatorSkills.None` in the `usedSkills` argument (like Brawler and Hatred) so it fires on every block action. Does not chain into a team reroll or Pro after failing.

### SneakyGit and doubles
`FoulStrategy` skips the "sent off on a double" roll entirely when SneakyGit is active — `var rollDouble = canUseSneakyGit ? 0 : _d6.RollDouble(roll, 12)`.

### Season enum vs UI strings
`Calculation.Season` is a `Season` enum (`Season2` / `Season3`). The Blazor UI stores and passes season as a `string` ("Season 2" / "Season 3") via `[CascadingParameter]` and local storage. Convert at the boundary using `ParseSeason(string)` in `Index.razor.cs`. Never store the string in `Calculation`.
