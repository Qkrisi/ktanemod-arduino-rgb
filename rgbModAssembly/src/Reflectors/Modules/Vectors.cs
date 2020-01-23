using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Vectors
{
    public List<int> arduinoRGBValues = new List<int>() { 0, 0, 0 };
    private Text displayText;

    public Vectors(GameObject module)
    {
        if(module!=null) displayText = module.transform.Find("displays").Find("Canvas (1)").Find("disp").GetComponent<Text>();
    }

    public void Update()
    {
        string text = displayText.text.ToUpperInvariant().Replace("VECTOR 1 (", "").Replace("VECTOR 2 (", "").Replace("VECTOR 3 (", "");
        Debug.Log(text);
        if (text.StartsWith("RED", StringComparison.InvariantCultureIgnoreCase)) arduinoRGBValues = new List<int>() { 255, 0, 0 };
        else if (text.StartsWith("ORANGE", StringComparison.InvariantCultureIgnoreCase)) arduinoRGBValues = new List<int>() { 184, 165, 0 };
        else if (text.StartsWith("YELLOW", StringComparison.InvariantCultureIgnoreCase)) arduinoRGBValues = new List<int>() { 255, 255, 0 };
        else if (text.StartsWith("GREEN", StringComparison.InvariantCultureIgnoreCase)) arduinoRGBValues = new List<int>() { 0, 255, 0 };
        else if (text.StartsWith("BLUE", StringComparison.InvariantCultureIgnoreCase)) arduinoRGBValues = new List<int>() { 0, 0, 255 };
        else if (text.StartsWith("PURPLE", StringComparison.InvariantCultureIgnoreCase)) arduinoRGBValues = new List<int>() { 128, 0, 128 };
        else { arduinoRGBValues = new List<int>() { 0, 0, 0 }; }
        return;
    }
}

