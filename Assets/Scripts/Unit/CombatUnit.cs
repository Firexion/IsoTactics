
using System;

namespace Unit
{
    [Serializable]
    public abstract class CombatUnit
    {
        public int id;
        public int speedRating;
        public string name;
        public bool isRightHanded;

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

        protected CombatUnit()
        {
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