using UnityEngine;
using System.Collections;

namespace KK.Frame.Util
{
    public class FSMMono<T, U> : MonoBehaviour, IFSM<T,U>
    {
        protected FiniteStateMachine<T, U> FSM;

        public virtual void FSMInit()
        {

        }
        public virtual void FSMUpdate()
        {
            FSM.Update();
        }
        public virtual void FSMLateUpdate()
        {
            FSM.LateUpdate();
        }
        public virtual void ChangeGlobalState(FSMState<T, U> NewState)
        {
            FSM.ChangeGlobalState(NewState);
        }

        public virtual void ChangeGlobalState(U state)
        {
            FSM.ChangeGlobalState(state);
        }

        public void ChangeState(FSMState<T, U> NewState, float fDelay)
        {
            StartCoroutine(OnChangetStateDelay(NewState, fDelay));
        }

        IEnumerator OnChangetStateDelay(FSMState<T, U> NewState, float fDelay)
        {
            yield return new WaitForSeconds(fDelay);
            ChangeState(NewState);
        }

        public void ChangeState(FSMState<T, U> NewState)
        {
            FSM.ChangeState(NewState);
        }

        public void RevertToPreviousState()
        {
            FSM.RevertToPreviousState();
        }

        //Changing state via enum
        public void ChangeState(U stateID)
        {
            FSM.ChangeState(stateID);
        }

        public void ChangeState(U stateID, float fDelay)
        {
            StartCoroutine(OnChangetStateDelay(stateID, fDelay));
        }

        IEnumerator OnChangetStateDelay(U stateID, float fDelay)
        {
            yield return new WaitForSeconds(fDelay);
            ChangeState(stateID);
        }


        public FSMState<T, U> RegisterState(FSMState<T, U> state)
        {
            return FSM.RegisterState(state);
        }

        public void UnregisterState(FSMState<T, U> state)
        {
            FSM.UnregisterState(state);
        }

        public U GetCurStateID()
        {
            return FSM.GetCurStateID();
        }

        public FSMState<T, U> GetCurState()
        {
            return FSM.GetCurState();
        }
    }


    public interface IFSM<T,U>
    {
        //FiniteStateMachine<T, U> FSM { get; }
        void FSMInit();
        void FSMUpdate();
        void FSMLateUpdate();
        void ChangeGlobalState(FSMState<T, U> NewState);
        void ChangeGlobalState(U state);
        void ChangeState(FSMState<T, U> NewState, float fDelay);
        void ChangeState(FSMState<T, U> NewState);
        void RevertToPreviousState();
        void ChangeState(U stateID);
        void ChangeState(U stateID, float fDelay);
        FSMState<T, U> RegisterState(FSMState<T, U> state);
        void UnregisterState(FSMState<T, U> state);
        U GetCurStateID();
        FSMState<T, U> GetCurState();
    }

    public class FSMSimpleClass<T, U>
    {
        public FiniteStateMachine<T, U> FSM;

        public virtual void FSMInit()
        {

        }
        public virtual void FSMUpdate()
        {
            FSM.Update();
        }

        public virtual void ChangeState(U newState)
        {
            FSM.ChangeState(newState);
        }

        public virtual void ChangeState(MonoBehaviour com, U newState, float delay)
        {
            com.StartCoroutine(ChangeStateWithDelay(newState, delay));
        }

        public virtual U GetCurStateID()
        {
            return FSM.GetCurStateID();
        }

        private IEnumerator ChangeStateWithDelay(U newState, float delay)
        {
            yield return new WaitForSeconds(delay);
            ChangeState(newState);
        }

        public FSMState<T, U> RegisterState(FSMState<T, U> state)
        {
            return FSM.RegisterState(state);
        }

        public void UnregisterState(FSMState<T, U> state)
        {
            FSM.UnregisterState(state);
        }
    }
}
