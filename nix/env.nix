{ pkgs, packages }:
with packages;
{
  system = [
    coreutils
    findutils
    gnugrep
    gnused
    bash
    jq
    yq
    curl
    xmlstarlet
  ];

  dev = [
    pls
    git
  ];

  main = [
    dotnet
    infisical
  ];

  lint = [
    # core
    treefmt
    gitlint
    shellcheck
    sg
  ];

  releaser = [
    sg
  ];

}
