namespace Game.Server.Engine.Mia.Interface
{
    public interface IDice
    {
        int DiceOne { get; }

        int DiceTwo { get; }

        bool IsDouble { get; }

        bool IsMia { get; }

        bool IsHigherThan(IDice otherDice);

        IDice Parse(string value);

        void Shake();
    }
}
