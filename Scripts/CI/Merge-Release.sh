#!/usr/bin/env bash
set -euo pipefail

repositories=(
  RobsSpacePatrol.CSharp
  RobsSpacePatrol.API
)

declare -A pr_numbers
declare -A expected_heads
declare -A expected_bases

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

  # Allow up to 10 minutes for Actions to complete.
  verified=false

  for attempt in $(seq 1 60); do
    runs=$(gh run list \
      --repo "$full_repo" \
      --branch main \
      --commit "$new_main" \
      --event push \
      --limit 30 \
      --json databaseId,status,conclusion)

    count=$(jq 'length' <<< "$runs")

    if [[ "$count" -gt 0 ]]; then
      pending=$(jq \
        '[.[] | select(.status != "completed")] | length' \
        <<< "$runs")

      failures=$(jq \
        '[.[] | select(.status == "completed" and .conclusion != "success")] | length' \
        <<< "$runs")

      if [[ "$failures" -gt 0 ]]; then
        echo "::error::$repo main workflow failed."
        exit 1
      fi

      if [[ "$pending" -eq 0 ]]; then
        echo "$repo main CI passed."
        verified=true
        break
      fi
    fi

    echo "Waiting for $repo CI ($attempt/60)"
    sleep 10
  done

  if [[ "$verified" != "true" ]]; then
    echo "::error::$repo main CI verification timed out."
    exit 1
  fi

  echo "$repo merge and verification completed."
done

echo "=== COORDINATED RELEASE COMPLETE ==="
echo "CSharp and API main workflows passed."
