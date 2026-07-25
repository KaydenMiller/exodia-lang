# Exodia.Lang.Test — agent guide

TUnit test harness for the Exodia language. It drives real source through the compiler
pipeline and asserts on the results. This file explains how to use and extend it.

## Running

TUnit runs on Microsoft.Testing.Platform — the test project is an **executable**, so run it,
don't `dotnet test` it (that works too, but `run` is the primary path):

```bash
dotnet run --project Exodia.Lang.Test -c Debug
```

Useful flags (passed after `--`):

- `--treenode-filter "/*/*/FixtureTests/*"` — run one class (or `.../EmitsWellFormedIr*` for one test)
- `--maximum-parallel-tests 1` — run serially (tests are parallel by default)

## The pipeline helper (`Pipeline.cs`)

Mirrors `Exodia.Lang/Program.cs`. Four composable stages — stop at whichever you need:

| Method | Returns | Use for |
|---|---|---|
| `Pipeline.Parser(src)` | `ExodiaParser` (before `.program()`) | syntax-error checks (`parser.NumberOfSyntaxErrors`) |
| `Pipeline.ParseTree(src)` | `ProgramContext` | grammar / parse-tree assertions |
| `Pipeline.LowerAst(src)` | `ProgramNode` | AST-shape assertions |
| `Pipeline.CompileToModule(src)` | `LLVMModuleRef` | verify / inspect IR (`module.TryVerify`, `module.PrintToString`) |
| `Pipeline.Compile(src)` | `string` | the emitted IR as text |

### Invariant: one LLVM context per compile

`CompileToModule` creates a **fresh `LLVMContextRef` per call** on purpose. LLVM's *global*
context is process-wide and de-duplicates named struct types, so compiling many fixtures in
one process there renames the Nth `Circle` to `Circle.4` and breaks codegen's name-keyed
lookups (non-deterministic failures under parallelism). **Do not** use
`LLVMModuleRef.CreateWithName(...)` in tests — always go through `Pipeline`. This is only safe
because the codegen routes every primitive through the module's context (no global-context
`LLVMTypeRef.Int32`-style leaks); if you reintroduce one, a fresh-context compile will
**segfault** on a context mismatch.

## Fixtures (`Fixtures.cs` + `Fixtures/`)

`.ex` source files under `Fixtures/` are copied next to the test binary at build time.

- `Fixtures.Load("codegen/return-literal.ex")` → source text
- `Fixtures.InDirectory("codegen")` → fixture paths (forward-slashed, relative to `Fixtures/`),
  the seam for multi-file / library tests
- `FullPath(rel)` → absolute path

The files are copied from `Exodia.Lang/tests/`. The originals are intentionally still there
(other work depends on them); don't delete either copy without checking first.

## Writing tests (TUnit conventions)

- `[Test]` on an `async Task`; assertions are awaited: `await Assert.That(x).IsEqualTo(y)`
- `[DisplayName("Readable sentence")]` — keep C# method names PascalCase, put the readable
  label here; `$param` interpolates arguments
- Data-driven: `[MethodDataSource(nameof(SomeStaticMethod))]` where the method returns
  `IEnumerable<string>` (or a tuple type) — **one generated test case per item**. The fixture
  sweeps use this to make "add a file → get a test" work with no code change.

## What is (and isn't) asserted today

- `ParsesWithoutSyntaxErrors` — the grammar accepts every fixture. Meaningful.
- `EmitsWellFormedIr` — every `codegen/` fixture lowers + codegens to IR that **passes LLVM's
  verifier** (`TryVerify`). Catches malformed IR, **not** wrong-but-valid IR.
- **No behavioral-correctness tests yet.** A program can emit valid IR and compute the wrong
  answer. The intended next rung is JIT-executing fixtures (MCJIT) and asserting exit codes /
  stdout, with expected values annotated in the fixture (e.g. `// expect-exit: N`).

## Inspecting emitted IR

`EmitsWellFormedIr` writes each module's IR to the test's captured output
(`TestContext.Current!.Output.WriteLine`).

- **Rider** shows it for passing tests.
- **CLI** shows it only on failure (alongside the verifier message).
- **CLI on demand:** set `EXODIA_DUMP_IR=1` to also dump `.ll` files under
  `bin/<cfg>/net10.0/TestResults/ir/` (mirrors the fixture tree). Off by default so normal
  runs write nothing.

```bash
env EXODIA_DUMP_IR=1 dotnet run --project Exodia.Lang.Test -c Debug
# then read e.g. bin/Debug/net10.0/TestResults/ir/codegen/return-literal.ll
```

Or dump IR for a single file in a fresh process via the CLI:
`dotnet run --project Exodia.Lang -- path/to/file.ex`

## Scope boundary

This project owns test infrastructure and fixtures. Changes to the compiler itself
(`Exodia.Lang/`) and the grammar (`Exodia.g4`) are the maintainer's call — surface them as
suggestions, don't edit unprompted.
