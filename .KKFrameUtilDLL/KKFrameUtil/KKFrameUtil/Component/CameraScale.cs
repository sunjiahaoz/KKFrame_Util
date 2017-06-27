using UnityEngine;

namespace KK.Frame.Util
{
    /// <summary>
    /// Camera缩放，请将该脚本放置到与Camera同对象上
    /// 设置原始尺寸，之后会根据实际尺寸自动缩放摄像头
    /// 主要用于对3d世界的缩放
    /// </summary>
    public class CameraScale : MonoBehaviour
    {
        /// <summary>
        /// 参考宽度，即默认开发时使用的宽度
        /// </summary>
        public int mNormalWidth = 960;
        /// <summary>
        /// 参考高度，即默认开发时使用的高度
        /// </summary>
        public int mNormallHeight = 640;
        void Start()
        {
            Camera camera = GetComponent<Camera>();
            if (camera == null)
                return;
            int manualHeight;
            if (System.Convert.ToSingle(Screen.height) / Screen.width > System.Convert.ToSingle(mNormallHeight) / mNormalWidth)
                manualHeight = Mathf.RoundToInt(System.Convert.ToSingle(mNormalWidth) / Screen.width * Screen.height);
            else
                manualHeight = mNormallHeight;

            float scale = System.Convert.ToSingle(manualHeight * 1.0f / mNormallHeight);
            camera.fieldOfView *= scale;
        }
    }
}
