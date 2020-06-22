# Bottlenose Grade Checker
 A CLI tool to check your grades and recent assignments for courses using [Bottlenose](https://github.com/CodeGrade/bottlenose).

# Usage
Build and run (requres [.NET runtime](https://github.com/dotnet/runtime))

    1. dotnet build "Bottlenose Grade Checker.csproj" -c Release -o /app/build
    2. dotnet publish "Bottlenose Grade Checker.csproj" -c Release -o /app/publish
    3. dotnet "Bottlenose Grade Checker.dll"

----

You can also specify a username & password via the program arguments to bypass the prompt. The password should be in [Rot-13](https://rot13.com/) cipher format.

Example (username "hello", password "world"):
`dotnet "Bottlenose Grade Checker.dll" -- hello wbeyq`.

# Screenshot
![](https://i.imgur.com/we5lRjo.png)
