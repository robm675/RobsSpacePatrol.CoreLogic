# Single-PR CI/CD hardening changeset

This archive contains repository-relative files. Copy/overlay into CoreLogic repo while on a NEW feature branch created from up-to-date `main`. Do not delete the rest of your repository.

Updated:
- `.github/workflows/CoreLogicWorkflow.yml`
- `Scripts/CI/Merge-Release.sh`
- `Scripts/CI/Show-ReleasePreview.ps1`

Added:
- `Scripts/CI/New-ReleaseManifest.ps1`
- `RELEASE-RECOVERY.md`

Highlights:
- Publishing from workflow_dispatch is only allowed on `main` and when merging is not requested.
- A per-run immutable manifest is uploaded in the trusted CoreLogic Actions preparation run, and the approved merge job downloads and validates it against each PR head SHA.
- Post-merge CI checks require the selected consumer workflow, on the exact post-merge main SHA, to succeed.
- Removing unused `package_changed` bookkeeping and adding explicit error checking in release preview reporting.
- Partial-release recovery guidance.

NOT included: pinning the SqlServer PowerShell module to a guessed version. The existing script remains unchanged. Pin a tested version later if desired.

Before first real release:
1. Verify both consumer repositories have ONE active workflow each, OR configure CoreLogic repository variables `CSHARP_CI_WORKFLOW` and `API_CI_WORKFLOW` to their respective required push workflow path (for instance `.github/workflows/build.yml`).
2. Confirm `CROSS_REPO_WRITE_TOKEN` can read Actions status in consumers, and CoreLogic GitHub Actions token has `actions: read`. Verify approval review protection on `release-approval`.
3. Test the feature-branch push; merge with a PR and test main; then exercise publishing and approved merging on a REAL reviewed prepared release at the next planned release. This is NOT an end-to-end runtime-tested patch.
