# AGENTS.md

This repository is **RestSharp.RequestBuilder** — a .NET Standard library that provides a fluent wrapper for constructing `RestRequest` objects for RestSharp. :contentReference[oaicite:0]{index=0}  
This file exists to give coding agents the *operational* context needed to make correct, minimal, reviewable changes. :contentReference[oaicite:1]{index=1}

---

## Repository layout

- `src/RestSharp.RequestBuilder/` — main library
- `tests/RestSharp.RequestBuilder.UnitTests/` — unit tests
- `RestSharp.RequestBuilder.sln` — solution root
- `Directory.Build.props`, `Directory.Packages.props` — shared build/package conventions
- `.editorconfig` — style rules (follow it) :contentReference[oaicite:2]{index=2}

---

## Setup commands

> Use the .NET SDK pinned by `global.json` (install that exact SDK if you see SDK-mismatch errors). :contentReference[oaicite:3]{index=3}

- Restore:
  - `dotnet restore ./RestSharp.RequestBuilder.sln`
- Build:
  - `dotnet build ./RestSharp.RequestBuilder.sln -c Release`
- Test:
  - `dotnet test ./RestSharp.RequestBuilder.sln -c Release`
- Pack (when needed):
  - `dotnet pack ./src/RestSharp.RequestBuilder/ -c Release -o ./artifacts`

If a change is isolated to one project, scope commands to that project folder instead of running solution-wide work.

---

## Guardrails

### Compatibility

- **Do not** remove or change existing public APIs (types/methods/overloads) without:
  1) updating/adding tests, and  
  2) updating README examples if behavior changes.
- Keep the library compatible with **.NET Standard** targets already defined in the project (do not “upgrade” TFM(s) unless explicitly requested). :contentReference[oaicite:4]{index=4}

### Fluent builder semantics

- Preserve fluent chaining: methods should return the builder instance when appropriate.
- Prefer predictable behavior over “magic”:
  - If a new method replaces/overrides an existing setting (e.g., headers/auth/body), make replacement behavior explicit and test it.
- Follow documented behaviors already stated in README (e.g., invariant-culture string conversion, duplicate handling, input validation). :contentReference[oaicite:5]{index=5}

### Code style & quality

- Follow `.editorconfig` and existing naming conventions.
- Prefer small, cohesive methods; avoid adding abstractions that don’t pull their weight.
- Add unit tests for:
  - happy path
  - null/empty argument validation
  - edge cases (duplicates, overwrite semantics, conflicting settings)

---

## How to implement changes

### Adding a new fluent method

1. Add method to the primary builder/extension surface used by consumers (match existing patterns in `src/`).
2. Ensure the method:
   - validates inputs (throw `ArgumentNullException`/`ArgumentException` as consistent with existing style)
   - does not introduce side effects beyond the request being built
3. Add/extend unit tests in `tests/` covering the behavior.
4. If user-facing, update README examples.

### Modifying request construction

- Keep `Create()` behavior stable.
- If request parameter/header handling changes, add tests that assert the exact RestSharp parameter/header outcomes.

---

## Security / secrets

- Never introduce real tokens/keys in code, tests, docs, or examples.
- Any samples should use obvious placeholders like `your-token-here`.

---

## PR checklist (agent self-check)

- [ ] Change is minimal and scoped to the request
- [ ] `dotnet build` and `dotnet test` `dotnet format` pass
- [ ] Public API remains backward compatible (or changes are clearly documented + tested)
- [ ] README updated if consumer-visible behavior changed
- [ ] No secrets committed
