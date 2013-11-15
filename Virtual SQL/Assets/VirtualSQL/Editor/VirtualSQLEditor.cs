using UnityEditor;
using System.Collections;

public class VirtualSQLEditor {

	[MenuItem("Virtual SQL/Add a Table")]
	static void addTable() {
		// parse settings from json
		System.IO.TextWriter w = new System.IO.StreamWriter(UnityEngine.Application.dataPath + "/Resources/VirtualSQL Data/family");
		
		// build head of initializer
		w.WriteLine("// DON'T MODIFY THIS FILE MANUALLY SINCE IT WAS GENERATED FROM NETWORK TOOL CHAIN!");
		
		w.Flush();
		
		w.Close();
	}
}
