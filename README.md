Bezier curve approximation algorithm with biarcs
==================================

![Appveyor](https://ci.appveyor.com/api/projects/status/github/domoszlai/bezier2biarc?branch=master&svg=true)

## Synopsis

Simple C# application to illustrate the algorithm of approximating a cubic bezier curve with biarcs.
The algorithm will also be available in Haskell, incorporated into my [JuicyGcode](https://github.com/domoszlai/svg2gcode) project.

## Implementation

The algorithm will be explained in the blog post at [http://dlacko.blogspot.nl/2016/10/approximating-bezier-curves-by-biarcs.html](http://dlacko.blogspot.nl/2016/10/approximating-bezier-curves-by-biarcs.html)

## Usage

The entry point of the application is the 
“`OnPaint`” method in “`Form.cs`”. You can set the bezier curve to be approximated there (first line) and you can also change the approximation 
parameters at the “`Algorithm.ApproxCubicBezier`” call (second line).

The application draws the original bezier curve (black color), the approximation biarcs (red color), and the full circles the arcs lie on (green color).

## Installation

Use the community edition of Visual Studio 2015 to run it.

