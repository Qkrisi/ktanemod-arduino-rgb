using System.Collections.Generic;
using UnityEngine;

public class BlueArrows
{
    public List<int> arduinoRGBValues = new List<int>();

    public BlueArrows(GameObject Module)
    {
        arduinoRGBValues = new List<int>() { 0, 0, 255 };
    }
}

