using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorChangerScript : MonoBehaviour {
	public Material[] Materials;
	public GameObject Background;
	public KMSelectable[] Buttons;

	List<int> arduinoRGBValues = new List<int>() {0,0,0};
	void Start()
	{
		Buttons[0].OnInteract += delegate(){arduinoRGBValues=new List<int>() {255,0,0};Background.GetComponent<Renderer>().material=Materials[0];return false;};
		Buttons[1].OnInteract += delegate(){arduinoRGBValues=new List<int>() {0,0,255};Background.GetComponent<Renderer>().material=Materials[1];return false;};
		Buttons[2].OnInteract += delegate(){arduinoRGBValues=new List<int>() {0,255,0};Background.GetComponent<Renderer>().material=Materials[2];return false;};
		Buttons[3].OnInteract += delegate(){arduinoRGBValues=new List<int>() {255,255,0};Background.GetComponent<Renderer>().material=Materials[3];return false;};
		return;
	}

}
