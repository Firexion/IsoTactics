using System;
using Events;
using Movement;
using Unit;
using UnityEngine;

namespace Turn
{
    public class TurnTaker : MonoBehaviour
    {
     //   public UnitObject unit;
        public bool isPlayer;
        public TurnTakerRuntimeSet runtimeSet;
        public int NextTurn { get; set; }
        public MoveController MoveController { get; private set; }
        public GameEvent startTurn;
        public GameEvent endTurn;

        public RuntimeUnitStats Stats { get; private set; }

        public void StartTurn()
        {
            startTurn.Raise();
            MoveController.StartTurn();
        }

        public void EndTurn()
        {
            NextTurn += 100;
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
            Stats = GetComponent<RuntimeUnitStats>();
            CalculateInitialNextTurn();
            runtimeSet.Add(this);
        }

        private void OnDisable()
        {
            runtimeSet.Remove(this);
        }
        
        private void CalculateInitialNextTurn()
        {
            Debug.Log("Stats: " + Stats);
            NextTurn = 100 - Stats.unit.Speed;
        }
    }
}