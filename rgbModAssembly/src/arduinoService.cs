using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;
using rgbMod;

public class arduinoService : MonoBehaviour
{
    class Settings
    {
        public int redPin = 0;
        public int greenPin = 0;
        public int bluePin = 0;
    }
    Settings modSettings;

    public Arduino arduinoConnection = new Arduino();
    public KMGameInfo gameInfo;
    public KMModSettings ModSettings;

    private KMGameInfo.State currentState;

    public int RP;
    public int GP;
    public int BP;

    private List<int> currentValues = new List<int>() { 0, 0, 0 };
    private List<int> previousValues = new List<int>() { 0, 0, 0 };


    void Start()
    {
        gameInfo.OnStateChange += OnStateChange;
        setPins();
    }

    private void setPins()
    {
        List<int> Pins = getPins();
        RP = Pins[0];
        GP = Pins[1];
        BP = Pins[2];
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
        while (currentState == KMGameInfo.State.Gameplay)
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
        try
        {
            ModSettings.RefreshSettings();
            modSettings = JsonConvert.DeserializeObject<Settings>(ModSettings.Settings);
            if (!(modSettings.redPin>0) || !(modSettings.greenPin > 0) || !(modSettings.bluePin > 0))
            {
                Debug.Log("[Arduino Manager] Settings Path: "+ModSettings.SettingsPath); // check settings path
                try { File.WriteAllText(ModSettings.SettingsPath, JsonConvert.SerializeObject(modSettings, Formatting.Indented)); }
                catch { Debug.Log("[Arduino Manager] Some writing error happened"); }
                return new List<int>() { 3, 4, 5 };
            }
            if (modSettings != null)
            {
                try
                {
                    return new List<int>() { modSettings.redPin, modSettings.greenPin, modSettings.bluePin };
                }
                catch
                {
                    return new List<int>() { 3, 4, 5 };
                }
            }
            return new List<int>() { 3, 4, 5 };

        }
        catch
        {
            return new List<int>() { 3, 4, 5 };
        }

    }
}

