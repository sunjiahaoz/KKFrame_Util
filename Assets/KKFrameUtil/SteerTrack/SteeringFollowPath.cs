using UnityEngine;
using System.Collections;

namespace KK.Frame.Util.SteerTrack
{
    public class SteeringFollowPath : SteeringBehaviour
    {
        public Vector3[] Path;
        public float SlowRadius = 1;
        public float StopRadius = 0.2f;
        public float NextCoordRadius = 0.2f;
        public bool Loop = false;

        public bool DrawGizmos = false;

        public bool Finished
        {
            get
            {
                return currentPoint >= Path.Length;
            }
        }

        protected int currentPoint = 0;

        public void SetNewPath(Vector3[] path)
        {
            Path = path;
            currentPoint = 0;
        }

        public override Vector3 GetVelocity()
        {
            Vector3 velocity;

            if (currentPoint >= Path.Length)
                return Vector3.zero;
            else if (!Loop && currentPoint == Path.Length - 1)
                velocity = arrive(Path[currentPoint]);
            else
                velocity = seek(Path[currentPoint]);

            float distance = Vector3.Distance(transform.position, Path[currentPoint]);
            if ((currentPoint == Path.Length - 1 && distance < StopRadius) || distance < NextCoordRadius)
            {
                currentPoint++;
                if (Loop && currentPoint == Path.Length)
                    currentPoint = 0;
            }

            return velocity;
        }

        Vector3 seek(Vector3 targetPoint)
        {
            return ((targetPoint - transform.position).normalized * agent.MaxVelocity) - agent.CurrentVelocity;
        }

        Vector3 arrive(Vector3 targetPoint)
        {
            float distance = Vector3.Distance(transform.position, (Vector3)targetPoint);
            Vector3 desiredVelocity = (targetPoint - transform.position).normalized;

            if (distance < StopRadius)
                desiredVelocity = Vector3.zero;
            else if (distance < SlowRadius)
                desiredVelocity = desiredVelocity * agent.MaxVelocity * ((distance - StopRadius) / (SlowRadius - StopRadius));
            else
                desiredVelocity = desiredVelocity * agent.MaxVelocity;

            return desiredVelocity - agent.CurrentVelocity;
        }

        void OnDrawGizmos()
        {
            if (DrawGizmos)
            {
                if (currentPoint < Path.Length)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(Path[currentPoint], .05f);

                    if (currentPoint == Path.Length - 1)
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(Path[currentPoint], SlowRadius);

                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireSphere(Path[currentPoint], StopRadius);
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                        Gizmos.DrawWireSphere(Path[currentPoint], NextCoordRadius);
                    }
                }

                Gizmos.color = Color.green;
                for (int i = 0; i < Path.Length - 1; ++i)
                {
                    Gizmos.DrawLine(Path[i], Path[i + 1]);
                }
            }
        }
    }
}