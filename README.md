# CSharp-CBS
### 24/07/20: the Stub.exe file is now detected by almost every AV solution

Reloaded C# Crypter Bind Shell
Coded with VS Community 2015. How does it works?

Files:<br>
 BindShell.cs: the source code of the payload (you can code what you prefer), compile it as DLL<br>
 Encoder.cs: the RC4 encoder. Compile it as Console app<br>
 Stub.cs: the executable wrapping the encrypted payload. Compile it as Console app<br><br>
 
Logic:<br>
BindShell is compiled as a DLL, then is encrypted using the Encoder as .dat file. The BindShell.dat file is embedded as resource on the Stub VS project and decrypted at runtime when executed, then the _bind method is invoked. You should get a CMD shell listening on the TCP 6666. Eventually you can pass to the Stub the IP and the port, e.g. Stub.exe 192.168.1.66 9999
