using UnityEngine;
using System.Collections;

namespace Xft
{
    public class XftTools
    {
        static public void TopLeftUVToLowerLeft(ref Vector2 tl, ref Vector2 dimension)
        {
            tl.y = 1f - tl.y;
            dimension.y = -dimension.y;
        }
    }
}

