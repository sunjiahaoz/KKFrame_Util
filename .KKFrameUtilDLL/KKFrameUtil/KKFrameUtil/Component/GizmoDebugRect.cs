using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    /// <summary>
    /// 指定两个点，Scene场景显示一个矩形
    /// </summary>
    public class GizmoDebugRect : MonoBehaviour
    {
        public Color _color = Color.green;
        public float _fPtSize = 1f;
        public Transform _trRT;
        public Transform _trLB;
        
        void OnDrawGizmos()
        {
            if (_trLB == null
                || _trRT == null)
            {
                return;
            }
            Gizmos.color = _color;
            Vector3 posLT = new Vector3(_trLB.position.x, _trRT.position.y, _trLB.position.z);
            Vector3 posLB = _trLB.position;
            Vector3 posRB = new Vector3(_trRT.position.x, _trLB.position.y, _trRT.position.z);
            Vector3 posRT = _trRT.position;            

            Gizmos.DrawLine(posLT, posRT);
            Gizmos.DrawLine(posRT, posRB);
            Gizmos.DrawLine(posRB, posLB);
            Gizmos.DrawLine(posLB, posLT);

            Gizmos.DrawSphere(posLT, _fPtSize);
            Gizmos.DrawSphere(posLB, _fPtSize);
            Gizmos.DrawSphere(posRB, _fPtSize);
            Gizmos.DrawSphere(posRT, _fPtSize);
        }
    }
}
