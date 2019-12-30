using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using rgbMod;

public class arduinoService : MonoBehaviour
{
    public class Settings
    {
        public int redPin = 3;
        public int greenPin = 4;
        public int bluePin = 5;
        public int baudRate = 9600;
        public bool enableModuleImplementation = true;
    }

    public Settings ModSettings;

    public Arduino arduinoConnection = new Arduino();
    public KMGameInfo gameInfo;

    private KMGameInfo.State currentState;

    [HideInInspector]
    public int RP;
    [HideInInspector]
    public int GP;
    [HideInInspector]
    public int BP;
    
    private List<int> currentValues = new List<int>() { 0, 0, 0 };
    private List<int> previousValues = new List<int>() { 0, 0, 0 };

    public int Baud;

    private bool implementationEnabled;

    void Start()
    {
        gameInfo.OnStateChange += OnStateChange;
        setPins();
    }

    public void setPins()
    {
        GetComponent<KMModSettings>().RefreshSettings();
        ModSettings = JsonConvert.DeserializeObject<Settings>(GetComponent<KMModSettings>().Settings);
        List<int> Pins = getPins();
        RP = Pins[0];
        GP = Pins[1];
        BP = Pins[2];
        Baud = this.ModSettings.baudRate;
        implementationEnabled = this.ModSettings.enableModuleImplementation;
        return;
    }

    private void OnStateChange(KMGameInfo.State state)
    { 
        currentState = state;
        setPins();
        StartCoroutine(getField());
    }

    private IEnumerator getField()
    {
        yield return null;
        setPins();
        while (currentState == KMGameInfo.State.Gameplay && implementationEnabled)
        {
            yield return null;
            //Get the field here
            if(currentValues!=previousValues)
            {
                previousValues = currentValues;
                arduinoConnection.sendMSG(String.Format("{0} {1} {2} {3} {4} {5}", RP, GP, BP, currentValues[0], currentValues[1], currentValues[2]));
            }
        }
        currentValues = new List<int>() { 0, 0, 0};
        previousValues = new List<int>() {0, 0, 0};
        yield break;
    }

    private List<int> getPins()
    {
        return new List<int>() { this.ModSettings.redPin, this.ModSettings.greenPin, this.ModSettings.bluePin };
    }
}

