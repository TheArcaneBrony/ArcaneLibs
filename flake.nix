{
  inputs.nixpkgs.url = "github:nixos/nixpkgs/nixos-unstable";
  inputs.flake-utils.url = "github:numtide/flake-utils";

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
    }:
    let
      pkgs = nixpkgs.legacyPackages.x86_64-linux;
      rVersion = let
        rev = self.sourceInfo.shortRev or self.sourceInfo.dirtyShortRev;
        date = builtins.substring 0 8 self.sourceInfo.lastModifiedDate;
        time = builtins.substring 8 6 self.sourceInfo.lastModifiedDate;
      in "preview.${date}-${time}+${rev}";
      makeNupkg =
        {name, nugetDeps ? null, projectReferences ? []}:
        pkgs.buildDotnetModule rec {
          inherit projectReferences nugetDeps;
          
          pname = "${name}";
          version = "1.0.0-" + rVersion;
          dotnetPackFlags = [ "--include-symbols" "--include-source" "--version-suffix ${rVersion}" ];
          dotnet-sdk = pkgs.dotnet-sdk_10;
          dotnet-runtime = pkgs.dotnet-aspnetcore_10;
          src = ./.;
          projectFile = [
            "${name}/${name}.csproj"
          ];
#          nugetDeps = if (depsFile != "") then (./. + depsFile) else null;
          packNupkg = true;
          meta = with pkgs.lib; {
            description = "ArcaneLibs .NET Libraries";
            homepage = "";
            license = licenses.agpl3Plus;
            maintainers = with maintainers; [ RorySys ];
          };
          #nativeBuildInputs = with pkgs; [
          #  pkg-config
          #];
        };
    in
    {
      packages.x86_64-linux = {
        ArcaneLibs = makeNupkg { name = "ArcaneLibs"; };
        ArcaneLibs-Blazor-Components = makeNupkg { name = "ArcaneLibs.Blazor.Components"; nugetDeps = ./ArcaneLibs.Blazor.Components/deps.json; };
        ArcaneLibs-Legacy = makeNupkg { name = "ArcaneLibs.Legacy"; };
        ArcaneLibs-Logging = makeNupkg { name = "ArcaneLibs.Logging"; };
        ArcaneLibs-StringNormalisation = makeNupkg { name = "ArcaneLibs.StringNormalisation"; nugetDeps = ./ArcaneLibs.StringNormalisation/deps.json; };
        ArcaneLibs-Timings = makeNupkg { name = "ArcaneLibs.Timings"; };
      };
    };
}
