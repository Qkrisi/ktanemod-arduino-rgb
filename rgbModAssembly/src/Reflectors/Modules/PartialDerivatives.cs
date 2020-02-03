using System.Collections.Generic;
using UnityEngine;

public class PartialDerivatives
{
    public List<int> arduinoRGBValues = new List<int>() { 0, 0, 0 };

    private GameObject MainLed;

    private Dictionary<string, List<int>> colorValues = new Dictionary<string, List<int>>()
    {
        {"Blue", new List<int>(){0,0,255 } },
        {"Green", new List<int>(){0,255,0 } },
        {"Orange", new List<int>(){255,150,0 } },
        {"Purple", new List<int>(){138,23,182 } },
        {"Red", new List<int>(){255,0,0 } },
        {"Yellow", new List<int>(){255,255,0 } },
    };

    public PartialDerivatives(GameObject Module)
    {
        MainLed = Module.transform.Find("MainLED").gameObject;
    }

    public void Update()
    {
        arduinoRGBValues = colorValues[MainLed.GetComponent<Renderer>().material.name.Replace(" (Instance)", "")];
    }
}