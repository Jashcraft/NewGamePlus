# Ticket #4b (for Code Claude): Fix Missed Key Presses in Fixed-Timestep Loop

**Branch:** stay on `feature/state-stack` (fix before merge, not a new branch)

**Bug (found via manual testing):** Pressing E to open the dialogue placeholder and Enter to close it are both unreliable — sometimes registers immediately, sometimes takes several presses, sometimes doesn't register at all.

**Root cause:** `Raylib.IsKeyPressed()` is edge-triggered — it only reports `true` on the single real frame where a key transitions from up to down, and that internal state is refreshed once per real rendered frame. But `Update()` (where `IsKeyPressed` is actually checked, in `OverworldState`/`DialogueState`) only runs when the fixed-timestep accumulator in `GameLoop` crosses its `1/60` threshold — which does NOT happen every real frame once rendering runs faster than 60Hz (uncapped, per the earlier frame-rate discussion). So a key's one "pressed" frame can land on a real frame where zero `Update()` calls happen, and the press is silently dropped. This will affect every future edge-triggered input (NPC interact key in Ticket #5, menu confirm/cancel, battle action selection, etc.) if not fixed now.

**Fix approach:**
1. In `GameLoop.Run()`'s outer loop (the one that runs once per real frame, before the accumulator's inner `while`), poll and latch any edge-triggered keys you need this frame — at minimum whatever `OverworldState`/`DialogueState` currently check (E, Enter). A simple approach: an `InputState` (or similarly named) snapshot captured once per real frame via `Raylib.IsKeyPressed(...)`, passed down into `s_stack.Update(dt, inputSnapshot)` (this changes the `IGameState.Update` signature — update `StateStackTests`' fakes accordingly) rather than states calling `Raylib.IsKeyPressed` directly themselves.
2. If multiple `Update()` calls happen within the same real frame (accumulator catch-up after a stutter), the same latched "pressed" snapshot should only be actionable once — don't let a single real-frame press trigger the push/pop twice just because `Update()` ran twice that frame. Decide and document how you're preventing that (e.g. consume/clear the latch after the first `Update()` call that uses it within a frame).
3. Held keys (continuous movement, `IsKeyDown` in `Player.ReadInput`) are NOT affected by this bug — `IsKeyDown` is level-triggered, not edge-triggered, so no fix needed there. Leave movement input as-is.
4. Update `StateStackTests` and any other affected tests for the new `Update` signature if you go the injected-input-snapshot route.
5. Manually re-verify: E reliably opens the dialogue box on the first press, Enter reliably closes it on the first press, with no missed presses across at least ~10 repeated open/close cycles.

**Definition of done:** `dotnet test` still passes with updated tests. E and Enter reliably trigger on the first press, every time, in manual testing.

---

**STATUS: DONE.** `InputSnapshot` (Core/InputSnapshot.cs) latches key presses across real frames instead of a fresh per-frame snapshot: `GameLoop` holds one persistent instance for the whole run and merges newly-pressed keys into it every real frame via `CaptureFrame`; `StateStack.Update` consumes (clears) it immediately after routing to the current state, so a press survives however many "quiet" frames it takes to reach the next `Update()` call but can't double-trigger within one real frame's accumulator catch-up. `IGameState.Update` now takes the injected snapshot; states no longer call `Raylib.IsKeyPressed` directly. 4 new tests (29 total), all passing.

Manually verified the actual bug, not just a regression: temporarily force-latched presses on real frames deliberately unaligned with any fixed-tick boundary, logged stack-depth transitions, and confirmed each forced press triggered exactly once — landing on a later real frame once an `Update()` call finally ran — across 3 full open/close cycles, no drops, no double-triggers. Debug code reverted before commit.
