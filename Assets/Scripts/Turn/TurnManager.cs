using System.Collections.Generic;
using UnityEngine;

namespace Turn
{
    public class TurnManager : MonoBehaviour
    {
        private static readonly Dictionary<int, Queue<TurnTaker>> TurnOrder = new Dictionary<int, Queue<TurnTaker>>();

        private static int _turnClock;
        private static int _activeUnitIndex = -1;
        private static TurnTaker _activeTurnTaker;

        private static CameraController _cameraController;
        private void Start()
        {
            _cameraController = FindObjectOfType<CameraController>();
            var turnTakers = FindObjectsOfType<TurnTaker>();
            foreach (var turnTaker in turnTakers) AddTurnTaker(turnTaker);
        }

        // Update is called once per frame
        private void Update()
        {
            if (_activeUnitIndex == -1)
            {
                // Iterate _turnClock until it's someone's turn
                while (_activeUnitIndex == -1)
                {
                    _turnClock++;
                    if (!TurnOrder.TryGetValue(_turnClock, out var turnTakers)) continue;
                    if (turnTakers.Count == 0) continue;
                    SetActive(turnTakers.Peek());
                }
            }
            else if (!_activeTurnTaker.turn) // unit just finished their turn
            {
                // Move Unit to the next turn value and if there is anyone left at this turn count, they get a turn
                AddTurnTaker(_activeTurnTaker);
                if (!TurnOrder.TryGetValue(_turnClock, out var turnTakers)) return;
                if (turnTakers.Count > 1)
                {
                    turnTakers.Dequeue();
                    SetActive(turnTakers.Peek());
                }
                else
                {
                    SetInactive();
                }
            }
        }

        private void LateUpdate()
        {
            if (_activeUnitIndex != -1 && _activeTurnTaker.transform.hasChanged)
            {
                _cameraController.Focus(_activeTurnTaker.transform);
            }
        }

        private static void AddTurnTaker(TurnTaker turnTaker)
        {
            if (TurnOrder.TryGetValue(turnTaker.nextTurn, out var turnTakersAtTime))
            {
                turnTakersAtTime.Enqueue(turnTaker);
            }
            else
            {
                var queue = new Queue<TurnTaker>();
                queue.Enqueue(turnTaker);
                TurnOrder.Add(turnTaker.nextTurn, queue);
            }
        }

        private static void SetInactive()
        {
            _activeTurnTaker.EndTurn();
            TurnOrder.Remove(_turnClock);
            _activeTurnTaker = null;
            _activeUnitIndex = -1;
        }

        private static void SetActive(TurnTaker turnTaker)
        {
            _cameraController.Focus(turnTaker.transform);
            _activeTurnTaker = turnTaker;
            _activeUnitIndex = turnTaker.id;
            turnTaker.StartTurn();
        }

        public static void EndCurrentTurn()
        {
            _activeTurnTaker.EndTurn();
        }
    }
}