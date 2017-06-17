using UnityEngine;
using System.Collections;

namespace KK.Frame.Util.SteerTrack
{
    public class SteeringDirOffset : SteeringBehaviour {
        public Vector2 _vecA = Vector2.zero;
        public bool ReSet = false;
        Vector3 _initTargetPos = Vector3.zero;
        float _fTimeTotal = 0f;
        public override Vector3 GetVelocity()
        {
            if (ReSet)
            {
                _fTimeTotal = 0;
                ReSet = false;
            }
            _fTimeTotal += Time.deltaTime;
            return agent.CurrentVelocity + (Vector3)_vecA * _fTimeTotal;
        }
    }
}