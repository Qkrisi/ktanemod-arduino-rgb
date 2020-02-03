using System.Collections.Generic;
using UnityEngine;

public class YellowArrows
{
    public List<int> arduinoRGBValues = new List<int>();

    public YellowArrows(GameObject Module)
    {
        arduinoRGBValues = new List<int>() { 255, 255, 0 };
    }
}

