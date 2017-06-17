using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{

    /// <summary>
    /// 垂直同步类型
    /// </summary>
    public enum VSyncCountSetting
    {
        DontSync,
        EveryVBlank,
        EverSecondVBlank
    }
    /// <summary>
    /// 设置最大FPS值，放到游戏一开始的对象上面可以设置
    /// </summary>
    public class SetMaxFPS : MonoBehaviour
    {
        /// <summary>
        /// 用于快捷设置Unity Quanity设置中的垂直同步相关参数  
        /// </summary>
        public VSyncCountSetting VSyncCount = VSyncCountSetting.DontSync;
        /// <summary>
        /// 不设限制，保持可达到的最高帧率  
        /// </summary>
        public bool MaxNoLimit = false;
        /// <summary>
        /// 帧率的值
        /// </summary>
        public int MaxFPSValue = 35;
        /// <summary>
        /// 是否在GUI上显示fps
        /// </summary>
        public bool _bDebugGuiFPS = false;
        private GUIStyle style = new GUIStyle();

        void Awake()
        {
            //设置垂直同步相关参数  
            switch (VSyncCount)
            {
                //默认设置，不等待垂直同步，可以获取更高的帧率数  
                case VSyncCountSetting.DontSync:
                    QualitySettings.vSyncCount = 0;
                    break;

                //EveryVBlank，相当于帧率设为最大60  
                case VSyncCountSetting.EveryVBlank:
                    QualitySettings.vSyncCount = 1;
                    break;
                //EverSecondVBlank情况，相当于帧率设为最大30  
                case VSyncCountSetting.EverSecondVBlank:
                    QualitySettings.vSyncCount = 2;
                    break;

            }

            //设置没有帧率限制，火力全开！  
            if (MaxNoLimit)
            {
                Application.targetFrameRate = -1;
            }
            //设置帧率的值  
            else
            {
                Application.targetFrameRate = MaxFPSValue - 1;
            }
            style.fontSize = 20;
            style.normal.textColor = Color.yellow;
        }
        void OnGUI()
        {
            if (!_bDebugGuiFPS)
            {
                return;
            }
            float fps = (1f / Time.smoothDeltaTime);
            GUI.Label(new Rect(10, 5, 50, 20), fps.ToString("#,##0.0 fps"), style);
        }
    }
}
