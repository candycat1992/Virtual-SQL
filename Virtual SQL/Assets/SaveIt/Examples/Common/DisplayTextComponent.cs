using UnityEngine;
using System.Collections;

public class DisplayTextComponent : MonoBehaviour {

    public string Text;

	void OnGUI()
    {
        GUI.Label(new Rect(10, 5, Screen.width - 10, 60), Text);
	}
}
