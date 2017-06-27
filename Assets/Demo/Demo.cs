using UnityEngine;
using System.Collections;
using KK.Frame.Util;

public class Demo : MonoBehaviour {

    public CameraShake _shake;

    void OnGUI()
    {
        if (GUILayout.Button("Click", GUILayout.Width(100), GUILayout.Height(100)))
        {
            Debug.Log("<color=green>[log]</color>---" + ToolsUseful.GetStackInfo());
            _shake.DoShake();
        }
    }
}
