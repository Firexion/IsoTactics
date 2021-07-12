using Movement;
using UnityEngine;

namespace Turn
{
    public class TurnTaker : MonoBehaviour
    {
         public bool turn;
        [SerializeField] public int id;
        [SerializeField] public int nextTurn;
        [SerializeField] public MoveController moveController;

        public void StartTurn()
        {
            turn = true;
            moveController.StartTurn();
        }

        public void EndTurn()
        {
            turn = false;
            nextTurn += 100;
            moveController.EndTurn();
        }
    }
}