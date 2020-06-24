# Bottlenose Grade Checker
 A CLI tool to check your grades and recent assignments for courses using [Bottlenose](https://github.com/CodeGrade/bottlenose).

# Instructions
- Run the appropriate build for your OS.
- Fill in user/pass prompts
- - You can also specify a username & password via the program arguments to bypass the prompt. The password should be in [Rot-13](https://rot13.com/) cipher format.
- BGC will retrieve your latest assignments & grades from Bottlenose

&nbsp;

Example usage (username `hello`, password `world`):

`dotnet "Bottlenose Grade Checker.dll" -- hello wbeyq`.

----
# Build Source
**Note:** Requires [dotnet runtime](https://github.com/dotnet/runtime)

    1. dotnet build "Bottlenose Grade Checker.csproj" -c Release -o /app/build
    2. dotnet publish "Bottlenose Grade Checker.csproj" -c Release -o /app/publish
    3. dotnet "Bottlenose Grade Checker.dll"

----
# Screenshot
![](https://i.imgur.com/6HGB1Jv.png)
