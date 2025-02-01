{ packages, formatter, pre-commit-lib }:
pre-commit-lib.run {
  src = ./.;

  # hooks
  hooks = {
    # formatter
    treefmt = {
      enable = true;
      excludes = [
        ".*(Changelog|README|CommitConventions).+(MD|md)"
      ];
    };

    # linters From https://github.com/cachix/pre-commit-hooks.nix
    shellcheck = {
      enable = false;
    };

    a-dotnet-lint-Lithium = {
      enable = true;
      name = "Lint .NET 'Lithium' Project";
      description = "Run formatter for .NET Project 'Lithium'";
      entry = "${packages.dotnet}/bin/dotnet format style --no-restore --severity info --verify-no-changes -v d ./Lithium/Lithium.csproj";
      language = "system";
      pass_filenames = false;
      files = "^Lithium/.*\\.cs$";
    };

    a-dotnet-lint-unit-test = {
      enable = true;
      name = "Lint .NET 'Unit Test' Project";
      description = "Run formatter for .NET Project 'Unit Test'";
      entry = "${packages.dotnet}/bin/dotnet format style --no-restore --severity info --verify-no-changes -v d ./UnitTest/UnitTest.csproj";
      language = "system";
      pass_filenames = false;
      files = "^UnitTest/.*\\.cs$";
    };

    a-infisical = {
      enable = true;
      name = "Secrets Scanning";
      description = "Scan for possible secrets";
      entry = "${packages.infisical}/bin/infisical scan . --verbose";
      language = "system";
      pass_filenames = false;
    };

    a-infisical-staged = {
      enable = true;
      name = "Secrets Scanning (Staged files)";
      description = "Scan for possible secrets in staged files";
      entry = "${packages.infisical}/bin/infisical scan git-changes --staged -v";
      language = "system";
      pass_filenames = false;
    };

    a-gitlint = {
      enable = true;
      name = "Gitlint";
      description = "Lints git commit message";
      entry = "${packages.gitlint}/bin/gitlint --staged --msg-filename .git/COMMIT_EDITMSG";
      language = "system";
      pass_filenames = false;
      stages = [ "commit-msg" ];
    };

    a-enforce-gitlint = {
      enable = true;
      name = "Enforce gitlint";
      description = "Enforce atomi_releaser conforms to gitlint";
      entry = "${packages.sg}/bin/sg gitlint";
      files = "(atomi_release\\.yaml|\\.gitlint)";
      language = "system";
      pass_filenames = false;
    };

    a-shellcheck = {
      enable = true;
      name = "Shell Check";
      entry = "${packages.shellcheck}/bin/shellcheck";
      files = ".*sh$";
      language = "system";
      pass_filenames = true;
    };

    a-enforce-exec = {
      enable = true;
      name = "Enforce Shell Script executable";
      entry = "${packages.atomiutils}/bin/chmod +x";
      files = ".*sh$";
      language = "system";
      pass_filenames = true;
    };
  };

  settings = {
    treefmt = {
      package = formatter;
    };
  };
}
