# ProjectX Developer Agent - conventions

Follows the shared workflow in `~/.claude/CLAUDE.md` (issue -> `feature/<n>-name` branch -> baseline -> implement -> green -> PR with `Closes #N`, human merges, commit message suggested at session end). Rules below are ProjectX-specific additions.

Subagent definition: `.claude/agents/projectx-dev.md`

- Repo: github.com/bleunguts/ProjectX, base branch `main`
- Tests: NUnit, `dotnet test`
- **Test first (ProjectX only):** write a failing test (or a failing build/test gate for infra changes) that captures the acceptance criteria, show it fails, then implement the smallest change that passes
- Never commit `cache.json`

## Issue quality bar (write the issue like a real ticket)
PR review comments are only as specific as the issue, so the issue must carry the implementation detail. Every issue has:
- **Summary + why now**
- **Baseline**: measured `dotnet build` / `dotnet test` results on `main` before any change, with pre-existing failures named and explained
- **Current state**: inventory with real numbers (projects, TFMs, package versions, file paths), checked against the repo, not guessed
- **Scope** as an ordered checklist, small steps that each leave the repo green
- **Out of scope**, each item with the reason, so reviewers do not ask for it in the PR
- **Risks + mitigation** table
- **Test plan**: the failing test written first, automated checks, manual smoke, CI
- **Acceptance criteria** as checkboxes and a **Definition of done** (branch name, commit format, `Closes #N`)
- **Related**: links to sibling repos/issues
Check `gh issue list --state all` first; update an existing issue instead of creating a duplicate.

## Sibling repos (same owner, same conventions; reuse patterns, link them from issues)
Local checkouts live next to this repo under `C:\Dev\projects\GitHub\`.
- `claude-mcp-flaky-test-agent` - FlakyDetective: MCP server + hand-coded agent loop that finds flaky tests (`run_test_n_times`, `git_branch_commit_pr`). Use it to tell flaky tests from real regressions after a framework/package upgrade.
- `claude-mcp-mispricing-watch-agent` - MispricingWatch: agent that decides if a published price is real or a pricing defect. Relevant to pricing/curve work in ProjectX.
- `claude-ai-sdlc-cva-dashboard` - CVA dashboard; its issue convention is one issue per module with a "Done when".
- `claude-work-secretary-agent` - personal opportunity-triage agent (unrelated to ProjectX code).
- `claude` - scratch repo of Claude/SDK experiments.

## Next task
GitHub issue #34 (Upgrade solution to .NET 10). Read it with `gh issue view 34`; GitHub is the source of truth, no local issue drafts.
