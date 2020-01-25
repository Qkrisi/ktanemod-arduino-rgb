using System.Collections.Generic;
using GameObject = UnityEngine.GameObject;

public class RedArrows
{
    public List<int> arduinoRGBValues = new List<int>();

    public RedArrows(GameObject Module)
    {
        arduinoRGBValues = new List<int>() { 255, 0, 0 };
    }
}

