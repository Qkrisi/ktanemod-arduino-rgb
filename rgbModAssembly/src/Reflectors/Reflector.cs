using System;
using System.Collections.Generic;

namespace rgbMod
{
    public static class Reflector
    {
        public static Dictionary<string, Type> moduleReflectors = new Dictionary<string, Type>()
        {
            {"Morse Code", typeof(MorseCode)},
            {"Red Arrows", typeof(RedArrows)},
            {"Simon Says", typeof(SimonSays)},
            {"Vectors", typeof(Vectors)}
        };
    }
}
