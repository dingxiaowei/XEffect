After version 4.1.0, you can control the camera shake by curve now.

And the shake force will be added relative to the camera transform now, which means this event will no longer conflict with
other camera component any more.

If you are using the shake by curve type, there aer something you should be aware of:
1, Make sure the curve's start value and end value is 0.5f(the 0.5f will be mapped to zero at run time).
2, The curve can be saved to Unity's preset library(above unity 4.1?) so that you can reuse any of the them.(http://docs.unity3d.com/Documentation/Manual/PresetLibraries.html)