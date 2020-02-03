using System.Collections.Generic;
using UnityEngine;

public class Egg
{
    public List<int> arduinoRGBValues = new List<int>();

    public Egg(GameObject Module)
    {
        arduinoRGBValues = new List<int>() { 255, 255, 255 };
    }
}