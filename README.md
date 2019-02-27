# CSharp-CBS
Reloaded C# Crypter Bind Shell
Coded with VS Community 2015. How does it works?

Files:
 BindShell.cs: the source code of the payload (you can change a different one), compile it as DLL
 Encoder.cs: the RC4 encoder. Compile it as Console app
 Stub.cs: the executable wrapping the encrypted payload. Compile it as Console app
 
Logic:
BindShell is compiled ad a DLL, then is encrypted using the Encoder as .dat file. The BindShell.dat file is embedded as resource on the Stub VS project and decrypted at runtime when executed, then the _bind method is invoked. You should get a CMD shell listening on the TCP 6666. Eventually you can pass to the Stub the IP and the port, e.g. Stub.exe 192.168.1.66 9999
