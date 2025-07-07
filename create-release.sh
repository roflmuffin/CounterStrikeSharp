#!/bin/bash

set -e

DRY_RUN=false
if [[ "$1" == "--dry-run" || "$1" == "-d" ]]; then
    DRY_RUN=true
    echo "Running in DRY-RUN mode - no changes will be pushed"
fi

echo "Starting automated release process..."

echo "Fetching latest tags from remote..."
git fetch --tags

LATEST_TAG=$(git describe --tags --abbrev=0 2>/dev/null || echo "v1.0.0")
echo "Latest tag found: $LATEST_TAG"

if [[ $LATEST_TAG =~ ^v([0-9]+)\.([0-9]+)\.([0-9]+)$ ]]; then
    MAJOR=${BASH_REMATCH[1]}
    MINOR=${BASH_REMATCH[2]}
    PATCH=${BASH_REMATCH[3]}
else
    echo "Error: Could not parse version from tag $LATEST_TAG"
    echo "Expected format: v1.0.x (e.g., v1.0.322)"
    exit 1
fi

NEW_PATCH=$((PATCH + 1))
NEW_TAG="v$MAJOR.$MINOR.$NEW_PATCH"

echo "New version will be: $NEW_TAG"

echo "Generating changelog with git-cliff..."
npx git-cliff -o CHANGELOG.md -t "$NEW_TAG"

if ! git diff --quiet CHANGELOG.md; then
    echo "Changelog updated successfully"
    
    git add CHANGELOG.md
    
    COMMIT_MSG="release: $NEW_TAG"
    echo "Committing changelog with message: $COMMIT_MSG"
    
    if [ "$DRY_RUN" = true ]; then
        git commit -m "$COMMIT_MSG"
        
        echo "Creating tag locally: $NEW_TAG"
        git tag "$NEW_TAG"
        
        echo "DRY-RUN: Would push commit to remote"
        echo "DRY-RUN: Would push tag to remote"
    else
        git commit -m "$COMMIT_MSG"
        
        echo "Pushing commit to remote..."
        git push origin $(git branch --show-current)
        
        echo "Creating and pushing tag: $NEW_TAG"
        git tag "$NEW_TAG"
        git push origin tag "$NEW_TAG"
    fi
    
    echo "Release $NEW_TAG completed successfully!"
    echo "Summary:"
    echo "   - Previous version: $LATEST_TAG"
    echo "   - New version: $NEW_TAG"
    echo "   - Changelog updated: Yes"
    if [ "$DRY_RUN" = true ]; then
        echo "   - Commit pushed: (dry-run)"
        echo "   - Tag created and pushed: (dry-run)"
    else
        echo "   - Commit pushed: Yes"
        echo "   - Tag created and pushed: Yes"
    fi
else
    echo "No changes detected in CHANGELOG.md"
    echo "This might indicate that there are no new commits since the last release."
    exit 1
fi