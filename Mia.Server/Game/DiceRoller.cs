using System;


namespace Mia.Server
{
    public class DiceRoller
    {
        public int RollOneDie()
        {
            return new Random().Next(1, 6);
        }

        public Dice Roll()
        {
            return new Dice(RollOneDie(), RollOneDie());
        }
    }
}
