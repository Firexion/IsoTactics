using UnityEngine;

namespace Turn
{
    [CreateAssetMenu]
    public class TurnTakerVariable : ScriptableObject
    {
        public TurnTaker Value;

        public void SetTurnTaker(TurnTaker value)
        {
            Value = value;
            Value.StartTurn();
        }

        public void SetTurnTaker(TurnTakerVariable value)
        {
            Value = value.Value;
            Value.StartTurn();
        }

        public void SetInactive()
        {
            Value = null;
        }

        public bool IsActive(TurnTaker turnTaker)
        {
            return Value != null && Value.Stats.unit.id == turnTaker.Stats.unit.id;
        }

        public bool IsPlayer()
        {
            return Value != null && Value.isPlayer;
        }
    }
}