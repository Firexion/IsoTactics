using System;

namespace Turn
{
    [Serializable]
    public class TurnTakerReference
    {
        public TurnTakerVariable Variable;

        public TurnTakerReference()
        {
        }

        public TurnTakerReference(TurnTakerVariable variable)
        {
            Variable = variable;
        }

        public TurnTaker Value => Variable.Value;

        public static implicit operator TurnTaker(TurnTakerReference reference)
        {
            return reference.Value;
        }
    }
}