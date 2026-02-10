# Next User Story

Execute the next pending user story from the current sprint.

## Workflow

### Phase 0: Clean Slate

1. **Check for uncommitted work**: Run `git status`. If there are uncommitted changes or untracked files:
   a. Stage and commit the changes with an appropriate message following project conventions
   b. Push the current branch
   c. Create a PR via `gh pr create` if not already created
   d. Merge the PR via `gh pr merge --auto --squash`
   e. Switch to `main` and pull: `git checkout main && git pull`
2. **If working tree is clean**: Pull latest from main: `git checkout main && git pull`

### Phase 1: Identify & Plan

3. **Identify the next story**: Read `docs/sprints/sprint-1-status.md` (or the current sprint file referenced in MEMORY.md). Find the first user story with status `🔴 Not Started` or `🟡 In Progress`. If `$ARGUMENTS` specifies a story ID (e.g., `US-1.3`), use that instead.

4. **Read context**: Read the story's tasks, acceptance criteria, and any referenced documentation (e.g., `docs/database-schema.md`, `docs/master-plan.md`). Read any existing source files that will be modified.

5. **Enter plan mode**: Use EnterPlanMode to design the implementation approach for the entire user story. The plan should cover all tasks in the story, identify files to create/modify, and flag any blockers or questions.

### Phase 2: Execute Tasks

6. **After plan approval, create task list**: Use TaskCreate to add one task per TASK-X.X.Y in the story. Set up dependencies (blockedBy) where tasks depend on earlier ones.

7. **For each task, sequentially**:
   a. Mark the task as `in_progress`
   b. Create a feature branch from `main`: `git checkout main && git pull && git checkout -b feature/US-X.X-TASK-Y-description`
   c. Implement the task with tests where applicable
   d. Run `npm run lint && npm run test:run && npm run build` to verify
   e. Commit with message format: `feat(US-X.X): description` including `TASK-X.X.Y` reference
   f. Update the sprint status file: mark the task row as `🟢` with notes
   g. Commit the status update
   h. Push the branch and create a PR via `gh pr create`
   i. Merge the PR via `gh pr merge --auto --squash`
   j. Switch back to main and pull: `git checkout main && git pull`
   k. Mark the task as `completed`

### Phase 3: Wrap Up

8. **After all tasks complete**:
   a. Update the user story status to `🟢 Complete` in the sprint status file
   b. Commit and push the final status update
   c. Update MEMORY.md with the new project state and any learnings
   d. Summarize what was accomplished and what the next story is

## Important Rules

- Never commit directly to `main` - always use feature branches
- Run lint + test + build before every commit
- Keep commits atomic and focused on one task
- Update sprint status after each task, not just at the end
- If a task is blocked or needs clarification, use AskUserQuestion before proceeding
- Reference task IDs in all commit messages
