export DATE=`git log -1 --format="%at" | xargs -I{} date -d @{} '+%Y%m%d-%H%M%S'`
#export DATE=`date -u '+%Y%m%d-%H%M%S'`
export REV=`git rev-parse --short HEAD`
echo "preview.$DATE+$REV" > version.txt
git add version.txt

for p in `nix flake show --json | jq '.packages."x86_64-linux" | keys[]' -r`
do
    nix build .\#${p} -j4 -L --out-link ./result-${p//-/\.} &
done
wait

#set +x
dotnet nuget push result*/share/nuget/source/*/*/*.nupkg -k ${NUGET_KEY} --source https://api.nuget.org/v3/index.json --skip-duplicate