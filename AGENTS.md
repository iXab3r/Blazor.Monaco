# Repository guidance

Start in the owning project under `Sources/`. See [README.md](README.md) for build/test entry points, [core architecture](Sources/BlazorMonacoEditor/README.md) for browser/.NET ownership and model identity, and [Roslyn architecture](Sources/BlazorMonacoEditor.Roslyn/README.md) for completion routing.

Authored browser code lives in `Sources/BlazorMonacoEditor/wwwroot/ts/`; `wwwroot/js/` is webpack output. Preserve model URI isolation, text-change coordinate/provenance semantics, provider disposal, and editor option ordering when changing interop. Use the existing focused regression tests and a browser/host check appropriate to the behavior changed. Do not infer UI correctness from a .NET build alone.

Follow the existing project/framework and package settings. For documentation-only changes, check links, factual consistency, and the diff; report checks actually run and any unverified runtime behavior. `npm test` is a placeholder failure, not a validation gate.

## Independent architecture and review

Start each delegated architect, advisor, and reviewer with empty conversation context (`fork_turns="none"` with `spawn_agent`, or the equivalent fresh-context mode). Give only the concrete question, repository path and relevant revisions, scope, constraints, and evidence locations. Keep the assignment focused on the actual topic; omit the author's reasoning, preferred verdict, and earlier agents' conclusions.

Each agent must inspect the relevant implementation and contracts itself and report its own evidence, conclusions, and uncertainty. The final reviewer must be separate from the implementer and design advisor and inspect the final diff at the named revision. Disclose unavailable independent review instead of presenting self-review as independent.
