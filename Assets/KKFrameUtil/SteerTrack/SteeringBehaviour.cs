using UnityEngine;
using System.Collections;

namespace KK.Frame.Util.SteerTrack
{
	[RequireComponent(typeof(SteeringAgent))]
	public abstract class SteeringBehaviour : MonoBehaviour {

        public float Weight = 1;

        protected SteeringAgent agent;
        public SteeringAgent Agent
        {
            get 
            {
                return ToolsUseful.DefaultGetComponent<SteeringAgent>(gameObject); 
            }
        }

        public abstract Vector3 GetVelocity();
        public virtual void Reset()
        {
            Agent.ResetDataInit();
        }

		void Start () {
            agent = GetComponent<SteeringAgent>();
            agent.RegisterSteeringBehaviour(this);
		}

		void OnDestroy()
		{
            if (agent != null)
                agent.DeregisterSteeringBehaviour(this);
		}   
	}
}