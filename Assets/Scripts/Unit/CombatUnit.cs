
using System;
using UnityEngine.Serialization;

namespace Unit
{
    [Serializable]
    public struct UnitData
    {
        public int id;
        public string Name { get; set; }

        public int Brawn { get; set; }
        public int Agility { get; set; }
        public int Perception { get; set; }
        public int Cunning { get; set; }
        public int Will { get; set; }
    }
    public abstract class CombatUnit
    {
        public int id;
        public int speedRating;
        public string name;

        public int Brawn;
        public int Agility;
        public int Perception;
        public int Cunning;
        public int Will;

        public int Hp;
        public int Mp;
        public int Speed;
        public int Move;
        public int Jump;

        public int MeleeAccuracy;
        public int RangedAccuracy;
        public int MagicAccuracy;
        public int Evade;

        protected CombatUnit(UnitData data)
        {
            this.id = data.id;
            this.name = data.Name;
            this.Brawn = data.Brawn;
            this.Agility = data.Agility;
            this.Perception = data.Perception;
            this.Cunning = data.Cunning;
            this.Will = data.Will;
            Initialize();
        }

        private void Initialize()
        {
            CalculateHp();
            CalculateMp();
            CalculateSpeed();
            CalculateMove();
            CalculateJump();
            CalculateMeleeAccuracy();
            CalculateRangedAccuracy();
            CalculateMagicAccuracy();
            CalculateEvade();
          //  CalculateInitialNextTurn();
            CalculateSpeedRating();
        }

        private void CalculateHp()
        {
            Hp = Brawn * 4 + Will / 2;
        }

        private void CalculateMp()
        {
            Mp = Will * 2 + Cunning / 4;
        }

        private void CalculateSpeed()
        {
            Speed = (Cunning + Agility + Perception) / 3;
        }

        private void CalculateMove()
        {
            Move = 2 * (Brawn + Agility) / 5;
        }

        private void CalculateJump()
        {
            Jump = Agility / 25;
        }

        private void CalculateMeleeAccuracy()
        {
            MeleeAccuracy = (Agility + Cunning) / 2;
        }

        private void CalculateRangedAccuracy()
        {
            RangedAccuracy = (Perception + Cunning) / 2;
        }

        private void CalculateMagicAccuracy()
        {
            MagicAccuracy = (Will + Cunning) / 2;
        }

        private void CalculateEvade()
        {
            Evade = (Perception + Cunning + Agility) / 3;
        }

        // private void CalculateInitialNextTurn()
        // {
        //     NextTurn = 100 - Speed;
        // }

        private void CalculateSpeedRating()
        {
            speedRating = Speed + Agility + Cunning + Will;
        }
    }
}