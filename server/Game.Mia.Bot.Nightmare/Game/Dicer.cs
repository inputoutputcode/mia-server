using System;
using System.Data;
using System.Linq;


namespace Game.Mia.Bot.Nightmare.Game
{
    internal class Dicer
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

        public Dicer(int diceOne, int diceTwo)
        {
            SetDices(diceOne, diceTwo);
            
            if(!Validate())
            {
                dice[0] = 0;
                dice[1] = 0;
            }
        }

        #endregion


        #region Method

        private void SetDices(int diceOne, int diceTwo)
        {
            dice[0] = diceOne;
            dice[1] = diceTwo;

            dice = dice.OrderByDescending(d => d).ToArray();
        }

        public static Dicer Beat(Dicer dicer, bool randomize = false)
        {
            Dicer returnDicer = null;
            var randomizer = new Random();

            if (dicer != null)
            {
                if (!dicer.IsMia)
                {
                    if (randomize)
                    {
                        int nextDiceOption = randomizer.Next(1, 3);

                        switch (nextDiceOption)
                        {
                            case 1:
                                // TODO: Do not announce mia, roll again
                                returnDicer = new Dicer(2, 1);
                                break;
                            case 2:
                                // TODO: increase by one is not a good strategy
                                if (dicer.DiceOne == 6)
                                    returnDicer = new Dicer(2, 1);
                                else
                                    returnDicer = new Dicer(dicer.DiceOne + 1, dicer.DiceOne + 1);
                                break;
                            case 3:
                                returnDicer = BeatByOne(dicer);
                                break;
                        }
                    }
                    else
                    {
                        returnDicer = BeatByOne(dicer);
                    }
                }
            }
            else
            {
                // Only for the case of first dice if bot is faking first rolled dice
                int randomNumberOne = randomizer.Next(1, 6);
                int randomNumberTwo = randomizer.Next(1, 6);
                returnDicer = BeatByOne(new Dicer(randomNumberOne, randomNumberTwo));
            }
            

            return returnDicer;
        }

        public static Dicer BeatByOne(Dicer dicer)
        {
            Dicer returnDicer = null;

            if (dicer.IsDouble && dicer.DiceOne == 6)
            {
                returnDicer = new Dicer(1, 2);
            }
            if (dicer.IsDouble)
            {
                returnDicer = new Dicer(dicer.DiceOne + 1, dicer.DiceOne + 1);
            }
            else
            {
                if (dicer.DiceOne == 6 && dicer.DiceTwo == 5)
                {
                    returnDicer = new Dicer(1, 1);
                }
                else
                {
                    if (dicer.DiceTwo == dicer.DiceOne - 1)
                        returnDicer = new Dicer(dicer.DiceOne + 1, 1);
                    else
                        returnDicer = new Dicer(dicer.DiceOne, dicer.DiceTwo + 1);
                }
            }

            return returnDicer;
        }

        public override string ToString()
        {
            return DiceOne.ToString() + DiceTwo;
        }

        public static Dicer Parse(string value)
        {
            Dicer returnDicer = null;
            int dieOne;
            int dieTwo;

            var diceValues = value.ToCharArray();
            if (diceValues.Length > 1)
            {
                int.TryParse(diceValues[0].ToString(), out dieOne);
                int.TryParse(diceValues[1].ToString(), out dieTwo);

                if (dieOne > 0 && dieOne < 7 && dieTwo > 0 && dieTwo < 7)
                    returnDicer = new Dicer(dieOne, dieTwo);
            }

            return returnDicer;
        }

        public bool Validate()
        {
            return (dice[0] > 0 && dice[0] < 7 && dice[1] > 0 && dice[1] < 7);
        }

        public int CompareTo(Dicer dicer)
        {
            int compareResult = 0;

            if (dicer.IsMia)
            {
                compareResult = 1;
            }
            else if (dicer.IsDouble)
            {
                if (dice[0] < dicer.DiceOne)
                    compareResult = 1;
                else
                    compareResult = -1;
            }
            else if (dice[0] <= dicer.DiceOne && dice[1] < dicer.DiceTwo)
            { 
                compareResult = 1;
            }
            else if (dice[0] >= dicer.DiceOne && dice[1] > dicer.DiceTwo)
            {
                compareResult = -1;
            }

            return compareResult;
        }

        #endregion
    }
}
