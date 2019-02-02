# StrubTUtilities

This repository contains various helpful utilities that I currently use in a number of my projects.

## Repository Contents

The repository contains a single library that can be compiled using several configurations and target frameworks.

### Supported Configurations

| Configuration | Description | Dependencies |
|:--- |:--- |:--- |
| `Release-Minimal` | minimal build | `System.ValueTuple` *(.NET Framework 4.5 only)* |
| `Release-FSharp` | additional F# utilities | `FSharp.Core` |
| `Release-Console` | advanced console utilities | `kernel32.dll` and `System.Drawing` |
| `Release`<br>`Debug` | full build | all of the above |

### Supported Target Frameworks

| Target Framework | Limitations |
|:--- |:--- |
| **.NET Framework 4.5** | – |
| **.NET Framework 4.7.2** | – |
| **.NET Standard 2.0** | does not support advanced console utilities |

## Usage

Simply add the desired version of the library to your .NET project.
