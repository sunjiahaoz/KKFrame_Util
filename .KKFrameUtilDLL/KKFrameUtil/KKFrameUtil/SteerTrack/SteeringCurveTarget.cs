using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace KK.Frame.Util.SteerTrack
{
    public class SteeringCurveTarget : SteeringFollowPath
    {
        static Bezier _curveCalc;
        public void SetTarget(Vector3 vecTarget)
        {
            Vector3 vec1 = new Vector3(0, Random.Range(1f, 3f), 0f);
            Vector3 vec2 = new Vector3(Random.Range(0.5f, 2f), 0, 0f);
            if (_curveCalc == null)
            {
                _curveCalc = new Bezier(transform.position, vec1, vec2, vecTarget);
            }
            else
            {
                _curveCalc.ResetBezier(transform.position, vec1, vec2, vecTarget);
            }

            List<Vector3> lst = new List<Vector3>();
            float fCount = 10;
            float fOff = 1f / fCount;
            for (int i = 0; i <= fCount; ++i)
            {
                Vector3 vec = _curveCalc.GetPointAtTime((float)(i * fOff));
                lst.Add(vec);
            }
            SetNewPath(lst.ToArray());
        }
        Vector3 _dir = Vector3.zero;
        public override Vector3 GetVelocity()
        {
            if (currentPoint != Path.Length)
            {
                _dir = transform.eulerAngles;
                return base.GetVelocity();
            }
            else
            {
                if (Path.Length >= 2)
                {
                    // todo                    
                    return (Path[Path.Length - 1] - Path[Path.Length - 2]).normalized * agent.MaxVelocity - agent.CurrentVelocity;
                }
                else
                {
                    return Vector3.zero;
                }                
            }
        }
    }    
}