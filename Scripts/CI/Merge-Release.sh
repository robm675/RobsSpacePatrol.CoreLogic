#!/usr/bin/env bash
set -euo pipefail

repositories=(
  RobsSpacePatrol.CSharp
  RobsSpacePatrol.API
)

declare -A pr_numbers
declare -A expected_heads
declare -A expected_bases


# Match the immutable artifact produced by the ORIGINAL approved preparation run.
# This closes the gap where a release PR's head is replaced after preparation.
if [[ ! "$RELEASE_BRANCH" =~ ^release-preview-([0-9]+)-([0-9]+)$ ]]; then
  echo "::error::Invalid release branch name; expected release-preview-RUNID-ATTEMPT."
  exit 1
fi
source_run_id="${BASH_REMATCH[1]}"
source_attempt="${BASH_REMATCH[2]}"
if [[ -z "${CORELOGIC_TOKEN:-}" ]]; then
  echo "::error::Missing CoreLogic Actions read token."
  exit 1
fi
run_json=$(GH_TOKEN="$CORELOGIC_TOKEN" gh api "repos/robm675/RobsSpacePatrol.CoreLogic/actions/runs/$source_run_id")
if ! jq -e --argjson attempt "$source_attempt"   '.event == "workflow_dispatch" and .head_branch == "main" and .conclusion == "success" and .run_attempt == $attempt and .path == ".github/workflows/CoreLogicWorkflow.yml"'   <<< "$run_json" >/dev/null; then
  echo "::error::Original release preparation was not a successful approved main workflow run."
  exit 1
fi
manifest_dir=$(mktemp -d)
trap 'rm -rf "$manifest_dir"' EXIT
GH_TOKEN="$CORELOGIC_TOKEN" gh run download "$source_run_id"   --repo robm675/RobsSpacePatrol.CoreLogic   --name coordinated-release-manifest --dir "$manifest_dir"
manifest_file="$manifest_dir/coordinated-release-manifest.json"
if [[ ! -s "$manifest_file" ]]; then
  echo "::error::Release manifest artifact missing."
  exit 1
fi
if ! jq -e --arg branch "$RELEASE_BRANCH" --argjson run_id "$source_run_id"   --argjson attempt "$source_attempt"   '.schema_version == 1 and .release_branch == $branch and .run_id == $run_id and .run_attempt == $attempt and .source_repo == "robm675/RobsSpacePatrol.CoreLogic"'   "$manifest_file" >/dev/null; then
  echo "::error::Release manifest does not match originating preparation run."
  exit 1
fi

# Identify the REQUIRED push workflow in each consumer, not an arbitrary green workflow.
resolve_workflow_id() {
  local full_repo="$1" desired="$2" workflows selected count
  workflows=$(gh api "repos/$full_repo/actions/workflows?per_page=100")
  selected=$(jq --arg desired "$desired" '[.workflows[] | select(.state == "active") | select($desired == "" or .name == $desired or .path == $desired or (.id | tostring) == $desired)]' <<< "$workflows")
  count=$(jq 'length' <<< "$selected")
  if [[ "$count" -ne 1 ]]; then
    echo "::error::$full_repo: Expected exactly one selected active CI workflow, found $count. Set repository variable CSHARP_CI_WORKFLOW or API_CI_WORKFLOW to its workflow file path." >&2
    return 1
  fi
  jq -r '.[0].id' <<< "$selected"
}
declare -A workflow_ids
workflow_ids[RobsSpacePatrol.CSharp]=$(resolve_workflow_id robm675/RobsSpacePatrol.CSharp "${CSHARP_CI_WORKFLOW:-}")
workflow_ids[RobsSpacePatrol.API]=$(resolve_workflow_id robm675/RobsSpacePatrol.API "${API_CI_WORKFLOW:-}")

echo "=== FINAL RELEASE PREFLIGHT ==="

# Verify BOTH repositories before changing either one.
for repo in "${repositories[@]}"; do
  full_repo="robm675/$repo"
  echo "Checking $full_repo"

  prs=$(gh pr list \
    --repo "$full_repo" \
    --state open \
    --base main \
    --head "$RELEASE_BRANCH" \
    --json number)

  count=$(jq 'length' <<< "$prs")
  if [[ "$count" -ne 1 ]]; then
    echo "::error::$repo: Expected one open release PR, found $count."
    exit 1
  fi

  number=$(jq -r '.[0].number' <<< "$prs")

  pr=$(gh pr view "$number" \
    --repo "$full_repo" \
    --json state,isDraft,headRefOid,baseRefOid,mergeable)

  state=$(jq -r '.state' <<< "$pr")
  draft=$(jq -r '.isDraft' <<< "$pr")
  mergeable=$(jq -r '.mergeable' <<< "$pr")
  head=$(jq -r '.headRefOid' <<< "$pr")
  base=$(jq -r '.baseRefOid' <<< "$pr")

  if [[ "$state" != "OPEN" ||
        "$draft" != "false" ||
        "$mergeable" != "MERGEABLE" ]]; then
    echo "::error::$repo PR is not ready to merge."
    exit 1
  fi

  current_main=$(gh api \
    "repos/$full_repo/git/ref/heads/main" \
    --jq '.object.sha')

  if [[ "$current_main" != "$base" ]]; then
    echo "::error::$repo main changed since PR verification."
    exit 1
  fi

  current_head=$(gh api \
    "repos/$full_repo/git/ref/heads/$RELEASE_BRANCH" \
    --jq '.object.sha')

  if [[ "$current_head" != "$head" ]]; then
    echo "::error::$repo release branch changed."
    exit 1
  fi

  # The head MUST be the exact prepared commit from the immutable manifest.
  prepared_head=$(jq -r --arg repo "$repo" '.consumers[$repo].release_sha // empty' "$manifest_file")
  validated_sha=$(jq -r --arg repo "$repo" '.consumers[$repo].validated_sha // empty' "$manifest_file")
  if [[ ! "$prepared_head" =~ ^[0-9a-f]{40}$ || ! "$validated_sha" =~ ^[0-9a-f]{40}$ || "${head,,}" != "$prepared_head" ]]; then
    echo "::error::$repo PR commit differs from validated release manifest."
    exit 1
  fi
  gh pr checks "$number" --repo "$full_repo"

  pr_numbers[$repo]="$number"
  expected_heads[$repo]="$head"
  expected_bases[$repo]="$base"

  echo "$repo PR #$number passed final preflight."
