{
  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
  inputs.flake-utils.url = "github:numtide/flake-utils";

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
    }:
    let
      pkgs = nixpkgs.legacyPackages.x86_64-linux;
      ver = "1.0.1";
      rVersion =
        let
          rev = self.sourceInfo.shortRev or self.sourceInfo.dirtyShortRev;
          date = builtins.substring 0 8 self.sourceInfo.lastModifiedDate;
          time = builtins.substring 8 6 self.sourceInfo.lastModifiedDate;
        in
        "preview.${date}-${time}"; # +${rev}";
      makeNupkg =
        {
          name,
          nugetDeps ? null,
          projectReferences ? [ ],
        }:
        pkgs.buildDotnetModule rec {
          inherit projectReferences nugetDeps;

          pname = "${name}";
          version = ver + "-" + rVersion;
          dotnetPackFlags = [
            "--include-symbols"
            "--include-source"
            "--version ${ver}-${rVersion}"
            # "--version-suffix ${rVersion}"
          ];
          dotnet-sdk = pkgs.dotnet-sdk_10;
          dotnet-runtime = pkgs.dotnet-aspnetcore_10;
          src = pkgs.lib.cleanSource ./.;
          projectFile = [
            "${name}/${name}.csproj"
          ];
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
        ArcaneLibs-Blazor-Components = makeNupkg {
          name = "ArcaneLibs.Blazor.Components";
          nugetDeps = ./ArcaneLibs.Blazor.Components/deps.json;
        };
        ArcaneLibs-Legacy = makeNupkg { name = "ArcaneLibs.Legacy"; };
        ArcaneLibs-Logging = makeNupkg { name = "ArcaneLibs.Logging"; };
        ArcaneLibs-StringNormalisation = makeNupkg {
          name = "ArcaneLibs.StringNormalisation";
          nugetDeps = ./ArcaneLibs.StringNormalisation/deps.json;
        };
        ArcaneLibs-Timings = makeNupkg { name = "ArcaneLibs.Timings"; };
      };
      checks = pkgs.lib.attrsets.unionOfDisjoint {
        # Actual checks
      } self.packages;
      nupkgs.x86_64-linux = pkgs.lib.mapAttrs (
        name: pkg:
        (
          with pkgs;
          pkgs.runCommand (pkg.pname + "." + pkg.version + ".nupkg") { } ''
            echo 'Creating zip archive for ${name}'
            set -x
            cd "${pkg.out}/share/nuget/packages/${lib.toLower pkg.pname}/${pkg.version}"
            ls -la
            # NuGet doesn't care about compression flags
            ${lib.getExe pkgs.zip} -db -ds 32k -9 -r "$out" *
            set +x
          ''
        )
      ) self.packages.x86_64-linux;
      nugetArtifactDir.x86_64-linux =
        let
          outPaths = pkgs.lib.mapAttrsToList (name: pkg: pkg.out) self.nupkgs.x86_64-linux;
        in
        pkgs.runCommand ("ArcaneLibs-" + ver + "-" + rVersion + "-nuget-artifacts") { } ''
          mkdir -p $out
          for path in ${pkgs.lib.concatStringsSep " " outPaths}; do
            ln -vs ''\${path} $out/
          done
        '';
      addNugetSource =
        let
          outPaths = pkgs.lib.mapAttrsToList (name: pkg: pkg.out) self.nupkgs.x86_64-linux;
          nugetDir = pkgs.runCommand ("ArcaneLibs-" + ver + "-" + rVersion + "-nuget-artifacts") { } ''
            mkdir -p $out
            # ${pkgs.dotnet-sdk_10}/bin/dotnet nuget add source --name 'out' "$out"
            export DOTNET_CLI_HOME=/tmp
            for path in ${pkgs.lib.concatStringsSep " " outPaths}; do
              ${pkgs.dotnet-sdk_10}/bin/dotnet nuget push "$path" --source "$out" &
            done
            wait 
          '';
        in
        pkgs.writeScriptBin "add-nuget-source" ''
          #!/bin/sh
          SRC_NAME='ArcaneLibs-${rVersion}'
          #SRC_DEST='${self.nugetArtifactDir."${pkgs.stdenv.hostPlatform.system}"}'
          SRC_DEST='${nugetDir}'

          # echo "Adding NuGet source '$SRC_NAME' pointing to '$SRC_DEST'..."
          # dotnet nuget add source --name 'ArcaneLibs-${rVersion}' "$SRC_DEST"
          dotnet nuget add source --name "$SRC_DEST" "$SRC_DEST"
        '';
    };
}
