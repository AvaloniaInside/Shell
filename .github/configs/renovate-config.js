module.exports = {
  "extends": ["config:base"],
  "platform": "github",
  "branchPrefix": "renovate/",
  "gitAuthor": "Renovate Bot <bot@renovateapp.com>",
  "automerge": false,
  "forkProcessing": "enabled",
  "allowedCommands": [".*"],
  "autodiscover": true,
  "prCreation": "immediate"
};
