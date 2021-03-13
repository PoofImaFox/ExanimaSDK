
# ExanimaSDK
This is a fully documented and extensive C# SDK for the Exanima game.

## Features:
- Read from resource files 
- Unpack resource files
- Repack resource files

## TODO:
- Read from save files
- Edit save files
- Import new items into save files
- Read from Rdb files
- Edit Rdb files
- Repack Rdb files

## Code Snippets:
*Unpack resource files* 
```c#
var packedRpkFile = "packedFile.rpk";
var resourceFile = new ResourcePackFile(packedRpkFile);
var packedFiles = resourceFile.ReadPackedFiles();
var unpackLocation = "unpackedTestdir";
Directory.CreateDirectory(unpackLocation);

var runningTaskList = new Task[packedFiles.Length];
for (var x = 0; x < packedFiles.Length; x++) {
	runningTaskList[x] = UnpackFileAsync(packedFiles[x], resourceFile, unpackLocation);
}
Task.WaitAll(runningTaskList);
```
