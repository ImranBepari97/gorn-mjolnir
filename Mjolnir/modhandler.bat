setlocal
echo "Moving your mod's dll to its respective folder..."

SET modFolder=F:\Program Files\Steam\steamapps\common\GORN\Mods\Mjolnir\Mjolnir.dll
SET dllLocation=F:\Users\Imran\Desktop\Games\GORN Modding\2.0 Mods\Mjolnir\Mjolnir\bin\Debug\Mjolnir.dll
SET dllName=Mjolnir

move "%dllLocation%" "%modFolder%"

echo "%dllName% has been moved to %modFolder%!"
endlocal