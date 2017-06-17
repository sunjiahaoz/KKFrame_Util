using UnityEngine;
using System.Collections;

namespace KK.Frame.Util.SteerTrack
{
    public class SteeringTarget : MonoBehaviour
    {

        //public Steer2D.Seek AgentSeek;
        public SteerTrack.SteeringArrive AgentArrive;

        void Start()
        {
            //if (AgentSeek != null)
            //    AgentSeek.TargetPoint = transform.position;
            Time.timeScale = 1;

            if (AgentArrive != null)
                AgentArrive._vecTarget = transform.position;
        }

        void Update()
        {
            //Debug.Log(Time.deltaTime + " " + Time.timeScale);
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                position.z = 0;
                transform.position = position;

                //if (AgentSeek != null)
                //    AgentSeek.TargetPoint = position;

                if (AgentArrive != null)
                    AgentArrive._vecTarget = position;
            }
        }
    }
}
