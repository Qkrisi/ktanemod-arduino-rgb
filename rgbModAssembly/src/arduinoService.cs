using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;
using Assets.Scripts.Missions;
using Module = rgbMod.Module;
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

    [HideInInspector]
    public KMGameInfo.State currentState;

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

    #pragma warning disable 414
    private int lastStrikes = 0;
    private int lastSolves = 0;
    #pragma warning restore 414

    [HideInInspector]
    public string currentModuleName = "";

    private bool ableToDisplay = true;

    private Dictionary<BombCommander, int> strikeCounts = new Dictionary<BombCommander, int>();
    private Dictionary<BombCommander, int> solveCounts = new Dictionary<BombCommander, int>();

    #pragma warning disable 414
    private int currentStrikes = 0;
    #pragma warning restore 414

    private BombCommander currentBomb;

    #pragma warning disable 414
    private bool Infinite = false;
    #pragma warning restore 414

    #pragma warning disable 414
    private int bombState = 0; //0 means runnig, 1 means exploded, 2 means defused
    #pragma warning restore 414

    private bool wait = false;
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
        else
        {
            if (bombState == 0) { StopCoroutines(); arduinoConnection.sendMSG(String.Format("{0} {1} {2} 0 0 255", RP, GP, BP)); } //For multiple bombs
        }
        if (currentState == KMGameInfo.State.Gameplay) 
        {
            bombState = 0;
            lastStrikes = 0;
            lastSolves = 0;
            strikeCounts.Clear();
            solveCounts.Clear();
            StartCoroutine(getField());
            StartCoroutine(Warning());
            StartCoroutine(OnStrike());
            StartCoroutine(OnSolve());
            StartCoroutine(CheckForBomb());
            StartCoroutine(FactoryCheck());
            StartCoroutine(getBomb());
        }
        else {
            currentModuleName = "";
            Modules.Clear();
            StopCoroutine(CheckForBomb());
            StopCoroutine(FactoryCheck());
            StopCoroutine(WaitUntilEndFactory());
            BombActive = false;
            Bombs.Clear();
            BombCommanders.Clear();
        }
        if (currentState == KMGameInfo.State.Quitting) { arduinoConnection.Disconnect(); }
    }

    private IEnumerator getField()
    {
        yield return null;
        setPins();
        while (currentState == KMGameInfo.State.Gameplay && implementationEnabled)
        {
            yield return null;
            //Debug.Log("Getting field...");
            Module heldModule = null;
            if(BombActive)
            {
                //Debug.Log("Bomb active!");
                heldModule=GetFocusedModule();
            }
            if(heldModule!=null && heldModule.IsSolved)
            {
                //Debug.Log("Solved module selected: displaying green...");
                while(heldModule!=null && heldModule.IsSolved)
                {
                    //Debug.Log("Updating module");
                    yield return null;
                    if (ableToDisplay) { ableToDisplay = false; arduinoConnection.sendMSG(String.Format("{0} {1} {2} 0 255 0", RP, GP, BP)); }
                    heldModule=GetFocusedModule();
                }
                arduinoConnection.Stop();
                ableToDisplay = true;
            }
            else if(currentValues!=previousValues)
            {
                //Debug.Log("Values checked");
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

    private void StopCoroutines()
    {
        StopCoroutine(getField());
        StopCoroutine(Warning());
        StopCoroutine(OnStrike());
        StopCoroutine(OnSolve());
        StopCoroutine(getBomb());
        ableToDisplay = true;
        return;
    }

    #region BombEvents
    private void OnBombExplodes()
    {
        bombState = 1;
        StopCoroutines();
        arduinoConnection.sendMSG(String.Format("{0} {1} {2} 255 0 0", RP, GP, BP));
    }

    private void OnBombDefused()
    {
        bombState = 2;
        StopCoroutines();
        arduinoConnection.sendMSG(String.Format("{0} {1} {2} 0 0 255", RP, GP, BP));
    }

    private IEnumerator Warning()
    {
        yield return null;
        while(currentState==KMGameInfo.State.Gameplay && !implementationEnabled)
        {
            yield return null;
            if (currentBomb!=null && currentBomb.CurrentTimer < 60 && currentBomb.SolveCount<currentBomb.SolvableCount)
            {
                yield return null;
                yield return new WaitForSeconds(1.5f);
                if(ableToDisplay) arduinoConnection.sendMSG(String.Format("{0} {1} {2} 200 0 0", RP, GP, BP));
                yield return new WaitForSeconds(1.5f);
                if (currentState == KMGameInfo.State.Gameplay) { arduinoConnection.Stop(); }
                if (currentBomb!=null && currentBomb.SolveCount >= currentBomb.SolvableCount)
                {
                    OnBombDefused();
                }
            }
            else if(currentBomb!=null && currentBomb.SolveCount >= currentBomb.SolvableCount)
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
            /*if(bombInfo.GetStrikes() > lastStrikes && (bombInfo.GetTime()>=60 || implementationEnabled))
            {
                yield return null;
                lastStrikes = bombInfo.GetStrikes();
                if(ableToDisplay) arduinoConnection.sendMSG(String.Format("{0} {1} {2} 255 0 0", RP, GP, BP));
                yield return new WaitForSeconds(1.5f);
                if (currentState == KMGameInfo.State.Gameplay && ableToDisplay) { arduinoConnection.Stop(); }
            }*/
            if (!wait && currentBomb!=null && currentBomb.StrikeCount > strikeCounts[currentBomb] && (currentBomb.CurrentTimer >= 60 || implementationEnabled))
            {
                yield return null;
                strikeCounts[currentBomb] = currentBomb.StrikeCount;
                if (ableToDisplay) arduinoConnection.sendMSG(String.Format("{0} {1} {2} 255 0 0", RP, GP, BP));
                yield return new WaitForSeconds(1.5f);
                if (currentState == KMGameInfo.State.Gameplay && ableToDisplay) { arduinoConnection.Stop(); }
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
            /*if (bombInfo.GetSolvedModuleNames().Count > lastSolves && bombInfo.GetSolvedModuleNames().Count < bombInfo.GetSolvableModuleNames().Count)
            {
                yield return null;
                lastSolves = bombInfo.GetSolvedModuleNames().Count;
                if(ableToDisplay) arduinoConnection.sendMSG(String.Format("{0} {1} {2} 0 255 0", RP, GP, BP));
                yield return new WaitForSeconds(1.5f);
                if (currentState == KMGameInfo.State.Gameplay && ableToDisplay) { arduinoConnection.Stop(); }
            }*/
            if (!wait && currentBomb!=null && currentBomb.SolveCount > solveCounts[currentBomb] && currentBomb.SolveCount < currentBomb.SolvableCount)
            {
                yield return null;
                solveCounts[currentBomb] = currentBomb.SolveCount;
                if (ableToDisplay) arduinoConnection.sendMSG(String.Format("{0} {1} {2} 0 255 0", RP, GP, BP));
                yield return new WaitForSeconds(1.5f);
                if (currentState == KMGameInfo.State.Gameplay && ableToDisplay) { arduinoConnection.Stop(); }
            }
        }
        yield break;
    }
    #endregion

    private List<int> getPins()
    {
        return new List<int>() { this.ModSettings.redPin, this.ModSettings.greenPin, this.ModSettings.bluePin };
    }




    public bool BombActive = false;
    public List<Module> Modules = new List<Module> { };
    public List<Bomb> Bombs = new List<Bomb> { };
    public List<BombCommander> BombCommanders = new List<BombCommander> { };

    public Module GetFocusedModule()
    {
        Module focused = null;
        //Debug.Log("finding focused module");
        foreach (Module module in Modules)
        {
            //Debug.LogFormat("Checking module {0}...", module.ModuleName);
            if (module.IsHeld()) {
                focused = module;
                //Debug.Log("focused module found");
            }
        }
        //if(focused==null) Debug.Log("Couldn't find focused module. :(");
        return focused;
    }

    private BombCommander GetHeldBomb()
    {
        BombCommander held = null;
        foreach (BombCommander commander in BombCommanders)
        {
            if (commander.IsHeld())
                held = commander;
        }
        return held;
    }




    private IEnumerator CheckForBomb()
    {
        yield return new WaitUntil(() => SceneManager.Instance.GameplayState.Bombs != null && SceneManager.Instance.GameplayState.Bombs.Count > 0);
        yield return new WaitForSeconds(2.0f);
        Bombs.AddRange(SceneManager.Instance.GameplayState.Bombs);
        int i = 0;
        string[] keyModules =
        {
            "SouvenirModule", "MemoryV2", "TurnTheKey", "TurnTheKeyAdvanced", "theSwan", "HexiEvilFMN", "taxReturns"
        };
        foreach (Bomb bomb in Bombs)
        {
            BombCommanders.Add(new BombCommander(bomb, i));
            foreach (BombComponent bombComponent in bomb.BombComponents)
            {
                ComponentTypeEnum componentType = bombComponent.ComponentType;
                bool keyModule = false;
                string moduleName = string.Empty;

                switch (componentType)
                {
                    case ComponentTypeEnum.Empty:
                    case ComponentTypeEnum.Timer:
                        continue;

                    case ComponentTypeEnum.NeedyCapacitor:
                    case ComponentTypeEnum.NeedyKnob:
                    case ComponentTypeEnum.NeedyVentGas:
                    case ComponentTypeEnum.NeedyMod:
                        moduleName = bombComponent.GetModuleDisplayName();
                        keyModule = true;
                        break;

                    case ComponentTypeEnum.Mod:
                        KMBombModule KMModule = bombComponent.GetComponent<KMBombModule>();
                        keyModule = keyModules.Contains(KMModule.ModuleType);
                        goto default;

                    default:
                        moduleName = bombComponent.GetModuleDisplayName();
                        break;
                }
                Module module = new Module(bombComponent, i)
                {
                    ComponentType = componentType,
                    IsKeyModule = keyModule,
                    ModuleName = moduleName
                };

                Modules.Add(module);
            }
            i++;
        }
        strikeCounts.Clear();
        solveCounts.Clear();
        foreach (BombCommander commander in BombCommanders)
        {
            strikeCounts.Add(commander, commander.StrikeCount);
            solveCounts.Add(commander, commander.SolveCount);
        }
        BombActive = true;
        arduinoConnection.Stop();
        wait = false;
    }

    void Update()
    {
        if(BombActive)
        {
            foreach (Module module in Modules)
			{
				module.Update();
			}
        }
    }

    private IEnumerator getBomb()
    {
        yield return null;
        while (currentState==KMGameInfo.State.Gameplay)
        {   
            yield return null;
            if (BombActive)
            {
                BombCommander current = GetHeldBomb();
                if(current!=null) currentBomb = current;
            }
        }
        yield break;
    }

    #region Factory Implementation
    private IEnumerator FactoryCheck()
    {
        yield return new WaitUntil(() => SceneManager.Instance.GameplayState.Bombs != null && SceneManager.Instance.GameplayState.Bombs.Count > 0);
        GameObject _gameObject = null;
        for (var i = 0; i < 4 && _gameObject == null; i++)
        {
            _gameObject = GameObject.Find("Factory_Info");
            yield return null;
        }

        if (_gameObject == null) yield break;

        _factoryType = ReflectionHelper.FindType("FactoryAssembly.FactoryRoom");
        if (_factoryType == null) yield break;

        _factoryBombType = ReflectionHelper.FindType("FactoryAssembly.FactoryBomb");
        _internalBombProperty = _factoryBombType.GetProperty("InternalBomb", BindingFlags.NonPublic | BindingFlags.Instance);

        _factoryStaticModeType = ReflectionHelper.FindType("FactoryAssembly.StaticMode");
        _factoryFiniteModeType = ReflectionHelper.FindType("FactoryAssembly.FiniteSequenceMode");
        _factoryInfiniteModeType = ReflectionHelper.FindType("FactoryAssembly.InfiniteSequenceMode");
        _currentBombField = _factoryFiniteModeType.GetField("_currentBomb", BindingFlags.NonPublic | BindingFlags.Instance);

        _gameModeProperty = _factoryType.GetProperty("GameMode", BindingFlags.NonPublic | BindingFlags.Instance);

        List<UnityEngine.Object> factoryObject = FindObjectsOfType(_factoryType).ToList();

        if (factoryObject == null || factoryObject.Count == 0) yield break;

        _factory = factoryObject[0];
        _gameroom = _gameModeProperty.GetValue(_factory, new object[] { });
        if (_gameroom.GetType() == _factoryInfiniteModeType)
        {
            Infinite = true;
            StartCoroutine(WaitUntilEndFactory());
        }
    }

    private UnityEngine.Object GetBomb => (UnityEngine.Object)_currentBombField.GetValue(_gameroom);

    private IEnumerator WaitUntilEndFactory()
    {
        yield return new WaitUntil(() => GetBomb != null);

        while (GetBomb != null)
        {
            UnityEngine.Object currentBombOBJ = GetBomb;
            Bomb bomb1 = (Bomb)_internalBombProperty.GetValue(currentBombOBJ, null);
            yield return new WaitUntil(() => bomb1.HasDetonated || bomb1.IsSolved());

            wait = true;
            Modules.Clear();
            BombCommanders.Clear();
            Bombs.Clear();

            while (currentBombOBJ == GetBomb)
            {
                yield return new WaitForSeconds(0.10f);
                if (currentBombOBJ != GetBomb)
                    continue;
                yield return new WaitForSeconds(0.10f);
            }
            
            StartCoroutine(CheckForBomb());
        }
    }
    //factory specific types

    private static Type _factoryType = null;
    private static Type _factoryBombType = null;
    private static PropertyInfo _internalBombProperty = null;

    private static Type _factoryStaticModeType = null;
    private static Type _factoryFiniteModeType = null;
    private static Type _factoryInfiniteModeType = null;

    private static PropertyInfo _gameModeProperty = null;
    private static FieldInfo _currentBombField = null;

    private object _factory = null;
    private object _gameroom = null;
    #endregion
}
