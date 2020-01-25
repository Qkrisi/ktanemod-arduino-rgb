using System.Collections.Generic;
using UnityEngine;

public class SimonSays
{
    public List<int> arduinoRGBValues = new List<int>() { 0, 0, 0 };

    private List<Material> Buttons = new List<Material>();

    private SimonButton[] btnsValue = new SimonButton[] { };

    private List<Texture> Textures = new List<Texture>();

    private static readonly string[] colorOrder = new[] { "Red", "Blue", "Green", "Yellow" };

    public SimonSays(GameObject Module)
    {
        btnsValue = Module.GetComponent<SimonComponent>().buttons;
        foreach(SimonButton button in btnsValue)
        {
            Buttons.Add(button.ButtonGO.GetComponent<Renderer>().material);
            Textures.Add(button.ButtonGO.GetComponent<Renderer>().material.mainTexture);
        }
    }

    public void Update()
    {
        if (Buttons[0].mainTexture != Textures[0]) arduinoRGBValues = new List<int>() { 255, 0, 0 };
        else if (Buttons[1].mainTexture != Textures[1]) arduinoRGBValues = new List<int>() { 0, 0, 255 };
        else if (Buttons[2].mainTexture != Textures[2]) arduinoRGBValues = new List<int>() { 0, 255, 0 };
        else if (Buttons[3].mainTexture != Textures[3]) arduinoRGBValues = new List<int>() { 255, 255, 0 };
        else { arduinoRGBValues = new List<int>() { 0, 0, 0 }; }
        return;
    }
}