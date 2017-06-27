using UnityEngine;
using System.Collections;

namespace KK.Frame.Util.SteerTrack
{
    public class SteeringAccelerated : SteeringBehaviour
    {
        public Vector3 _vecCur = Vector3.zero; // 初始速度
        public Vector3 _Accelerated = Vector3.zero;
        public override Vector3 GetVelocity()
        {   
            _vecCur += _Accelerated * Time.deltaTime;
            return _vecCur;
        }
    }
}

