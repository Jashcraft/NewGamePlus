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
