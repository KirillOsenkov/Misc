Obtain perfview.exe from https://github.com/microsoft/perfview/releases/latest and ensure it's on the path.

From an admin command prompt, start.cmd to start recording, stop.cmd to stop recording.

It will record all .NET first-chance exceptions system-wide with all managed+native callstacks, and use symbols where it can to resolve locations to method names. It will also record the command line for each process, parent process, loaded modules for each process and other useful info.

