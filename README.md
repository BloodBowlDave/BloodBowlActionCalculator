# Blood Bowl Action Calculator

A probability calculator for Blood Bowl (2020 edition) that computes the cumulative success chance for any sequence of actions a player or team might attempt during a turn.

## Features

- **Action sequence builder** — chain any number of actions for one or more players
- **Team re-rolls** — specify how many re-rolls are available; the calculator factors in optimal re-roll usage
- **Branching actions** — model Frenzy follow-up blocks, Dauntless, and other conditional paths
- **Skill-aware** — player skills (Dodge, Sure Hands, Brawler, Pro, Claw, etc.) modify probabilities automatically
- **Season toggle** — switch between Season 2 and Season 3 rules
- **Save & compare** — store named calculations in local browser storage and view them side-by-side
- **Compact notation** — every calculation is encoded as a short string you can share or bookmark

## Tech Stack

| Layer | Technology |
|---|---|
| Runtime | .NET 10 |
| UI | Blazor WebAssembly (Hosted) |
| Component library | MudBlazor 9.3 |
| Local persistence | Blazored.LocalStorage |
| Validation | FluentValidation |
| Tests | xUnit |

## Project Structure

| Project | Role |
|---|---|
| `ActionCalculator` | Core calculation engine — strategies, evaluators, re-roll logic |
| `ActionCalculator.Abstractions` | Interfaces shared across projects |
| `ActionCalculator.Models` | Domain models — `Calculation`, `PlayerAction`, `Block`, enums |
| `ActionCalculator.Utilities` | Small extension helpers (clamping, enum flags, combinatorics) |
| `ActionCalculator.Web` | Blazor WASM hosted app (Client + Server projects) |
| `ActionCalculator.Tests` | xUnit test suite (~200+ parametrised cases) |

## Supported Actions

| Code | Action |
|---|---|
| `O` | Rerollable (generic) |
| `X` | Non-rerollable (generic) |
| `D` | Dodge |
| `R` | Rush |
| `U` | Pick Up |
| `P` | Pass |
| `K` | Block |
| `C` | Catch |
| `F` | Foul |
| `B` | Armour Break |
| `H` | Throw Teammate |
| `L` | Dauntless |
| `I` | Interference |
| `N` | Tentacles |
| `S` | Shadowing |
| `A` | Argue the Call |
| `E` | Bribe |
| `J` | Injury |
| `G` | Landing |
| `M` | Hail Mary Pass |
| `Y` | Hypnogaze |
| `T` | Stab |
| `W` | Chainsaw |

## Skills

Dodge, Break Tackle, Sure Hands, Sure Feet, Pass, Cloud Burster, Catch, Diving Catch, Brawler, Claw, Mighty Blow, Pro, Loner, Dirty Player, Sneaky Git, Diving Tackle, Consummate Professional, Old Pro, The Ballista, Mesmerising Dance, Ram, Brutal Block, Savage Mauling, Crushing Blow, Slayer, Incorporeal, Blind Rage, Blast It, Whirling Dervish, Hatred, Lone Fouler

## Running Locally

```bash
dotnet run --project ActionCalculator.Web/Server/ActionCalculator.Web.Server.csproj
# Browse to http://localhost:5000
```

## Running Tests

```bash
dotnet test ActionCalculator.Tests
```

## Calculation Notation

Calculations are serialised as compact strings, for example:

| Notation | Meaning |
|---|---|
| `3D2` | Block — 3 dice, 2 success faces |
| `-2D2` | Block — half-dice (opponent chooses), 2 success faces |
| `2D2!2{1D2}` | Block — 2 dice, 2 successes, 2 push results trigger a Frenzy follow-up |
| `2D2^` | Block with Brawler |
| `2D2*` | Block with Pro |
| `2D2%` | Block with Hatred |
| `D,R,U,P,C` | Dodge → Rush → Pick Up → Pass → Catch |
| `L2,2D2\|1D2` | Dauntless into Block (success / failure paths) |
| `~S2` | Suffix indicating Season 2 rules |

Actions for a single player are separated by `,`. Multiple players are separated by `;`. Rerollable branches use `{...}` and non-rerollable branches use `[...]`.
