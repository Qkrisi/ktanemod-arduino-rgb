using System.Collections.Generic;

namespace rgbMod
{
    public static class Reflector
    {
        public static Dictionary<string, object> moduleReflectors = new Dictionary<string, object>()
        {
            {"Red Arrows", new RedArrows(null)},
            {"Vectors", new Vectors(null)}
        };
    }
}
