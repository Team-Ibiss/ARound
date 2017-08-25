using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapBoxAnimation : MonoBehaviour {
    public Text txt;
    public Image img;
    public Animator ani;
	public void Delete()
    {
        txt.text = "";
        txt.enabled = false;
        img.enabled = false;
    }
}