done

echo "Both repositories passed final preflight."

if [[ "$DRY_RUN" == "true" ]]; then
  for repo in "${repositories[@]}"; do
    echo "DRY RUN: Would rebase and merge $repo PR #${pr_numbers[$repo]}"
    echo "DRY RUN: Would wait for $repo main CI."
  done

  echo "DRY RUN: No repositories were modified."
  exit 0
fi

echo "=== APPROVED RELEASE EXECUTION ==="

# Merge and verify one consumer at a time.
for repo in "${repositories[@]}"; do
  full_repo="robm675/$repo"
  number="${pr_numbers[$repo]}"
  expected_head="${expected_heads[$repo]}"
  expected_base="${expected_bases[$repo]}"

  echo "Rechecking $repo PR #$number"

  pr=$(gh pr view "$number" \
    --repo "$full_repo" \
    --json state,isDraft,headRefOid,baseRefOid,mergeable)

  if [[ "$(jq -r '.state' <<< "$pr")" != "OPEN" ||
        "$(jq -r '.isDraft' <<< "$pr")" != "false" ||
        "$(jq -r '.mergeable' <<< "$pr")" != "MERGEABLE" ||
        "$(jq -r '.headRefOid' <<< "$pr")" != "$expected_head" ||
        "$(jq -r '.baseRefOid' <<< "$pr")" != "$expected_base" ]]; then
    echo "::error::$repo PR changed since final preflight."
    exit 1
  fi

  current_main=$(gh api \
    "repos/$full_repo/git/ref/heads/main" \
    --jq '.object.sha')

  if [[ "$current_main" != "$expected_base" ]]; then
    echo "::error::$repo main changed before merge."
    exit 1
  fi

  gh pr checks "$number" --repo "$full_repo"

  echo "Merging $repo PR #$number"

  gh pr merge "$number" \
    --repo "$full_repo" \
    --rebase \
    --match-head-commit "$expected_head"

  merged=$(gh pr view "$number" \
    --repo "$full_repo" \
    --json state \
    --jq '.state')

  if [[ "$merged" != "MERGED" ]]; then
    echo "::error::$repo PR did not merge successfully."
    exit 1
  fi

  new_main=$(gh api \
    "repos/$full_repo/git/ref/heads/main" \
    --jq '.object.sha')

  if [[ "$new_main" == "$expected_base" ]]; then
    echo "::error::$repo main did not advance."
    exit 1
  fi

  echo "$repo main commit: $new_main"
  echo "Waiting for the main push workflow..."

  # Require the one designated push workflow for THIS exact main commit.
  workflow_id="${workflow_ids[$repo]}"
  verified=false
  for attempt in $(seq 1 90); do
    runs=$(gh api "repos/$full_repo/actions/workflows/$workflow_id/runs?branch=main&event=push&per_page=100")
    matching=$(jq --arg sha "$new_main" '[.workflow_runs[] | select(.head_sha == $sha and .event == "push")]' <<< "$runs")
    count=$(jq 'length' <<< "$matching")
    if [[ "$count" -gt 0 ]]; then
      # GitHub may briefly report queued, in_progress, or completed.
      failures=$(jq '[.[] | select(.status == "completed" and .conclusion != "success")] | length' <<< "$matching")
      pending=$(jq '[.[] | select(.status != "completed")] | length' <<< "$matching")
      if [[ "$failures" -gt 0 ]]; then
        echo "::error::$repo required main workflow failed."
        exit 1
      fi
      if [[ "$pending" -eq 0 ]]; then
        echo "$repo required main CI passed for $new_main."
        verified=true
        break
      fi
    fi
    echo "Waiting for required $repo main CI ($attempt/90)"
    sleep 10
  done
  if [[ "$verified" != "true" ]]; then
    echo "::error::$repo required main CI did not complete successfully within 15 minutes."
    exit 1
  fi

  echo "$repo merge and verification completed."
done

echo "=== COORDINATED RELEASE COMPLETE ==="
echo "CSharp and API main workflows passed."
