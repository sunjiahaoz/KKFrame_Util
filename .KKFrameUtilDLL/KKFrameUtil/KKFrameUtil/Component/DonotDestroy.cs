using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    /// <summary>
    /// 有该组件的对象切换场景不会销毁
    /// </summary>
    public class DonotDestroy : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
