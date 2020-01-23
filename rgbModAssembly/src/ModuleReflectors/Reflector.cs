using System.Collections.Generic;
using GameObject = UnityEngine.GameObject;

namespace rgbMod
{
    public static class Reflector
    {
        public static Dictionary<string, object> moduleReflectors = new Dictionary<string, object>()
        {
            { "Red Arrows", new RedArrows(new GameObject()) }
        };
    }
}
