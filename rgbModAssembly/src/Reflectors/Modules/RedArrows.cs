using System.Collections.Generic;
using GameObject = UnityEngine.GameObject;

public class RedArrows
{
    public List<int> arduinoRGBValues = new List<int>();

    public RedArrows(GameObject module)
    {
        arduinoRGBValues = new List<int>() { 255, 0, 0 };
    }

    public void Update()
    {
        return;
    }
}

