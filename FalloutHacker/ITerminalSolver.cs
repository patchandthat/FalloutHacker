using System.Collections.Immutable;

namespace FalloutHacker
{
    public interface ITerminalSolver
    {
        string BestGuess { get; }

        void AddPassword(string password);
        void EliminatePassword(string guess, int likeness);

        IImmutableSet<string> PossiblePasswords { get; }
        IImmutableSet<string> EliminatedPasswords { get; }
    }
}