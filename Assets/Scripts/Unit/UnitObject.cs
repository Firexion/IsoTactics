using UnityEngine;

namespace Unit
{
    public abstract class UnitObject : ScriptableObject
    {
        public int id;
        public new string name;

        public int brawn;
        public int agility;
        public int perception;
        public int cunning;
        public int will;

        public int Hp { get; set; }
        public int Mp { get; set; }
        public int Speed { get; private set; }
        public int Move { get; private set; }
        public int Jump { get; private set; }

        public int MeleeAccuracy { get; private set; }
        public int RangedAccuracy { get; private set; }
        public int MagicAccuracy { get; private set; }
        public int Evade { get; private set; }
        public int SpeedRating { get; private set; }

        private void OnEnable()
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
            CalculateSpeedRating();
        }

        private void CalculateHp()
        {
            Hp = brawn * 4 + will / 2;
        }

        private void CalculateMp()
        {
            Mp = will * 2 + cunning / 4;
        }

        private void CalculateSpeed()
        {
            Speed = (cunning + agility + perception) / 3;
        }

        private void CalculateMove()
        {
            Move = 2 * (brawn + agility) / 5;
        }

        private void CalculateJump()
        {
            Jump = agility / 25;
        }

        private void CalculateMeleeAccuracy()
        {
            MeleeAccuracy = (agility + cunning) / 2;
        }

        private void CalculateRangedAccuracy()
        {
            RangedAccuracy = (perception + cunning) / 2;
        }

        private void CalculateMagicAccuracy()
        {
            MagicAccuracy = (will + cunning) / 2;
        }

        private void CalculateEvade()
        {
            Evade = (perception + cunning + agility) / 3;
        }

        private void CalculateSpeedRating()
        {
            SpeedRating = Speed + agility + cunning + will;
        }
    }
}