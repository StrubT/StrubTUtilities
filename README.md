# StrubTUtilities

This repository contains various helpful utilities that I currently use in a number of my projects.

## Repository Contents

All of the utilities are available in the following versions of the C# programming language:
* C# 7.2 ([`csharp-71`](https://github.com/StrubT/StrubTUtilities/tree/csharp-72))
* C# 7.1 ([`csharp-71`](https://github.com/StrubT/StrubTUtilities/tree/csharp-71))
* C# 7.0 ([`csharp-70`](https://github.com/StrubT/StrubTUtilities/tree/csharp-70))
* C# 6.0 ([`csharp-60`](https://github.com/StrubT/StrubTUtilities/tree/csharp-60))
* C# 5.0 ([`csharp-50`](https://github.com/StrubT/StrubTUtilities/tree/csharp-50))

Please note that for simplicity every branch contains a Visual Studio 2017 solution
with a shared project as well as an empty console application with the correct C# language version.

## Pre-processor Directives

A number of pre-processor directives have been defined to enable advanced features.
Most of these features will require you to add additional references to your project.
* Advanced console utilities
  * `ENABLE_KERNEL32_DLL` (depends on *kernel32.dll*)
  * `ENABLE_SYSTEM_DRAWING` (depends on *System.Drawing.dll*)
* F# utilities
  * `ENABLE_FSHARP_CORE` (depends on *FSharp.Core.dll* – i.e. add [*FSharp.Core* from NuGet](https://www.nuget.org/packages/FSharp.Core/))

## Usage

* I suggest that you add this repository as a git submodule (with `-b csharp-<language version>`).
* If you use **Visual Studio 2017 or 2015**, you can then directly add the shared project to your solution
  and add it as a reference for the project(s) you wish to use the utilities in.
* If you use **Visual Studio 2013 or 2012**, you can't use the shared project, because they are not supported.
  But you can still link the code files in Visual Studio directly (*Add Existing Item* and *Add As Link*).
  If you added the repository as a git submodule, these links are safe and should work wherever you clone your project.
* If you use **Visual Studio 2010 or earlier**, you have to manually translate the code to the appropriate C# language version.
