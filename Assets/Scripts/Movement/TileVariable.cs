using UnityEngine;

namespace Movement
{
    [CreateAssetMenu]
    public class TileVariable : ScriptableObject
    {
        public Tile Value;

        public void SetValue(Tile value)
        {
            if (Value != null) Value.currentlySelecting = false;
            Value = value;
            Value.currentlySelecting = true;
        }

        public void Remove()
        {
            if (Value != null) Value.currentlySelecting = false;
            Value = null;
        }

    }
}