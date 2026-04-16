# Accounting-Companion — Deep Research & Competitor Comparison

Date: 2026-04-16

## Executive summary

- The primary public project in the org is `TallyConnector` — a C#/.NET connector library (NuGet v2.2.0, ~21.5K total downloads, ~74 stars). It's the highest-value repo for integration capabilities.
- I added a Windows-focused GitHub Actions workflow (`.github/workflows/windows-ci.yml`) to build/test on `windows-latest` across .NET SDK versions and to run CodeQL analysis (committed locally to branch `add-windows-ci`).
- I attempted local SCA and a Linux build: the clone fails to restore/build on Linux due to Windows-targeting and Central Package Management configuration (NETSDK1100 / NU1008). Build is expected to succeed on `windows-latest` in GH Actions.
- No plaintext secrets were observed in the public Accounting-Companion repos during scanning; local codebases may still require secret scanning across forks/backups.
- Recommendation: enable the workflow in CI (push branch + open PR), enable Dependabot/nuget SCA, add CODEOWNERS/CONTRIBUTING, and establish release/governance to reduce single-maintainer risk.

## Scope & methodology

- Fetched org metadata and top repository READMEs for `TallyConnector`, `TallyXmlsGenerator`, `ERPNextDotNet`, `TallyConnector_Samples`, and `TallyPowerBI.Docs`.
- Performed pattern-based scans for secrets and risky files in those repos.
- Cloned `TallyConnector` and attempted `dotnet --info`, `dotnet restore`, `dotnet build`, and `dotnet list package` to collect SCA output.
- Created a GitHub Actions workflow to run on `windows-latest` and included CodeQL analysis; added a written research report to the repo.

## Repository summaries

### TallyConnector

- Language/stack: C#/.NET (source generators, multi-targeting references to modern SDKs). Uses Central Package Management (Directory.Packages.props).
- Maintenance & adoption: MIT license; moderate adoption (NuGet downloads ~21.5K; ~74 stars). Single primary maintainer — contributor/maintenance risk.
- Build notes: the repo requires Windows-targeting for some projects; on Linux restore/build fails (NETSDK1100). Central Package Management requires `PackageVersion` items for packages; missing versions cause NU1008 during restore on this environment.
- Security posture: no public plaintext secrets discovered in public repo scan. Recommend enabling CodeQL and automated dependency auditing.

### ERPNextDotNet, TallyXmlsGenerator, TallyConnector_Samples, TallyPowerBI.Docs

- These are companion projects: helpers, sample code, generators, and documentation. They complement `TallyConnector` but do not appear to replace it. Many are small, sample-driven, and MIT-licensed.

## Comparative analysis (Competitors & positioning)

- `TallyConnector` (Accounting-Companion): Open source, focused on .NET consumers; provides models, helper libraries, and a NuGet package for integration with Tally. Strengths: .NET-native, NuGet distribution. Risks: single maintainer, limited CI/automation, multi-targeting complexity.
- Alternatives/adjacent projects encountered:
  - `ERPNextDotNet`: alternative open-source connector for ERPNext, useful when ERPNext is the destination. Different target systems and requirements.
  - Community or commercial connectors (not in this org): vendor-provided Tally connectors/ETL tools (commercial offerings) — typically more polished support but proprietary.

Key differentiators for `TallyConnector`:

- .NET-native library and source-generator usage make it attractive for .NET teams.
- NuGet distribution simplifies consumption for .NET projects.
- Lacks visible enterprise governance (CI, release cadence, CONTRIBUTING). This reduces enterprise adoption confidence.

## Technical deep-dive: build & CI

- Symptoms found when building locally (Linux):
  - NETSDK1100: projects target Windows-only assets; set `/p:EnableWindowsTargeting=true` or build on `windows-latest` in Actions.
  - NU1008: central package management is in use but some `PackageReference` entries expect `PackageVersion` items. Directory-level package versions must be provided (Directory.Packages.props) or package references must include explicit `Version` values.

Recommendations:

- Run the provided GitHub Actions workflow on `windows-latest` (already added locally in `.github/workflows/windows-ci.yml`). It tests multiple .NET SDKs and runs CodeQL.
- Add a `Directory.Packages.props` with `PackageVersion` items if central package management is intended, or restore explicit versions.
- Enable `dotnet list package --vulnerable` job in CI and/or Dependabot for automated dependency updates.

## Security, SCA, and quality

- The added workflow includes CodeQL analysis. Additionally, enable:
  - Dependabot `version` and `security` updates for NuGet.
  - `dotnet list package --vulnerable` in CI and block merges on critical/high findings.
  - A SAST step with an org-approved tool (Semgrep, SonarCloud) for extra coverage.

## Governance & maintainability

- Add files to improve contributions and trust:
  - `CONTRIBUTING.md` with how to run tests locally, build matrix, and where to open issues.
  - `CODEOWNERS` to assign reviewers for PRs touching core code.
  - `RELEASE.md` with the release and tagging process for NuGet publishing.

## PR template & push instructions

Proposed PR title: `chore(ci): add Windows CI + CodeQL`

Proposed PR body:

```
This PR adds a GitHub Actions workflow to build and test `TallyConnector` on `windows-latest` across supported .NET SDK versions and runs CodeQL analysis.

Also included: `ACCOUNTING_COMPANION_RESEARCH.md` — a research & comparison report for the Accounting-Companion org.

Notes:
- The project uses Central Package Management; CI runs on Windows to avoid NETSDK1100 errors encountered when building on Linux.
- After merge: enable Dependabot and the repository's Actions to run on pushes/PRs.

Checklist:
- [ ] CI runs green on `windows-latest`.
- [ ] Add Dependabot configuration for NuGet.
- [ ] Add `CONTRIBUTING.md` and `CODEOWNERS`.
```

Push & PR commands (run where you have push rights):

```bash
git checkout -b add-windows-ci
git add .github/workflows/windows-ci.yml ACCOUNTING_COMPANION_RESEARCH.md
git commit -m "chore(ci): add Windows CI + CodeQL; docs: add research report"
git push origin add-windows-ci

# Create PR with GitHub CLI (if configured):
gh pr create --title "chore(ci): add Windows CI + CodeQL" \
  --body "Adds Windows CI and CodeQL; see ACCOUNTING_COMPANION_RESEARCH.md" \
  --base main --head add-windows-ci
```

If pushing is blocked (permission), open a PR from a fork with the same changes.

## Recommended next steps (priority order)

1. Push branch and open PR so Actions can run on `windows-latest` (verifies build & CodeQL).  
2. Enable Dependabot for NuGet and add `dotnet list package --vulnerable` gate.  
3. Add `CONTRIBUTING.md`, `CODEOWNERS`, and a simple release process.  
4. Consider adding a maintenance plan (multiple maintainers, issue triage SLA).  

---

Prepared by: agent research run on 2026-04-16. Files added: `.github/workflows/windows-ci.yml`, `ACCOUNTING_COMPANION_RESEARCH.md` (this file).
