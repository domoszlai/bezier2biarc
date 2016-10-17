## Synopsis

Simple C# application to illustrate the algorithm of approximating a cubic bezier curve with biarcs.
The algorithm will also be available in Haskell, incorporated into my [JuicyGcode](https://github.com/domoszlai/svg2gcode) project.

## Implementation

The algorithm will be explained in a future blog post at dlacko.blogspot.com.

## Usage

The entry point of the application is the 
“`OnPaint`” method in “`Form.cs`”. You can set the bezier curve to be approximated there (first line) and you can also change the approximation 
parameters at the “`Algorithm.ApproxCubicBezier`” call (second line).

The application draws the original bezier curve (black color), the approximation biarcs (red color), and the full circles the arcs lies on (green color).

## Installation

Use the community edition of Visual Studio 2015 to run it.

