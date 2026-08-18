# Ticket #2: Tilemap Renderer + Camera


**Goal:** Load and render a simple tilemap, with a camera that follows a target point. No real art yet — use flat-colored placeholder tiles so the system is provable now and re-skinnable later.

This ticket follows TDD for the logic pieces: write the test first (it should fail/not compile), then write the implementation until it passes. The actual `Raylib.Draw*` rendering calls are manually verified, not unit tested (see Testing Strategy in `game-architecture.md`).

**Tasks:**

1. **Tilemap data structure (TDD):**
   - Write tests first in `NewGamePlus.Tests` for a `Tilemap` class: e.g. `GetTile(x, y)` returns the correct tile ID for known coordinates; out-of-bounds access behaves sensibly (your choice: throw, or return a defined "void" tile ID — pick one and test it).
   - Then implement `Tilemap` (under a new `/World` folder) to make those tests pass — a 2D grid of tile IDs, e.g. `int[,]`. Hardcode a test map roughly 20x15 tiles directly in code (no file loading yet — that's a later milestone).
2. Define a small placeholder tile palette by ID, e.g.:
   - `0` = grass (green rectangle)
   - `1` = wall (dark gray rectangle)
   - `2` = water (blue rectangle)
   Pick a tile size constant (e.g. 32x32 px) as a shared constant other systems will reuse.
3. Render the tilemap each frame using `Raylib.DrawRectangle` per tile based on its ID — no textures needed yet. Put this in a new `TilemapRenderer` class under `/World`. (Rendering itself: manually verified, not unit tested.)
4. **Camera targeting logic (TDD):**
   - Write tests first for whatever math determines camera behavior — e.g. if you add any clamping (camera shouldn't show past map edges) or offset calculation, test that logic in isolation as a plain function/class before wiring it to raylib's `Camera2D`. If the camera for this ticket is truly just "target = a Vector2, no clamping yet," there may not be much to test yet — that's fine, note it and move on; don't force tests where there's no real logic yet.
   - Implement a basic `Camera2D` (raylib's built-in struct) with a settable `Target` (no player entity yet, that's M3).
   - Pass it into `Raylib.BeginMode2D(camera)` / `EndMode2D()` around the tilemap draw call so panning actually works.
   - Confirm that moving the target (temporary test-key input, throwaway code is fine) visibly pans the camera over the map — manually verified.
5. Wire this into `GameLoop.Draw()` so the tilemap renders inside the existing loop.
6. Confirm `dotnet test` still passes (new tests included) and `dotnet run` still shows the game window with the tilemap rendering.

**Out of scope for this ticket:** player entity, real movement/input system, collision, loading maps from file/Tiled, real art/textures.

**Definition of done:** `dotnet test` passes including new tilemap tests. Running the project shows a colored grid of tiles larger than the screen, and the camera can be confirmed to pan across it (temporary test-key movement is fine to prove this, doesn't need to be production input code).

---

**STATUS: DONE.** `Tilemap` (20x15 grid, 48px tiles → 960x720, larger than the 800x600 screen) built TDD-first, 8 new tests passing (10 total). `TilemapRenderer` draws placeholder colored rectangles; `GameLoop` wires a raylib `Camera2D` via `BeginMode2D`/`EndMode2D` with throwaway arrow-key panning. Manually verified via screenshot capture (target at origin, then panned) that tile colors/positions and camera panning both work correctly. No camera clamping/offset logic existed to unit test this round, per the ticket's own carveout. Branch: `feature/tilemap-camera`.
