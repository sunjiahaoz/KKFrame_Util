using UnityEngine;
using UnityEditor;
using System.Collections;
using KK.Frame.Util.Editor;

public class DemoEditor : Editor{

    [MenuItem("Tools/SSS")]
	public static void SSSSS()
    {
        ToolsEditor.OpenFolder("C://");
    }
}
