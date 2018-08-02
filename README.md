# Azur-Lane-LuaHelper
A helper tool to decrypt and encrypt, decompile and recompile Azur Lane's lua.

## System Requirements
1. NET Framework 4.5.1 or newer https://www.microsoft.com/en-us/download/details.aspx?id=40779
2. Python 3.7.0 https://www.python.org/ftp/python/3.7.0/python-3.7.0.exe
3. Added Python's directory into System Environment Variables named **Path** https://docs.python.org/3/using/windows.html#configuring-python

## Commands and Usage Example
```
[INFO]> Usage: Azurlane.exe <option> <path-to-file(s) or path-to-directory(s)
[INFO]> You can input multiple files or directories, Lua & AssetBundle are the only acceptable file.

Options:
  -u, --unlock               Decrypt Lua
  -l, --lock                 Encrypt Lua
  -d, --decompile            Decompile Lua (will automatically decrypt if Lua
                               is encrypted)
  -r, --recompile            Recompile Lua
      --decrypt              Decrypt AssetBundle
      --encrypt              Encrypt AssetBundle
      --unpack               Unpack all lua from AssetBundle (will
                               automatically decrypt if AssetBundle is
                               encrypted)
      --repack               Repack all lua from AssetBundle
```
### Usage Example
```
$ ./Azurlane.exe --unpack scripts
[22:37:57][INFO]> Decrypting scripts... <Done>
[22:37:57][INFO]> Unpacking scripts... <Done>

Unpacking assetbundle is done

$ ./Azurlane.exe --unlock Unity_Assets_Files
[22:38:43][INFO]> Decrypting 0000000.lua.txt... <Done>
[22:38:43][INFO]> Decrypting 0000001.lua.txt... <Done>
[22:38:43][INFO]> Decrypting 1000.lua.txt... <Done>
[22:38:43][INFO]> Decrypting 2000.lua.txt... <Done>
[22:38:43][INFO]> Decrypting 2001.lua.txt... <Done>
[22:38:43][INFO]> Decrypting 2002.lua.txt... <Done>
[22:38:43][INFO]> Decrypting 2003.lua.txt... <Done>
[22:38:43][INFO]> Decrypting 2004.lua.txt... <Done>

Decrypt is done
Success: 8 - Failed: 0

$ ./Azurlane.exe --decompile Decrypted_lua
[22:38:55][INFO]> Decompiling 0000000.lua.txt... <Done>
[22:38:57][INFO]> Decompiling 0000001.lua.txt... <Done>
[22:38:58][INFO]> Decompiling 1000.lua.txt... <Done>
[22:39:00][INFO]> Decompiling 2000.lua.txt... <Done>
[22:39:01][INFO]> Decompiling 2001.lua.txt... <Done>
[22:39:01][INFO]> Decompiling 2002.lua.txt... <Done>
[22:39:02][INFO]> Decompiling 2003.lua.txt... <Done>
[22:39:03][INFO]> Decompiling 2004.lua.txt... <Done>

Decompile is done
Success: 8 - Failed: 0

$ ./Azurlane.exe --recompile Decompiled_lua
[22:39:11][INFO]> Recompiling 0000000.lua.txt... <Done>
[22:39:12][INFO]> Recompiling 0000001.lua.txt... <Done>
[22:39:12][INFO]> Recompiling 1000.lua.txt... <Done>
[22:39:12][INFO]> Recompiling 2000.lua.txt... <Done>
[22:39:13][INFO]> Recompiling 2001.lua.txt... <Done>
[22:39:13][INFO]> Recompiling 2002.lua.txt... <Done>
[22:39:13][INFO]> Recompiling 2003.lua.txt... <Done>
[22:39:13][INFO]> Recompiling 2004.lua.txt... <Done>

Recompile is done
Success: 8 - Failed: 0

$ ./Azurlane.exe --lock Recompiled_lua
[22:39:25][INFO]> Encrypting 0000000.lua.txt... <Done>
[22:39:25][INFO]> Encrypting 0000001.lua.txt... <Done>
[22:39:25][INFO]> Encrypting 1000.lua.txt... <Done>
[22:39:25][INFO]> Encrypting 2000.lua.txt... <Done>
[22:39:25][INFO]> Encrypting 2001.lua.txt... <Done>
[22:39:25][INFO]> Encrypting 2002.lua.txt... <Done>
[22:39:25][INFO]> Encrypting 2003.lua.txt... <Done>
[22:39:25][INFO]> Encrypting 2004.lua.txt... <Done>

Encrypt is done
Success: 8 - Failed: 0

```
## Known Issues
### System Locale
AssetBundle extractor (UnityEx.exe) will not working properly when your System Locale is not English, you can either try extract it manually or use another AssetBundle extractor such as UABE.
