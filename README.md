# PortScanner

Simple port-scanning application made in C#, using `System.Net`. 

Scans for open port `port` across IP adresses from range `addressStart` to `addressEnd` (both inclusive), while writing found adresses to a file. Uses multithreading for increased performance, albeit compromises on output quality. There's a small problem, where a fraction (<1%) of found IPs are being either split in half or duplicated when saving. I've tried using **lock** statement where I save them, but it didn't help at all. I've noticed setting `sw.AutoFlush` to `false` instead of `true` helps a bit. I am aware that the problem is caused by multithreaded nature of the application, but I just can't be bothered enough to fix it.

<br>

## How to run

I think it's pretty obvious. Modify the contents of the `Program.cs` to your liking, then go to the root directory and run `dotnet build`. Once finished, either use `dotnet run`, or go three directories deep into `/bin` and run the executable directly. The main difference between those two methods is where the output file gets saved. Using `dotnet run`, `out.txt` will be saved in root directory. Otherwise, where the executable is located.

It's worth mentioning that the project uses .NET 8. If you'd like to run it as it is, you need to have .NET 8 installed. Otherwise, just modify `.csproj` and you'll be fine.
