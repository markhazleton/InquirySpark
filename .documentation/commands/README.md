# DevSpark Command Overrides

This folder is reserved for intentional team-level DevSpark command overrides.

Current state: no command overrides are active in this repository.

Resolution order for commands:

1. `.documentation/{git-user}/commands/`
2. `.documentation/commands/`
3. `.devspark/defaults/commands/`

Create a file here only when the team explicitly wants to override a stock `devspark.*` command.