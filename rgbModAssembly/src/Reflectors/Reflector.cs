using System;
using System.Collections.Generic;

namespace rgbMod
{
    public static class Reflector
    {
        public static Dictionary<string, Type> moduleReflectors = new Dictionary<string, Type>()
        {
            {"Blue Arrows", typeof(BlueArrows)},
            {"egg", typeof(Egg)},
            {"Green Arrows", typeof(GreenArrows)},
            {"Morse Code", typeof(MorseCode)},
            {"Orange Arrows", typeof(OrangeArrows)},
            {"Partial Derivatives", typeof(PartialDerivatives)},
            {"Purple Arrows", typeof(PurpleArrows)},
            {"Red Arrows", typeof(RedArrows)},
            {"Simon Says", typeof(SimonSays)},
            {"Vectors", typeof(Vectors)},
            {"Yellow Arrows", typeof(YellowArrows)}
        };
    }
}
