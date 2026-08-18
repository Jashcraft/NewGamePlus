# Ticket #5 (for Code Claude): NPC Interaction & Real Dialogue

**Branch suggestion:** `feature/npc-dialogue`

**Goal:** Replace the Ticket #4 throwaway "press E anywhere to open a placeholder box" with the real thing: an NPC standing in the world that the player can walk up to, face, and interact with to trigger dialogue — and `DialogueState` shows actual line content instead of static placeholder text, advancing line-by-line on Enter.

This ticket follows TDD for the interaction-detection and dialogue-advancement logic — write the test first, then implement. Rendering (NPC placeholder shape, text box) is manually verified, not unit tested.

**Tasks:**

1. **NPC entity:**
   - Create an `Npc` class (`/Entities`, alongside `Player`) with at minimum: `Position` (Vector2, same world-space pixel units as `Player`), and a list of dialogue lines (`string[]` or `List<string>` is fine for now — a richer dialogue data format is a later milestone per the roadmap's "content pipeline" step).
   - No sprite yet — render as a simple colored rectangle/circle distinct from the player's color (placeholder, consistent with prior tickets).
   - Place one NPC somewhere reachable on the existing test map in `OverworldState` (open grass, not inside a wall).

2. **Interaction-range detection (TDD):**
   - Write tests first for a function/class that determines "is the player facing and close enough to this NPC to interact" — given player position, player facing direction (`Player.Facing`, already tracked since Ticket #3), and NPC position, does it return true only when the player is both within some interaction distance AND facing toward the NPC (not just standing next to it facing the wrong way)? Cover: too far away (false even if facing correctly), close but facing away (false), close and facing correctly (true).
   - Implement that as its own testable unit (e.g. `InteractionChecker.CanInteract(playerPos, playerFacing, npcPos)`) rather than inlining the math into `OverworldState`.
   - Decide and document the interaction distance/range value (e.g. "must be within 1 tile plus a small buffer") — pick something that feels right for the tile size (48px) rather than an arbitrary number, and note the reasoning briefly in `game-architecture.md`.

3. **Replace the throwaway trigger:**
   - In `OverworldState`, replace the "press E anywhere pushes DialogueState" logic from Ticket #4 with: pressing E only triggers `DialogueState` if `InteractionChecker.CanInteract(...)` is true for the player and at least one NPC in the scene. If not facing/near an NPC, E does nothing.
   - Pass the specific NPC's dialogue lines into the `DialogueState` that gets pushed, rather than static placeholder text.

4. **Dialogue line advancement (TDD):**
   - Write tests first for the line-advancement logic: given a list of lines and a "current index," does "advance" correctly move to the next line, and correctly signal "no more lines, should close" when advancing past the last one? (Plain logic, testable without raylib.)
   - Implement `DialogueState` to use that logic: show the current line, pressing Enter (via the existing `InputSnapshot`/`WasPressed` pattern from Ticket #4b) advances to the next line if one exists, or pops the state off the stack if that was the last line.

5. Confirm `dotnet test` passes (including new interaction and dialogue-advancement tests) and `dotnet run` shows: an NPC visible on the map, walking up and facing it lets E open dialogue, dialogue shows real line content and advances through all lines on repeated Enter presses, closing automatically after the last line (or on a final explicit Enter — your call, just be consistent and document it).

**Out of scope for this ticket:** multiple NPCs with different behavior, branching dialogue based on faction standing (that's later, per the architecture doc's faction system notes), NPC sprites/animation, NPC movement/AI.

**Definition of done:** `dotnet test` passes including new tests. Running the project shows a real NPC that only responds to interaction when the player is near and facing it, with real multi-line dialogue that advances and closes correctly.

---

**STATUS: DONE.** `InteractionChecker.CanInteract` (60px range, cardinal-facing match) and `DialogueProgress` (line index + Advance() signaling close) both TDD'd first, 13 new tests (42 total) passing. `Npc` placed on the test map; `OverworldState` only pushes `DialogueState` when a facing/range check passes, handing it the NPC's real lines. Enter on the last line closes immediately rather than needing an extra press. Both design decisions (range/facing math, close-on-last-line behavior) recorded in `game-architecture.md`.

Manually verified with a tick-gated debug harness (not real-frame-number timing, which proved unreliable since the first fixed tick can fire earlier than a real-frame count would predict): wrong-facing E correctly finds no target, corrected facing opens dialogue on the real first line, Enter advances to the exact second line, a final Enter closes it - confirmed via console log and screenshots. Debug code fully reverted before commit. Branch: `feature/npc-dialogue`.
