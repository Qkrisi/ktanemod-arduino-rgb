using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class arduinoModHoldable : MonoBehaviour
{

    [HideInInspector]
    public arduinoService Service;

    private KMSelectable mainHoldable;
    private KMSelectable DisconnectButton;
    private KMSelectable RefreshButton;
    private KMSelectable TestButton;
    private KMSelectable[] defaultChildren;

    private GameObject Button;

    private List<GameObject> connectBTNs = new List<GameObject>();
    private List<KMSelectable> childrenBTNs = new List<KMSelectable>();

    private GameObject Frame;
    private GameObject redOBJ;
    private GameObject greenOBJ;
    private GameObject yellowOBJ;

    GameObject modSelectorObject;
    IDictionary<string, object> modSelectorAPI;
    bool isEnabled { get => modSelectorObject == null ? true : !getEnumerable().ToList().Contains("rgbService(Clone)"); }

    private IEnumerable<string> getEnumerable()
    {
        return (IEnumerable<string>)modSelectorAPI["DisabledServices"];
    }

    private int ID;
    static int Counter;

    private Material defaultMat;

    void Start()
    {
        StartCoroutine(Setup());
    }

    private IEnumerator Setup()
    {
        yield return new WaitForSeconds(0.1f);
        ID = Counter++;
        modSelectorObject = GameObject.Find("ModSelector_Info");
        if(modSelectorObject!=null) modSelectorAPI = modSelectorObject.GetComponent<IDictionary<string, object>>();
        while (true)
        {
            yield return null;
            arduinoService[] Services = FindObjectsOfType<arduinoService>();
            if (Services.Length > 0) { Service = Services[0]; }
            else { Service = null; }
            if (Service != null && isEnabled)
            {
                mainHoldable = this.GetComponent<KMSelectable>();
                defaultChildren = mainHoldable.Children;
                DisconnectButton = mainHoldable.Children[0];
                RefreshButton = mainHoldable.Children[1];
                TestButton = mainHoldable.Children[2];
                Button = mainHoldable.Children[3].gameObject;
                Frame = mainHoldable.gameObject.transform.Find("arduinoHoldableBacking").gameObject;
                redOBJ = mainHoldable.gameObject.transform.Find("arduinoHoldableRedOBJ").gameObject;
                greenOBJ = mainHoldable.gameObject.transform.Find("arduinoHoldableGreenOBJ").gameObject;
                yellowOBJ = mainHoldable.gameObject.transform.Find("arduinoHoldableYellowOBJ").gameObject;
                defaultMat = Frame.GetComponent<Renderer>().material;
                DisconnectButton.OnInteract += disconnectBTNAction;
                RefreshButton.OnInteract += refreshBTNAction;
                TestButton.OnInteract += testBTNAction;
                if (Service.arduinoConnection._connected)
                {
                    Frame.GetComponent<Renderer>().material = greenOBJ.GetComponent<Renderer>().material;
                }
                StartCoroutine(Refresh());
                //StartCoroutine(getName());
                yield break;
            }
        }
    }

    /*private IEnumerator getName()
    {
        yield return null;
        while (Service != null)
        {
            yield return null;
            if (Service.currentState == KMGameInfo.State.Gameplay)
            {
                Debug.LogFormat("[Arduino Manager Holdable] The name of the held module is {0}, bomb active: {1}, module count is {2}, bomb count is {3}, bomb commander count is {4}", Service.currentModuleName, Service.BombActive, Service.Modules.Count, Service.Bombs.Count, Service.BombCommanders.Count);
            }
        }
        yield break;
    }*/

    private bool ConnectBTNAction(string portName)
    {
        if (Service != null && isEnabled) { StartCoroutine(attemptConnection(portName)); }
        return false;
    }

    private IEnumerator attemptConnection(string portName)
    {
        yield return null;
        Debug.LogFormat("Connecting to {0}", portName);
        if (Service.arduinoConnection._connected)
        {
            Service.arduinoConnection.Disconnect();
        }
        Frame.GetComponent<Renderer>().material = yellowOBJ.GetComponent<Renderer>().material;
        Service.setPins();
        Service.arduinoConnection.Connect(portName, Service.Baud);
        yield return new WaitForSeconds(1.5f);
        if (Service.arduinoConnection._connected)
        {
            //StartCoroutine(Test());
            Frame.GetComponent<Renderer>().material = greenOBJ.GetComponent<Renderer>().material;
        }
        else
        {
            Frame.GetComponent<Renderer>().material = redOBJ.GetComponent<Renderer>().material;
        }
        yield break;
    }

    private bool disconnectBTNAction()
    {
        if (Service != null && isEnabled)
        {
            Service.arduinoConnection.Disconnect();
            Frame.GetComponent<Renderer>().material = defaultMat;
        }
        return false;
    }

    private bool testBTNAction()
    {
        if (Service != null && isEnabled) { StartCoroutine(Test()); }
        return false;
    }

    private IEnumerator Test()
    {
        yield return null;
        Service.setPins();
        yield return new WaitUntil(() => Service.arduinoConnection._connected);
        //Service.minimumWait = Service.arduinoConnection.Calibrate(String.Format("{0} {1} {2} 255 255 255", Service.RP, Service.GP, Service.BP), Service.goAbove);
        yield return new WaitForSeconds(3f);
        Debug.LogFormat("[Arduino Manager Holdable] Pins are: {0}, {1}, {2}. Baud rate is {3}. Implementation enabled: {4}, output time: {5}", Service.RP, Service.GP, Service.BP, Service.Baud, Service.implementationEnabled, Service.minimumWait);
        Service.arduinoConnection.Stop();
        yield break;
    }

    private bool refreshBTNAction()
    {
        if (Service != null && isEnabled) { StartCoroutine(Refresh()); }
        return false;
    }

    private IEnumerator Refresh()
    {
        yield return null;
        Button.GetComponent<Renderer>().enabled = false;
        for (int i = 0; i < connectBTNs.Count; i++)
        {
            if (i == 0) { continue; }
            Destroy(connectBTNs[i]);
        }
        connectBTNs.Clear();
        connectBTNs.Add(Button);
        childrenBTNs.Clear();
        childrenBTNs.Add(DisconnectButton);
        childrenBTNs.Add(RefreshButton);
        string[] ports = Service.arduinoConnection.getAvailablePorts();
        if (ports.Length > 0) { 
            connectBTNs[0].transform.Find("buttonText").gameObject.GetComponent<TextMesh>().text = ports[0];
            connectBTNs[0].GetComponent<Renderer>().enabled = true;
        }
        else
        {
            connectBTNs[0].transform.Find("buttonText").gameObject.GetComponent<TextMesh>().text = "";
            connectBTNs[0].GetComponent<Renderer>().enabled = false;
        }
        for (int i = 0; i < ports.Length; i++)
        {
            connectBTNs.Add(Instantiate(connectBTNs[0], new Vector3(connectBTNs[0].transform.position.x, connectBTNs[0].transform.position.y, connectBTNs[0].transform.position.z - (0.05f * (i + 1))), connectBTNs[0].transform.rotation));
        }
        for(int i = 0; i < ports.Length; i++)
        {
            connectBTNs[i].transform.Find("buttonText").gameObject.GetComponent<TextMesh>().text = ports[i];
            connectBTNs[i].transform.parent = mainHoldable.gameObject.GetComponent<Transform>();
            if (i == 0) { continue; }
            connectBTNs[i].transform.localPosition = new Vector3(connectBTNs[0].transform.localPosition.x, connectBTNs[0].transform.localPosition.y, connectBTNs[0].transform.localPosition.z - (0.05f * i));
        }
        connectBTNs[connectBTNs.Count-1].transform.parent = mainHoldable.gameObject.GetComponent<Transform>();
        GameObject btnToRemove = connectBTNs[connectBTNs.Count - 1];
        connectBTNs.RemoveAt(connectBTNs.Count - 1);
        Destroy(btnToRemove);
        foreach (GameObject item in connectBTNs)
        {
            item.GetComponent<KMSelectable>().OnInteract += () => ConnectBTNAction(item.transform.Find("buttonText").gameObject.GetComponent<TextMesh>().text);
            item.GetComponent<Renderer>().enabled = true;
            childrenBTNs.Add(item.GetComponent<KMSelectable>());
        }
        Debug.Log("Got there!");
        while (mainHoldable.Children.Length < childrenBTNs.Count)
        {
            yield return null;
            mainHoldable.Children = childrenBTNs.ToArray();
        }
        yield break;
    }

    #pragma warning disable 414
    string TwitchHelpMessage = "Commands are: '!{0} disconnect'; '!{0} refresh'; '!{0} test'; '!{0} connect x' where x is the position of the correct connect button from top to bottom. Only the streamer can run these commands.";
    #pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant();
        if (command == "disconnect")
        {
            yield return null;
            yield return new object[] { "streamer", (Action)(() => DisconnectButton.OnInteract()), "This command is for the streamer only." };
        }
        if (command == "refresh")
        {
            yield return null;
            yield return new object[] { "streamer", (Action)(() => RefreshButton.OnInteract()), "This command is for the streamer only." };
        }
        if (command == "test")
        {
            yield return null;
            yield return new object[] { "streamer", (Action)(() => TestButton.OnInteract()), "This command is for the streamer only." };
        }
        if (command.StartsWith("connect", StringComparison.InvariantCulture))
        {
            command = command.Replace("connect ", "");
            yield return null;
            if(!int.TryParse(command, out int num))
            {
                yield return "sendtochaterror Number not valid!";
                yield break;
            }
            if(num<1 || num > connectBTNs.Count)
            {
                yield return "sendtochaterror Number out of range!";
                yield break;
            }
            yield return new object[] { "streamer", (Action)(() => connectBTNs[num-1].GetComponent<KMSelectable>().OnInteract()), "This command is for the streamer only." };
            yield return String.Format("sendtochat Connecting to {0}", connectBTNs[num-1].transform.Find("buttonText").gameObject.GetComponent<TextMesh>().text);
            yield return new WaitForSeconds(1.51f);
            if (Service.arduinoConnection._connected)
            {
                yield return "sendtochat PraiseIt Connection succesful! PraiseIt";
            }
            else
            {
                yield return "sendtochat VoteNay Connection failed! VoteNay";
            }
        }
    }
}