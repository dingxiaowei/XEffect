----------------------------------------------
            Xffect Editor Pro Edition
 Copyright 2012- Shallway Studio
                Version 4.4.1
            shallwaycn@gmail.com
----------------------------------------------

Thank you for buying Xffect Pro!

If you have any questions, suggestions, comments or feature requests, please
drop by the Xffect forum, found here: http://shallway.net/xffect/forum/

---------------------------------------
News, tutorials and supports
---------------------------------------
http://shallway.net/xffect/doku.php?id=en:main


--------------------
 How To Update Xffect
--------------------
1. In Unity, File -> New Scene
2. Delete the Xffect folder from the Project View.
3. Import Xffect from the updated Unity Package.


--------------------
Release Notes
--------------------
Version 4.4.1
-FIX: re-enabled updating in editor for 'camera shake' and 'camera effect' event.
-FIX: adjusted the width of the color gradient field in inspector, now all the inspector label width are uniform.

Version 4.4.0
-FIX: The sprites in the scene view will face the editor camera now.
-FIX: You can set another camera to XffectComponent now: myXffect.MyCamera = anotherCamera.
-NEW: Obsoleted the color gradient editor and Added a new one, it's the same as shuriken's color gradient editor.

Version 4.3.2
-FIX: Camera rendering upside down problem completely solved, thanks for gtotoy's solution.
-NEW: Multi EffectLayer editing supported(still in beta, be careful to use it. If you messed up your effectlayers, use ctrl+z to revert them).

Version 4.3.1
-FIX: re-enabled the auto activate feature in the Start() method of xffect obj.

Version 4.3.0
*-FIX: EffectLayer editor performance has been improved, is's super fast now and no jerky when update it in editor.
-FIX: Fixed a very old problem that if you have any xffect obj in the scene, there is always a "*" in the unity title bar which means the scene needs to re-save. Many thanks to Arcanor's feedback, now this issue has been solved and the xffect editor is more stable.
-FIX: Effect Visibility check method fixed, now uncheck the "update when offscreen" option should work properly.
-NEW: Added a new effect: "energy_shield.prefab" in "EffectLibrary/Other" directory.
-NEW: Added a new option "start offset" in UV Scroll config.
-NEW: Added a new UV change type: "uv scale" in UV Config. You can use it to change the uv dimensions dynamically.

Versin 4.2.1
-FIX: The "Cone" and "CustomMesh"'s direction can inherit the client's rotation now.
-FIX: API "SetColor()" should work peroperly now.
-NEW: Added a new effect: "boost wind" to "EffectLibrary/Mobile" directory.
-NEW: Added option : "sync with client" in RibbonTrail, you can use this option to synchronize all the trail's nodes with the client's position.
(The protect_ring.prefab has also been improved with this option.)
-IMPROVED: Improved the performance of "Glow" and "GlowPerObj" camera effect according to this warning:http://forum.unity3d.com/threads/191906-4-2-Any-way-to-turn-off-the-quot-Tiled-GPU-perf-warning-quot/page2


Version 4.2.0
-NEW: Added a new render type: 'Spherical Billboard', you can use it to make realistic explosions and fogs, check 'Tutorial/SphericalBillboard' folder to learn more.
-NEW: Added 'Playback time' option in the 'update in editor' control window.
-FIX: Fixed some camera shake bugs.
-FIX: Fixed a bug that the camera effect events can't be removed correctly since version 4.1.0.


Version 4.1.5
-NEW: Added a button "Put to Scene" in Xffect prefabs' inspector, you can use this button to preview xffect prefab easily.
-FIX: Fixed that while updating in editor at EffectLayer the effect becomes very jerky.
-DEL: Obsolete parameter "emit loop count" and "delay after each loop" in "Emitter Config" since they are useless but bring confusions.

Version 4.1.4
-FIX: Fixed a bug that the xffect will become jerky when the 'Time.timeScale' is very small.
-NEW: Added option 'Max Fps' in XffectComponent, you can use this option to limit xffect updates in a certain fps.

Version 4.1.3
-NEW: Added option "use with 2d sprite" in XffectComponent, you can use it to integrate xffect with 2d sprite in unity 4.3 now.
-FIX: Fixed a bug that the Glow Per Obj is rendering upside down in version above 4.1.0.
-FIX: Fixed a bug that the Direction is not inherited from client correctly. 


Version 4.1.2
-NEW: Added option "uniform random start scale" in Scale Config.
-NEW: Added a control window for EffectLayer to update the parent Xffect in Editor.
-FIX: The default scale curve is zoom out to the extends of the graph now.


Version 4.1.0
-NEW: Compatible with unity 4.3.
-NEW: Added option "update when offscreen" in XffectComponent;
-NEW: Added a control window in the bottom left corner of the scene view.
-NEW: You can control the camera shake event by curve now.
-IMPROVED£º The camera effect code has been rewritten, all the camera components are combined into one component, and you can assign a priority to each camera effect now.
-FIXED: The Camera shake force will be added relative to the camera transform now, which means this event will no longer conflict with other camera component any more.
-FIXED£ºFixed some bugs that 'Glow Per Obj' shader and 'displacement-dissolve' shader are not working properly in some cases.
-FIXED: Fixed a bug that the 'Glow Per Obj' shader will make the skybox glow..
-FIXED: Fixed a bug that the camera will not be assigned correctly if you have multiply camera in the scene.

Version 4.0.0
-Documentations and tutorials are now included in this package, please check it out! 
-This version has many changes, please check UPGRADE_NOTES.TXT before updating!

