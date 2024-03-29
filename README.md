Bezier curve approximation algorithm with biarcs
==================================

[![Appveyor](https://ci.appveyor.com/api/projects/status/github/domoszlai/juicy-gcode?branch=master&svg=true)](https://ci.appveyor.com/project/domoszlai/bezier2biarc)

## Synopsis

Simple C# application to illustrate the algorithm of approximating a cubic bezier curve with biarcs.
The algorithm is explained in the blog post at http://dlacko.org/blog/2016/10/19/approximating-bezier-curves-by-biarcs/.

## Implementation

This is a new implementation based on dotnet [MAUI](https://learn.microsoft.com/en-us/dotnet/maui/what-is-maui?view=net-maui-7.0)
as the original Windows.Form implementation put an unacceptable hard requirement on Windows. You can still find the original one on the `winforms` branch.

The algorithm is also be available in Haskell, incorporated into my [JuicyGcode](https://github.com/domoszlai/svg2gcode) project.

## Installation

- Install community edition of Visual Studio 2022 (at least 17.4) or the latest Visual Studio for Mac
- Install .NET 7.0 SDK
- Check out the repository and run `dotnet workload restore`
- Open BiArcTutorial.csproj and pray
