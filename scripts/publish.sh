#DATE=`date -u '+%Y%m%d-%H%M%S'`
DATE=`git log -1 --format="%at" | xargs -I{} date -d @{} '+%Y%m%d-%H%M%S'`
REV=`git rev-parse --short HEAD`
BASEDIR="$PWD"
echo "preview.$DATE+$REV" > version.txt
git add version.txt
rm ./result* *.nupkg

for p in `nix flake show --json | jq '.packages."x86_64-linux" | keys[]' -r`
do
    nix build .\#${p} -j4 -L --out-link ./result-${p//-/\.} &
done
wait

for p in result*/share/nuget/packages/*/*/.unpacked
do
    PNAME=$(basename `realpath "${p}/../.."`)
    echo $PNAME:
    cd "${p}"
    zip -db -ds 32k -9 -r "${BASEDIR}/${PNAME//./-}.nupkg" *
    cd -
done

dotnet nuget push *.nupkg -k ${NUGET_KEY} --source https://api.nuget.org/v3/index.json --skip-duplicate
git restore version.txt