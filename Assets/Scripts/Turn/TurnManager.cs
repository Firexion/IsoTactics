using System.Collections.Generic;
using UnityEngine;

namespace Turn
{
    public class TurnManager : MonoBehaviour
    {
        private static readonly Dictionary<int, Queue<TurnTaker>> TurnOrder = new Dictionary<int, Queue<TurnTaker>>();
        private static int _turnClock = 0;
        public TurnTakerVariable activeTurnTaker;

        public TurnTakerRuntimeSet RuntimeSet;

        private void Awake()
        {
            Debug.Log("There are " + RuntimeSet.Items.Count + " units on the board");
            foreach (var turnTaker in RuntimeSet.Items)
            {
                AddTurnTaker(turnTaker);
            }
        }

        private void Start()
        {
            GetNextTurn();
        }

        public void EndTurn()
        {
            // Move Unit to the next turn value and if there is anyone left at this turn count, they get a turn
            AddTurnTaker(activeTurnTaker.Value);
            if (!TurnOrder.TryGetValue(_turnClock, out var turnTakers)) return;
            if (turnTakers.Count > 1)
            {
                turnTakers.Dequeue();
                activeTurnTaker.SetTurnTaker(turnTakers.Peek());
            }
            else
            {
                activeTurnTaker.SetInactive();
                GetNextTurn();
            }
        }

        private void GetNextTurn()
        {
            while (activeTurnTaker.Value == null && TurnOrder.Count > 0)
            {
                _turnClock++;
                if (!TurnOrder.TryGetValue(_turnClock, out var turnTakers)) continue;
                if (turnTakers.Count == 0) continue;
                activeTurnTaker.SetTurnTaker(turnTakers.Peek());
            }
        }

        private static void AddTurnTaker(TurnTaker turnTaker)
        {
            Debug.Log("Adding " + turnTaker.Stats.unit.id);
            if (TurnOrder.TryGetValue(turnTaker.NextTurn, out var turnTakersAtTime))
            {
                turnTakersAtTime.Enqueue(turnTaker);
            }
            else
            {
                var queue = new Queue<TurnTaker>();
                queue.Enqueue(turnTaker);
                TurnOrder.Add(turnTaker.NextTurn, queue);
            }
        }
    }
}