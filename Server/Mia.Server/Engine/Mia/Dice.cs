using System;
using System.Linq;

using Game.Server.Engine.Mia.Interface;


namespace Game.Server.Engine.Mia
{
    public class Dice : IDice
    {
        #region Members

        private int[] dice = new int[2];

        #endregion Members


        #region Properties

        public int DiceOne
        {
            get
            {
                return dice[0];
            }
        }

        public int DiceTwo
        {
            get
            {
                return dice[1];
            }
        }

        public bool IsDouble
        {
            get
            {
                return DiceOne == DiceTwo;
            }
        }

        public bool IsMia
        {
            get
            {
                return DiceOne == 2 && DiceTwo == 1;
            }
        }

        #endregion Properties


        #region Constructor

        public Dice()
        {
            Shake();
        }

        public Dice(int diceOne, int diceTwo)
        {
            SetDices(diceOne, diceTwo);
        }

        #endregion Constructor


        #region Method

        private void SetDices(int diceOne, int diceTwo)
        {
            dice[0] = diceOne;
            dice[1] = diceTwo;

            dice = dice.OrderByDescending(d => d).ToArray();
        }

        private int Scoring(IDice diceToCompare)
        {
            int value = 10 * diceToCompare.DiceOne + diceToCompare.DiceTwo;
            value += diceToCompare.IsDouble ? 100 : 0;
            value += diceToCompare.IsMia ? 200 : 0;

            return value;
        }

        public bool IsHigherThan(IDice otherDice)
        {
            return Scoring(this) >= Scoring(otherDice);
        }

        public override string ToString()
        {
            return DiceOne.ToString() + DiceTwo;
        }

        public IDice Parse(string value)
        {
            string[] diceValues = value.Split(',');

            int dieOne;
            int dieTwo;

            int.TryParse(diceValues[0], out dieOne);
            int.TryParse(diceValues[1], out dieTwo);

            if (dieOne > 0 && dieTwo > 0)
                return new Dice(dieOne, dieTwo);

            return null;
        }

        public void Shake()
        {
            int diceOne = new Random().Next(1, 6);
            int diceTwo = new Random().Next(1, 6);

            SetDices(diceOne, diceTwo);
        }

        #endregion Methods
    }
}
