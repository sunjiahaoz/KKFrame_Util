using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KK.Frame.Util
{
    /// <summary>
    /// 延迟执行工具
    /// 需要协程支持
    /// </summary>
    public class DelayToInvoke : MonoBehaviour
    {

        public static IEnumerator DelayToInvokeDo(System.Action action, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            action();
        }
        public static IEnumerator DelayToInvokeDoList(List<System.Action> action, float delaySeconds)
        {
            yield return new WaitForSeconds(delaySeconds);
            foreach (System.Action p in action)
            {
                p();
                yield return new WaitForEndOfFrame();
            }
        }        
    }
}
