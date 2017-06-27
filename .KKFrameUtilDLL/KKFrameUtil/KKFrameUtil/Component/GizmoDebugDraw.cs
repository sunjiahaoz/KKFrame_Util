/*
*GizmoDebugDraw
*by sunjiahaoz 2016-4-15
*
*挂到对象上可以用于标注该对象的位置Gizmo
*/
using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    /// <summary>
    /// 挂到对象上可以用于标注该对象的位置Gizmo
    /// </summary>
    public class GizmoDebugDraw : MonoBehaviour
    {
        public Color _color = Color.green;
        public float _fSize = 1f;
        void OnDrawGizmos()
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _fSize);
        }
    }
}
