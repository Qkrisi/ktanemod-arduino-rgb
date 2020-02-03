using System.Collections.Generic;
using UnityEngine;

public class GreenArrows
{
    public List<int> arduinoRGBValues = new List<int>();

    public GreenArrows(GameObject Module)
    {
        arduinoRGBValues = new List<int>() { 0, 255, 0 };
    }
}

