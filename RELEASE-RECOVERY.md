# Coordinated release recovery

## If the approved merge workflow fails

Do **not** immediately rerun the failed dispatch. First inspect which repository has already merged and which PR remains open. Cross-repository merges are not atomic.

1. In the CoreLogic Actions run, record the release branch, both PR numbers, expected commit SHAs, and the last successful step.
2. In CSharp and API, check the release PR states and the current `main` workflow for each repository. Confirm the last successful build/test run belongs to the actual post-merge `main` SHA.
3. If **neither** PR merged, correct the failure, create a fresh coordinated release preparation run on CoreLogic `main`, review the new release PRs, then obtain the required manual approval. Never modify the old prepared PR branches to reuse their manifest.
4. If **CSharp merged but API did not**, stop automated release operations. Diagnose API's PR/checks and decide between (a) a new, fully validated coordinated release for the remaining changes, or (b) a reviewed revert of the CSharp merge. Do not simply rerun the original workflow; its CSharp PR is already merged.
5. If **both merged but a post-merge CI run failed**, treat the release as incomplete. Fix forward or revert using reviewed PRs, and rerun the consumer CI against the resulting `main` commits.
6. When both consumers are green at their latest intended `main` commits, document the resolution and clean up unused release branches.

## Safety controls

- Keep `release-approval` required reviewer protection in place.
- Only dispatch `prepare_release` or `merge_release` from trusted CoreLogic `main`.
- `prepare_release` and `merge_release` must never both be selected.
- The release manifest artifact expires after 90 days. Expired preparations must be recreated, not bypassed.
- The merge job requires the original preparation workflow to have completed successfully.
- For repos with multiple active workflows, define CoreLogic repository variables `CSHARP_CI_WORKFLOW` and `API_CI_WORKFLOW` as the paths of the required push-triggered CI YAML files (for example `.github/workflows/ci.yml`). When only one active workflow exists, the script discovers it automatically.
- The `CROSS_REPO_WRITE_TOKEN` needs permission to read pull requests, repository contents, and Actions workflow status in both consumer repositories. The job's `GITHUB_TOKEN` needs CoreLogic Actions read access to retrieve the immutable manifest.
- Always inspect the PR and its release branch before granting manual approval. An approved workflow can perform real merges.
