Bezier curve approximation algorithm with biarcs
==================================

![Appveyor](https://ci.appveyor.com/api/projects/status/github/domoszlai/bezier2biarc?branch=maui&svg=true)

## Synopsis

Simple C# application to illustrate the algorithm of approximating a cubic bezier curve with biarcs.
The algorithm will also be available in Haskell, incorporated into my [JuicyGcode](https://github.com/domoszlai/svg2gcode) project.

## Implementation

The algorithm is explained in the blog post at http://dlacko.org/blog/2016/10/19/approximating-bezier-curves-by-biarcs/.

This is a new implementation based on dotnet [MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui?view=net-maui-7.0)
as the original Windows.Form implementation put a hard requirement on Windows. You can still find theoriginal one on the `master` branch.

## Installation

Use the community edition of Visual Studio 2022 [for Mac], open BiArcTutorial.csproj and pray.
