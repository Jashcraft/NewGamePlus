# Untitled Critter Game — Engine Architecture Doc

_Living document. Update as decisions change. This is the source of truth Jackal + Claude use to write tickets for Code Claude._

## Vision (one paragraph)

Top-down 2D exploration in the style of Zelda, with Pokémon-style instanced/turn-based battles, but mature in tone: creatures range from cute forest critters to eldritch/demonic and humanoid entities. The player is a stat-bearing combatant in their own right (level, skills, equipment) and can be attacked directly, not just through their critter team. A faction-based reputation/morality system lets players play "good" (legal, earns respect/liking) or "bad" (e.g. taking over towns for resources — earns respect/fear but not liking). Full spectrum of human behavior is in scope; hard excludes: child abuse, bestiality.

## Stack

- **Language:** C# / .NET
- **Graphics/windowing/input:** Raylib-cs (NuGet package, wraps native raylib C library)
- **Platform:** Linux Mint (dev + target), cross-platform by nature of raylib
- **Data authoring:** JSON for game data (critter defs, maps, dialogue) — Tiled map editor likely for tilemaps once we get there

## Core Systems

1. **Game loop / state stack** — fixed-timestep update, states pushed/popped (Overworld, Battle, Dialogue, Menu). Only top state(s) update/render as appropriate.
2. **Tilemap renderer + camera** — load tileset, draw map, camera follows player, basic culling.
3. **Entity/movement/collision** — grid or pixel-based movement (TBD), tile-based collision for overworld.
4. **Combatant abstraction** — the key architectural decision from this session. A single turn-based Battle system serves three cases:
   - Player's critter team vs. wild/enemy critters
   - Player (direct) vs. hostile human/NPC
   - Mixed (future: player + critters together vs. enemy)
   All combatants (critter or human) implement the same interface: stats, moves/actions, turn order, HP/status. This means ONE battle engine, not two.
5. **Player stats/progression** — separate data model from critters: level, skill tree, equipment slots, derived combat stats (from base stats + gear + skills).
6. **Critter data model** — species def (base stats, moveset, catch rate, visual tier from "cute" to "eldritch"), individual instance data (level, current HP, IVs/EVs-equivalent if we want that depth — TBD later).
7. **Faction/reputation system** — NOT a single morality slider. Per-faction standing values. Standing affects dialogue, prices, aggro, quest availability, and town state.
8. **Town state machine** — each town tracks ownership/control state (independent / player-controlled / rival-controlled) and this affects what's rendered and available there. Not needed for vertical slice (1 town), but data model should anticipate it.
9. **Dialogue/NPC interaction** — facing-direction interact trigger, dialogue box, branching based on faction standing (basic branching fine for now).

## Data Model Sketch (early, will evolve)

```
Combatant (interface/base)
 - Name, MaxHP, CurrentHP, Stats (Atk/Def/Spd/etc — TBD final list)
 - Actions available (moves, or human "attacks/items/skills")
 - StatusEffects[]

CritterInstance : Combatant
 - SpeciesId -> CritterSpecies (def data)
 - Level, Moveset[]

PlayerCombatant : Combatant
 - Level, SkillTree state, EquippedItems[]
 - (used when player fights directly, no team on hand)

CritterSpecies (JSON def)
 - Id, Name, BaseStats, MoveLearnset, CatchRate, VisualTier (cute|feral|eldritch|humanoid)

FactionStanding
 - FactionId -> int standing (per faction, not global)

Town
 - Id, ControlState (Independent|Player|Rival), NPCs[], Shops[]
```

This will get revised — treat as a starting skeleton, not gospel.

## Milestone Roadmap (vertical slice target)

**Goal: 1 town, a few critters, prove the core loop (explore → encounter → battle → outcome) works end to end.**

1. **M1 — Project scaffold**: window opens, fixed-timestep game loop, empty state stack, runs on Linux Mint via `dotnet run`.
2. **M2 — Tilemap + camera**: load a small test map (hand-authored JSON or Tiled), render it, camera follows a point.
3. **M3 — Player movement + collision**: player entity moves on the map, tile-based collision blocks walls.
4. **M4 — State stack for real**: Overworld/Battle/Dialogue states, clean push/pop transitions (even with placeholder battle/dialogue screens).
5. **M5 — Combatant abstraction + battle skeleton**: turn-based battle loop works with two placeholder combatants (doesn't matter if critter or human yet — prove the turn/action/HP loop).
6. **M6 — Critter data + wild encounter**: a few real critter species defined in JSON, random encounter triggers battle with a real critter combatant.
7. **M7 — NPC interaction + dialogue**: talk to an NPC, basic dialogue box.
8. **M8 — Player-direct combat**: hostile NPC attacks player with no team — same battle system, PlayerCombatant used directly.
9. **M9 — Catching mechanic**: basic catch flow in battle.
10. **M10 — Vertical slice polish**: tie it together in the 1 town — wander, get into a wild battle, catch something, talk to an NPC, get jumped by a hostile human. Loop proven.

Faction/reputation and town-takeover are architected for (see data model) but NOT required to prove the vertical slice — they slot in after M10 once the core loop is fun.

---

## Ticket #0: Prerequisite Tools (Linux Mint)

**Goal:** Get the machine ready before Code Claude touches anything. Run these yourself (or hand this ticket to Code Claude to execute on your machine).

**1. Install .NET SDK 8.0 (LTS)**

Preferred path — Microsoft's official apt repo (more reliably up to date than Mint's default repos). Confirmed for **Linux Mint 22.3 (Zena)**, which tracks **Ubuntu 24.04 (Noble)**:
```bash
wget https://packages.microsoft.com/config/ubuntu/24.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb
sudo apt update
sudo apt install -y dotnet-sdk-8.0
```

