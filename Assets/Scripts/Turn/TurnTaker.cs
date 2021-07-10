using Movement;
using UnityEngine;
using UnityEngine.Serialization;

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
            moveController.canMove = true;
            moveController.moving = false;
        }

        public void EndTurn()
        {
            turn = false;
            nextTurn += 100;
            moveController.FinishedMoving();
        }
    }
}