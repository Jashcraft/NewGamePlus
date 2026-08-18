# Ticket #4: State Stack (Overworld / Battle / Dialogue / Menu)

**Goal:** Replace the current hardcoded "GameLoop directly owns the tilemap + player" setup with a real push/pop state stack, so Overworld, Battle, Dialogue, and Menu can each be their own self-contained state that gets pushed on top of / popped off the stack. This is the seam that every future system (battles, dialogue, menus) will plug into, so getting the transition behavior right matters more than any individual state's content right now.

This ticket follows TDD for the stack mechanics themselves (push/pop/peek/update-routing logic) — write the test first, then implement. Actual rendering per-state is manually verified, not unit tested.

**Tasks:**

1. **Define the state interface (TDD-adjacent — this is mostly a contract, but write at least one test confirming a fake/mock state receives calls correctly):**
   - Create an `IGameState` interface (or abstract class, your call) with at minimum: `Update(float dt)`, `Draw()`, and lifecycle hooks for `OnEnter()` / `OnExit()` (called when pushed onto / popped off the stack — useful later for e.g. "pause music" or "reset battle state").
   - Decide and document: should states below the top of the stack still `Update`? (e.g. should the Overworld keep animating in the background while a Dialogue box is open on top of it, or should it fully pause?) This affects the Dialogue/Menu feel — pick a sensible default (my instinct: paused states shouldn't `Update`, only `Draw`, so a paused Overworld renders as a frozen backdrop behind a dialogue box) but flag it if you think differently, don't silently diverge.

2. **Implement the real `StateStack` (TDD):**
   - Replace the current empty placeholder `Core/StateStack.cs` with a real implementation: `Push(IGameState)`, `Pop()`, `Peek()`/`Current`, and whatever `Update`/`Draw` routing logic follows from the decision in step 1.
   - Write tests first: pushing a state calls its `OnEnter`; popping calls `OnExit`; `Update`/`Draw` route to the correct state(s) per the "does the state below still update" decision; popping an empty stack doesn't crash (define and test the behavior — e.g. no-op, or should the stack never legally go empty because Overworld is always the base state?).

3. **Wrap existing gameplay into an `OverworldState`:**
   - Move the current tilemap + player logic out of `GameLoop` and into a new `OverworldState : IGameState` (new `/States` folder — the empty placeholder folder already exists from Ticket #1).
   - `GameLoop` should now own a `StateStack`, push an `OverworldState` at startup, and just call `stack.Update(dt)` / `stack.Draw()` each frame — `GameLoop` itself should no longer know about `Tilemap`/`Player` directly.

4. **Placeholder `DialogueState` (to prove push/pop actually works end-to-end):**
   - Build a minimal `DialogueState` — doesn't need real dialogue content yet, just something visibly distinct (e.g. a solid-color box with placeholder text like "Dialogue placeholder — press Enter to close" drawn over the frozen Overworld behind it).
   - Wire a temporary test trigger (e.g. press a specific key like `E` while in `OverworldState`) that pushes `DialogueState` onto the stack; pressing Enter in `DialogueState` pops it back off, returning control to `OverworldState`.
   - This is throwaway trigger logic (real NPC-triggered dialogue is Ticket #5) — just needs to prove the stack mechanics work visibly, similar to how Ticket #2's arrow-key pan proved camera panning before real player movement existed.

5. Confirm `dotnet test` passes (including new state-stack tests) and `dotnet run` shows: normal overworld movement, pressing the trigger key opens the dialogue placeholder (overworld visibly frozen behind it, per the step-1 decision), pressing Enter closes it and movement resumes normally.

**Out of scope for this ticket:** real dialogue content/branching, NPC interaction triggers, the Battle state (that's a later milestone — M5/M6 per the roadmap), Menu state content.

**Definition of done:** `dotnet test` passes including new state-stack tests. `GameLoop` no longer directly references `Tilemap`/`Player` — it only drives a `StateStack`. Running the project demonstrates a working push/pop cycle via the temporary dialogue-placeholder trigger.
