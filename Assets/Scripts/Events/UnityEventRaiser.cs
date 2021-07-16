using UnityEngine;
using UnityEngine.Events;

namespace Events
{
    public class UnityEventRaiser : MonoBehaviour
    {
        public UnityEvent OnEnableEvent;

        public void OnEnable()
        {
            OnEnableEvent.Invoke();
        }
    }
}