# How to apply the changes and open a PR

Files produced in this clone:

- `add-windows-ci.bundle` (git bundle of branch `add-windows-ci`) — preferred
- `add-windows-ci.patch` (fallback patch) — if bundle not available

Two options to get these changes into your fork and open a PR.

Option A — preferred: use the git bundle (preserves commits)

1. Download `add-windows-ci.bundle` from this workspace to your machine (via VS Code, scp, or other transfer).
2. In your local fork clone of `TallyConnector`:

```bash
# fetch the branch from the bundle into a local branch
git fetch /path/to/add-windows-ci.bundle add-windows-ci:refs/heads/add-windows-ci
git checkout add-windows-ci

# push to your fork (create a fork if you haven't already)
git push origin add-windows-ci

# create PR (GitHub CLI) or open PR via web UI
gh pr create --title "chore(ci): add Windows CI + CodeQL" \
  --body "Adds Windows CI and CodeQL; see ACCOUNTING_COMPANION_RESEARCH.md" \
  --base main --head add-windows-ci
```

Notes:
- If your default branch is `master`, use `--base master` instead of `--base main`.
- If you cannot fetch from a local path, copy the bundle into the repository root and use the local path: `git fetch ./add-windows-ci.bundle add-windows-ci:refs/heads/add-windows-ci`.

Option B — fallback: apply the patch file (loses original commit metadata)

1. Copy `add-windows-ci.patch` into your fork clone.
2. In your fork clone:

```bash
git apply add-windows-ci.patch
git add .
git commit -m "chore(ci): add Windows CI + CodeQL; docs: add research report"
git push origin HEAD:add-windows-ci
gh pr create --title "chore(ci): add Windows CI + CodeQL" --body "Adds Windows CI and CodeQL; see ACCOUNTING_COMPANION_RESEARCH.md" --base main --head add-windows-ci
```

If you need me to create a fork and open the PR from it, I need GitHub credentials or you must run the push/PR commands from your environment; I can prepare the bundle/patch and instructions (done).

Location of artifacts in this workspace:

```
/tmp/accounting_companion_research/TallyConnector/add-windows-ci.bundle
/tmp/accounting_companion_research/TallyConnector/add-windows-ci.patch
/tmp/accounting_companion_research/TallyConnector/PATCH_INSTRUCTIONS.md
```
