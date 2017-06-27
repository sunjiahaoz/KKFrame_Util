using UnityEngine;
using System.Collections;
namespace KK.Frame.Util.SteerTrack
{
    public class SteeringArrive : SteeringBehaviour
    {
        public Vector3 _vecTarget = Vector3.zero;
        public float _fSlowRadius = 1f;
        public float _fStopRadius = 0.2f;
        public bool DrawGizmos = false;

        public override Vector3 GetVelocity()
        {
            float fDistance = Vector3.Distance(transform.position, _vecTarget);
            Vector3 vec3DesiredVel = (_vecTarget - transform.position).normalized;

            if (fDistance < _fStopRadius)
            {
                vec3DesiredVel = Vector3.zero;
            }
            else if (fDistance < _fSlowRadius)
            {
                vec3DesiredVel = vec3DesiredVel * agent.MaxVelocity * ((fDistance - _fStopRadius) / (_fSlowRadius - _fStopRadius));
            }
            else
            {
                vec3DesiredVel = vec3DesiredVel * agent.MaxVelocity;
            }
            return vec3DesiredVel - agent.CurrentVelocity;
        }

        void OnDrawGizmos()
        {
            if (DrawGizmos)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_vecTarget, _fSlowRadius);

                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_vecTarget, _fStopRadius);
            }
        }
    }


}