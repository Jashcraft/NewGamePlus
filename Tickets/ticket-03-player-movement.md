# Ticket #3: Player Entity, Movement & Collision



**Goal:** A player entity that moves around the tilemap with real input (replacing the debug arrow-key camera pan from Ticket #2), blocked by walls via tile-based collision. The camera now follows the player instead of being panned directly.

This ticket follows TDD for the logic pieces (collision checks, movement resolution) — write the test first, then implement. Actual rendering of the player sprite/rectangle and live input polling are manually verified, not unit tested (see Testing Strategy in `game-architecture.md`).

**Tasks:**

1. **Player entity:**
   - Create a `Player` class (new `/Entities` folder, or fold into `/World` if you think that fits better — your call, just be consistent) holding at minimum: `Position` (Vector2, in pixels or tile units — pick one and be consistent, document which), and whatever's needed to track current facing direction (will matter for Ticket #5's NPC interaction, so include it now even though it's unused this ticket).
   - No sprite/texture yet — render as a simple colored rectangle or circle at its position (placeholder, consistent with the "no art yet" approach from Ticket #2).

2. **Movement (TDD):**
   - Decide on movement style: **grid-locked** (player moves exactly one tile at a time, e.g. classic Pokémon-style step movement) vs. **free pixel movement** (smooth, Zelda-style). Given the stated Zelda-exploration + Pokémon-battle blend, free pixel movement is likely the better fit for overworld feel, but this is a real decision — pick one, note it in `game-architecture.md`'s data model section afterward, and don't silently default without flagging it back to me if you're unsure.
   - Write tests first for the movement resolution logic in isolation (i.e., "given current position + input direction + collision result, what's the new position") as a plain function/class, not tangled into raylib input polling directly. This is what makes it testable without a window.
   - Implement input handling (`Raylib.IsKeyDown` for arrow keys and/or WASD — support both) that feeds into that tested movement logic each `Update`.

3. **Tile-based collision (TDD):**
   - Write tests first: given a `Tilemap` and a proposed move, does collision correctly block movement into `Wall` tiles while allowing movement into `Grass`/`Water` (or however you want water handled — walkable or not is your call, note it either way)? Cover edge cases: moving along a wall, corner cases, moving toward the map boundary (`Void` tiles from Ticket #2 should also block movement, not crash).
   - Implement the collision check as its own testable unit, then wire it into the movement resolution so blocked moves simply don't happen (player stops at the wall, doesn't clip through or crash).

4. **Camera follows player:**
   - Remove the Ticket #2 debug arrow-key camera pan from `GameLoop.cs` entirely.
   - Camera `Target` now tracks the player's position every frame instead.
   - Manually verify: walking the player toward a wall should visibly stop them; walking around the test map should keep the camera centered on the player with no jitter.

5. Confirm `dotnet test` passes (including new movement/collision tests) and `dotnet run` shows a controllable player that collides correctly with the test map's border walls and can walk into/around the water patch.

**Out of scope for this ticket:** animated sprites, diagonal-movement edge polish beyond "doesn't crash or clip," NPCs, any state stack work (that's M4), any battle/dialogue systems.

**Definition of done:** `dotnet test` passes including new tests. Running the project shows a player that moves via arrow keys/WASD, is blocked by wall tiles (including the map border), and the camera follows the player smoothly.

---

**STATUS: DONE.** `Player` (Entities/Player.cs) built with free pixel movement (design decision confirmed with Jackal, recorded in `game-architecture.md`); water tiles decided to block movement, same as walls, also recorded there. `TileCollision` and `MovementResolver` TDD'd first (10 new tests, 20 total passing), with axis-separated resolution so diagonal movement slides along walls instead of sticking. Ticket #2's debug camera pan removed entirely; camera now tracks `Player.Position` every frame. Manually verified via screenshot capture (driving the same production movement code with synthetic input, since this dev environment can't simulate real keypresses): rendering, camera-follow, open-grass movement, water-edge collision, and border-wall collision all confirmed working. Branch: `feature/player-movement`.
