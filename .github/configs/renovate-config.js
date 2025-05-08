module.exports = {
  "extends": ["config:recommended", ":disableDependencyDashboard"],
  "repositories": ["AvaloniaInside/Shell"],
  "platform": "github",
  "branchPrefix": "renovate/",
  "gitAuthor": "Renovate Bot <bot@renovateapp.com>",
  "automerge": false,
  "forkProcessing": "enabled",
  "allowedCommands": [".*"],
  "prCreation": "immediate"
};
