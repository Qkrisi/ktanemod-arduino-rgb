using UnityEngine;
using rgbMod.Arduino;

public class arduinoService : MonoBehaviour
{
    public Arduino arduinoConnection = new Arduino();
    private KMGameInfo gameInfo;

    void Start(){
        gameInfo = GetComponent<KMGameInfo>();
    }
}