Avoid `sudo snap install dotnet-sdk` — it's had sandboxing issues with native library loading in the past, which matters for raylib specifically.

Verify:
```bash
dotnet --version
```
Should print something like `8.0.x`.

**2. Install VS Code C# tooling**

In VS Code:
- Open Extensions (Ctrl+Shift+X)
- Install **C# Dev Kit** (by Microsoft) — this pulls in the base C# extension automatically, gives you IntelliSense, debugging, build tasks, test explorer.

**3. Build essentials (safety net, likely already present)**
```bash
sudo apt install -y build-essential
```
Covers any native compilation raylib's package might need at install time. Probably a no-op on your machine given your existing dev setup.

**4. Confirm git is present**
```bash
git --version
```
Almost certainly already installed; if not: `sudo apt install -y git`.

**5. Sanity check (do this AFTER Ticket #1 creates the project)**
Once Code Claude scaffolds the project in Ticket #1, confirm the native raylib binary resolves correctly by actually running the project (`dotnet run`). If it throws a `DllNotFoundException` or similar for raylib, that's the signal we need the fallback: `sudo apt install libraylib-dev` or manual native lib placement. Don't pre-install this speculatively — the NuGet package usually bundles what's needed for Linux x64.

**Definition of done:** `dotnet --version` prints 8.x, C# Dev Kit is installed in VS Code, `git --version` works.

---

## Ticket #1 (for Code Claude): Project Scaffold

**Goal:** A running Raylib-cs window with a fixed-timestep game loop, ready for us to build on.

**Tasks:**
1. Create a new .NET console project (`dotnet new console`), add `Raylib-cs` via NuGet.
2. Open an 800x600 (or similar, make it a constant) window titled with the game's working title.
3. Implement a basic game loop with a **fixed timestep update** (e.g. 60Hz update) decoupled from render rate — this matters for later deterministic movement/collision. A simple accumulator pattern is fine.
4. Clear screen each frame to a placeholder color; confirm the window opens, runs, and closes cleanly (Esc or window close button).
5. Set up project structure with room to grow: e.g. `/Core` (game loop, state stack placeholder), `/States` (empty for now), `/Content` (empty, for future assets).
6. Add a `.gitignore` appropriate for .NET, and confirm `dotnet build` + `dotnet run` both work cleanly on Linux Mint.
7. Commit to a new git repo (or tell me if you want it initialized separately) with a clear initial commit message.

**Out of scope for this ticket:** tilemaps, player entity, state stack logic, any actual game content. Just the scaffold + loop.

**Definition of done:** `dotnet run` opens a window, the loop runs at a stable fixed timestep, closing the window exits cleanly, project structure is in place for M2.

**STATUS: DONE.** Confirmed working on Jackal's machine (Mint 22.3) — window opens, loop runs, clean structure, .gitignore in place. Repo: github.com/Jashcraft/NewGamePlus.

---

## Testing Strategy (TDD where it applies)

Not everything in a raylib-based game is unit-testable — actual screen rendering isn't something an automated test can verify meaningfully. The split:

- **Unit-testable (write tests first, TDD style):** tilemap data/logic, camera math (e.g. "does the camera target move correctly given input"), collision detection, combatant/battle rules, stat/damage calculations, faction standing logic, save data serialization. Basically anything that's pure logic, not a draw call.
- **Not practically unit-testable:** actual `Raylib.Draw*` calls, window behavior. These get manually verified (run it, look at it) as we've been doing, not automated.

Test project: `NewGamePlus.Tests`, using **xUnit** (standard, well-supported .NET test framework). Lives alongside the main project, referenced separately, never shipped with the game.

Going forward, tickets that touch testable logic will include explicit test tasks — write the test (it should fail), then implement until it passes, per ticket.

---

## Ticket #1.5 (for Code Claude): Test Project Setup

**Goal:** Get an xUnit test project wired up so every ticket from here on can include real tests.

**Tasks:**
1. Add a new test project: `dotnet new xunit -o NewGamePlus.Tests`.
2. Add a project reference from `NewGamePlus.Tests` to the main `NewGamePlus` project so tests can reference game code.
3. If needed, add the pair to a solution file (`dotnet new sln`, `dotnet sln add`) so `dotnet test` at the repo root runs everything.
4. Write one trivial placeholder test (e.g. `Assert.True(true)` or similar) just to confirm the harness runs.
5. Confirm `dotnet test` runs cleanly from the repo root and the placeholder test passes.
6. Update `.gitignore` if needed to exclude the test project's `bin/`/`obj/` too (likely already covered by the existing .gitignore's patterns, but confirm).

**Definition of done:** `dotnet test` from repo root runs and passes, test project correctly references main project.

**STATUS: DONE.** Confirmed passing on Jackal's machine. xUnit test project + solution file wired up, cross-project reference verified via a real test against `GameLoop` constants.

---

## Ticket Status

| Ticket | Description | Status |
|---|---|---|
| #0 | Prerequisite tools | DONE |
| #1 | Project scaffold | DONE |
| #1.5 | xUnit test project setup | DONE |
| #2 | Tilemap renderer + camera | DONE (see `Tickets/ticket-02-tilemap-camera.md`) |
| #3 | Player entity, movement & collision | See `ticket-03-player-movement.md` |

_Tickets #0, #1, and #1.5 are kept above since they're already complete/short. Starting with #2, each ticket lives in its own file (`ticket-NN-short-name.md`) to keep this doc from ballooning. Update this table as tickets are picked up/completed._
