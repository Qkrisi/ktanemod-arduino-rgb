using System.Collections.Generic;
using UnityEngine;

public class MorseCode
{
    public List<int> arduinoRGBValues = new List<int>() { 0, 0, 0 };

    private GameObject LitObject;

    public MorseCode(GameObject Module)
    {
        LitObject = Module.GetComponent<MorseCodeComponent>().LEDLit;
    }

    public void Update()
    {
        if (LitObject.activeSelf)
        {
            arduinoRGBValues = new List<int>() { 255, 255, 0 };
        }
        else
        {
            arduinoRGBValues = new List<int>() { 0, 0, 0 };
        }
        return;
    }
}