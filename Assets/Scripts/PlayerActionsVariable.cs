using UnityEngine;

namespace DefaultNamespace
{
    [CreateAssetMenu]
    public class PlayerActionsVariable : ScriptableObject
    {
        public PlayerActions Value;

        public static implicit operator PlayerActions(PlayerActionsVariable variable)
        {
            return variable.Value;
        }
   
        private void OnEnable()
        {
            Value = new PlayerActions();
        }
    }
}