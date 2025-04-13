{ pkgs, packages }:
with packages;
{
  system = [
    atomiutils
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
