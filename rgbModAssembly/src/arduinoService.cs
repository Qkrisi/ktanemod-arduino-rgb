using UnityEngine;
using rgbMod.Arduino;

public class arduinoService : MonoBehaviour
{
    public Arduino arduinoConnection = new Arduino();
    private KMGameInfo gameInfo;

    void Start(){
        Debug.Log("[Arduino Manager] Initialised");
        gameInfo = GetComponent<KMGameInfo>();
    }
}

