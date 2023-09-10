﻿using System;
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

        public bool IsValid
        {
            get
            {
                int[] validNumbers = new int[] { 21, 66, 55, 44, 33, 22, 11, 65, 64, 63, 62, 61, 54, 53, 52, 51, 43, 42, 41, 32, 31 };
                return validNumbers.Contains(int.Parse(ToString()));
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
            dice[0] = diceOne;
            dice[1] = diceTwo;
        }

        #endregion Constructor


        #region Method

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
            int dieOne;
            int dieTwo;
            Dice returnDice = null;

            var diceValues = value.ToCharArray();
            if (diceValues.Length == 2) 
            {
                int.TryParse(diceValues[0].ToString(), out dieOne);
                int.TryParse(diceValues[1].ToString(), out dieTwo);

                if (dieOne > 0 && dieTwo > 0)
                    returnDice = new Dice(dieOne, dieTwo);
            }

            return returnDice;
        }

        public void Shake()
        {
            int diceOne = new Random().Next(1, 6);
            int diceTwo = new Random().Next(1, 6);

            dice[0] = diceOne;
            dice[1] = diceTwo;

            dice = GetOrdered();
        }

        public virtual int[] GetOrdered()
        {
            var newDice = new int[2] { dice[0], dice[1] };
            return newDice.OrderByDescending(d => d).ToArray();
        }

        #endregion Methods
    }
}
