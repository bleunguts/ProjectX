# ProjectX Developer Agent - conventions

Subagent definition: `.claude/agents/projectx-dev.md`

- Repo: github.com/bleunguts/ProjectX, base branch `main`
- Loop: issue -> branch -> failing test -> implement -> green build/tests -> PR (human merges)
- Branches: `feature/<issue-number>-short-name`; commits: `#<issue>: summary`
- Tests: NUnit, `dotnet test`
- Never push to `main`, never merge own PR, never commit secrets or `cache.json`
- Out-of-scope findings become new GitHub issues

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
- `claude-mcp-flaky-test-agent` - FlakyDetective: MCP server + hand-coded agent loop that finds flaky tests (`run_test_n_times`, `git_branch_commit_pr`). Use it to tell flaky tests from real regressions after a framework/package upgrade. Its CLAUDE.md is the model for issue -> branch -> PR flow.
- `claude-mcp-mispricing-watch-agent` - MispricingWatch: agent that decides if a published price is real or a pricing defect. Relevant to pricing/curve work in ProjectX.
- `claude-ai-sdlc-cva-dashboard` - CVA dashboard; its issue convention is one issue per module with a "Done when".
- `claude-work-secretary-agent` - personal opportunity-triage agent (unrelated to ProjectX code).
- `claude` - scratch repo of Claude/SDK experiments.
Their rule to copy: GitHub issue -> branch -> implementation -> PR, never commit to `main`, each PR updates its roadmap/status.

## Workflow (every task)
1. Issue (rich, per the bar above) -> 2. branch -> 3. baseline build/test -> 4. failing test first -> 5. implement -> 6. build + test green -> 7. PR with `Closes #N`, test evidence, risks -> 8. stop for human review.
Out-of-scope discoveries: new issue, linked from the PR.

First task: GitHub issue #34 (Upgrade solution to .NET 10). Read it with `gh issue view 34`; GitHub is the source of truth, no local issue drafts.
