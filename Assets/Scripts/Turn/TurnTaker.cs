using System;
using Events;
using Movement;
using UnityEngine;

namespace Turn
{
    public class TurnTaker : MonoBehaviour
    {
        public int id;
        public bool isPlayer;
        public TurnTakerRuntimeSet runtimeSet;
        public int nextTurn;
        public MoveController MoveController { get; private set; }
        public GameEvent startTurn;
        public GameEvent endTurn;

        public void StartTurn()
        {
            Debug.Log("Starting turn for " + id);
            startTurn.Raise();
            MoveController.StartTurn();
        }

        public void EndTurn()
        {
            nextTurn += 100;
            MoveController.EndTurn();
            endTurn.Raise();
        }

        private void OnEnable()
        {
            runtimeSet.Add(this);
            MoveController = gameObject.GetComponent<MoveController>();
        }

        private void Awake()
        {
            runtimeSet.Add(this);
        }

        private void OnDisable()
        {
            runtimeSet.Remove(this);
        }
    }
}