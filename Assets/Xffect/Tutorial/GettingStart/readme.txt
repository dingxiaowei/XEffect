First of all, you need to add this line to your script:
using Xft;

*************************Release Your Effect****************************
To release your effect, you can:
1, Instantiate the effect by GameObject.Instantiate() method. This way is not recommended since the new effect is not reuseable.
2, Use 'XffectCache' to pool the effect.

Note: Just drug the effect that you want to be cached to a XffectCache object as its child, then you can get the effect 
by XffectCache.GetEffect(string name) and the returned effect will be pooled automatically.

*************************Stop Your Effect****************************
Every non-looped xffect object will become non-active when it is finished. To stop a looped xffect, you may need to call these API:
1, DeActive()
this method will stop the xffect immediately.
2, StopSmoothly(float fadeTime)
this method will stop the xffect softly.

To check if your xffect is finished, you can either check the xffect object's activities or check the 'IsPlaying' variable of XffectComponent.

To Reset your xffect, invoke Active() method.
