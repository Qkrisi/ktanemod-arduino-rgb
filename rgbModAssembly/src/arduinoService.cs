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
    private KMGameInfo gameInfo;

    private KMGameInfo.State currentState;

    [HideInInspector]
    public int RP;
    [HideInInspector]
    public int GP;
    [HideInInspector]
    public int BP;
    
    private List<int> currentValues = new List<int>() { 0, 0, 0 };
    private List<int> previousValues = new List<int>() { 0, 0, 0 };

    [HideInInspector]
    public int Baud;

    [HideInInspector]
    public bool implementationEnabled;

    private KMBombInfo bombInfo;

    private int lastStrikes = 0;
    private int lastSolves = 0;

    #pragma warning disable 414
    private int bombState = 0; //0 means runnig, 1 means exploded, 2 means defused
    #pragma warning restore 414
    void Start()
    {
        gameInfo = this.GetComponent<KMGameInfo>();
        gameInfo.OnStateChange += OnStateChange;
        bombInfo = this.GetComponent<KMBombInfo>();
        bombInfo.OnBombExploded += OnBombExplodes;
        bombInfo.OnBombSolved += OnBombDefused;
        setPins();
        //StartCoroutine(checkBombState());
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
        if (currentState != KMGameInfo.State.PostGame) { arduinoConnection.Stop(); }
        if (currentState == KMGameInfo.State.Gameplay) 
        {
            bombState = 0;
            lastStrikes = 0;
            lastSolves = 0;
            StartCoroutine(getField());
            StartCoroutine(Warning());
            StartCoroutine(OnStrike());
            StartCoroutine(OnSolve());
        }
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

    /*private IEnumerator checkBombState()
    {
        while (true)
        {
            yield return null;
            if (currentState == KMGameInfo.State.PostGame)
            {
                switch (bombState)
                {
                    case 1:
                        arduinoConnection.sendMSG(String.Format("{0} {1} {2} 255 0 0", RP, GP, BP));
                        break;
                    case 2:
                        arduinoConnection.sendMSG(String.Format("{0} {1} {2} 0 0 255", RP, GP, BP));
                        break;
                    default:
                        arduinoConnection.Stop();
                        break;
                }
            }
        }
    }*/

    #region BombEvents
    private void OnBombExplodes()
    {
        bombState = 1;
        arduinoConnection.sendMSG(String.Format("{0} {1} {2} 255 0 0", RP, GP, BP));
    }

    private void OnBombDefused()
    {
        bombState = 2;
        arduinoConnection.sendMSG(String.Format("{0} {1} {2} 0 0 255", RP, GP, BP));
    }

    private IEnumerator Warning()
    {
        yield return null;
        while(currentState==KMGameInfo.State.Gameplay && !implementationEnabled)
        {
            yield return null;
            if (bombInfo.GetTime() < 60 && bombInfo.GetSolvedModuleNames().Count<bombInfo.GetSolvableModuleNames().Count)
            {
                yield return null;
                yield return new WaitForSeconds(1.5f);
                arduinoConnection.sendMSG(String.Format("{0} {1} {2} 200 0 0", RP, GP, BP));
                yield return new WaitForSeconds(1.5f);
                if (currentState == KMGameInfo.State.Gameplay) { arduinoConnection.Stop(); }
                if (bombInfo.GetSolvedModuleNames().Count >= bombInfo.GetSolvableModuleNames().Count)
                {
                    OnBombDefused();
                }
            }
            else if(bombInfo.GetSolvedModuleNames().Count >= bombInfo.GetSolvableModuleNames().Count)
            {
                OnBombDefused();
            }
        }
        yield break;
    }

    private IEnumerator OnStrike()
    {
        yield return null;
        while (currentState == KMGameInfo.State.Gameplay)
        {
            yield return null;
            if(bombInfo.GetStrikes() > lastStrikes && (bombInfo.GetTime()>=60 || implementationEnabled))
            {
                yield return null;
                lastStrikes = bombInfo.GetStrikes();
                arduinoConnection.sendMSG(String.Format("{0} {1} {2} 255 0 0", RP, GP, BP));
                yield return new WaitForSeconds(1.5f);
                if (currentState == KMGameInfo.State.Gameplay) { arduinoConnection.Stop(); }
            }
        }
        yield break;
    }

    private IEnumerator OnSolve()
    {
        yield return null;
        while (currentState == KMGameInfo.State.Gameplay)
        {
            yield return null;
            if (bombInfo.GetSolvedModuleNames().Count > lastSolves && bombInfo.GetSolvedModuleNames().Count < bombInfo.GetSolvableModuleNames().Count)
            {
                yield return null;
                lastSolves = bombInfo.GetSolvedModuleNames().Count;
                arduinoConnection.sendMSG(String.Format("{0} {1} {2} 0 255 0", RP, GP, BP));
                yield return new WaitForSeconds(1.5f);
                if (currentState == KMGameInfo.State.Gameplay) { arduinoConnection.Stop(); }
            }
        }
        yield break;
    }
    #endregion

    private List<int> getPins()
    {
        return new List<int>() { this.ModSettings.redPin, this.ModSettings.greenPin, this.ModSettings.bluePin };
    }
}

