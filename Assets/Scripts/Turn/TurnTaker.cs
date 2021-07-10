using UnityEngine;
using UnityEngine.Serialization;

namespace Turn
{
    public class TurnTaker : MonoBehaviour
    {
        public bool Turn { get; set; }
        [SerializeField] public int id;
        [SerializeField] public int nextTurn;

        public void TakeTurn()
        {
            Turn = true;
            
        }
    }
}