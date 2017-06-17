using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    /// <summary>
    /// ‘⁄GUI…œœ‘ æFPS
    /// </summary>
    public class FPSCounter : MonoBehaviour
    {
        private GUIStyle style = new GUIStyle();

        void Awake()
        {
            style.fontSize = 20;
            style.normal.textColor = Color.white;

            Application.targetFrameRate = 60;
        }

        void OnGUI()
        {
            float fps = (1f / Time.smoothDeltaTime);
            GUI.Label(new Rect(10, 5, 50, 20), fps.ToString("#,##0.0 fps"), style);
        }
    }
}
