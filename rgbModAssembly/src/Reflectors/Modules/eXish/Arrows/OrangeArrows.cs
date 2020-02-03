using System.Collections.Generic;
using UnityEngine;

public class OrangeArrows
{
    public List<int> arduinoRGBValues = new List<int>();

    public OrangeArrows(GameObject Module)
    {
        arduinoRGBValues = new List<int>() { 255, 150, 0 };
    }
}

