using System;
using System.Linq;


namespace Mia.Server
{
    public class Dice
    {
        #region Members

        private int[] dies = new int[2];

        #endregion Members


        #region Properties

        public int DiceOne
        { 
            get 
            {
                return dies[0];
            }
        }

        public int DiceTwo
        {
            get
            {
                return dies[1];
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

        public Dice(int dieOne, int dieTwo)
        {
            dies[0] = dieOne;
            dies[1] = dieTwo;

            dies.OrderByDescending(d => d).ToArray();
        }

        #endregion Constructor


        #region Method

        public bool Equal(Dice otherDice)
        { 
            if (otherDice == null)
                return false;

            return DiceOne == otherDice.DiceOne && DiceTwo == otherDice.DiceTwo;
        }

        public int ValueForOrdering()
        {
            int value = 10 * DiceOne + DiceTwo;
            value += IsDouble ? 100 : 0;
            value += IsMia ? 200 : 0;

            return value;
        }

        public bool IsHigherThan(Dice otherDice)
        {
            return ValueForOrdering() >= otherDice.ValueForOrdering();
        }

        public Dice Create(int dieOne, int dieTwo)
        {
            return new Dice(dieOne, dieTwo);
        }

        public Dice Parse(string values)
        {
            string[] diceValues = values.Split(',');
            int dieOne = int.Parse(diceValues[0]);
            int dieTwo = int.Parse(diceValues[1]);

            if (dieOne > 0 && dieTwo > 0)
                return new Dice(dieOne, dieTwo);

            return null;
        }

        public override string ToString()
        {
            return DiceOne.ToString() + DiceTwo;
        }

        #endregion Methods
    }
}
