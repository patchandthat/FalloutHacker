using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FalloutHacker
{
    public class TerminalSolver : ITerminalSolver
    {
        private int _passwordLength = -1;

        private readonly HashSet<string> _possiblePasswords = new HashSet<string>();
        private readonly HashSet<string> _eliminatedPasswords = new HashSet<string>();
        private string _cachedGuess;

        public string BestGuess => GuessPassword();

        private class LikenessInfo
        {
            public int Likeness { get; set; }
            public int PasswordsSimilarTo { get; set; }
        }

        private string GuessPassword()
        {
            if (!string.IsNullOrEmpty(_cachedGuess))
            {
                return _cachedGuess;
            }

            int availablePasses = _possiblePasswords.Count;

            if (availablePasses == 0)
                return null;

            if (availablePasses == 1 || availablePasses == 2)
                return _possiblePasswords.First();

            //Likeness => Password, <#likeness, #matches>
            var likenessDict = new Dictionary<string, List<LikenessInfo>>();
            foreach (string pass in _possiblePasswords)
            {
                likenessDict.Add(pass, new List<LikenessInfo>());

                //compare to each other password
                //work out how many passwords it has each likeness with
                //ie. 3 likeness with 1 other password, 2 likeness with 3 other passwords, 1 likeness with 4 other passwords
                foreach (string otherPass in _possiblePasswords)
                {
                    if (pass == otherPass) continue;

                    int likeness = 0;
                    for (int i = 0; i < pass.Length; i++)
                    {
                        if (pass[i] == otherPass[i]) likeness++;
                    }

                    var info = likenessDict[pass].FirstOrDefault(x => x.Likeness == likeness);
                    if (info == null)
                    {
                        likenessDict[pass].Add(new LikenessInfo
                        {
                            Likeness = likeness,
                            PasswordsSimilarTo = 1
                        });
                    }
                    else
                    {
                        info.PasswordsSimilarTo++;
                    }
                }
            }

            //Find the highest likeness
            //Of the passwords that have this likeness, find out the one that has the likeness with the most passwords
            int highestLikeness = 0;
            foreach (List<LikenessInfo> likenessInfos in likenessDict.Values)
            {
                int likeness = likenessInfos.Max(x => x.Likeness);
                if (likeness > highestLikeness)
                    highestLikeness = likeness;
            }

            int highestCommonality = 0;
            string guess = "";
            foreach (var pair in likenessDict)
            {
                string pass = pair.Key;
                List<LikenessInfo> likenesses = pair.Value;
                var info = likenesses.FirstOrDefault(x => x.Likeness == highestLikeness);
                if (info != null)
                {
                    if (info.PasswordsSimilarTo > highestCommonality)
                    {
                        highestCommonality = info.PasswordsSimilarTo;
                        guess = pass;
                    }
                }
            }

            //cache the result until passwords added or elimiated to prevent reevaluation
            _cachedGuess = guess;

            return guess;
        }

        public void AddPassword(string password)
        {
            password = password.ToUpper();

            if (_passwordLength == -1)
                _passwordLength = password.Length;

            if (_passwordLength != password.Length)
                throw new InvalidPasswordException("All passwords must be the same length");

            _possiblePasswords.Add(password);
            _cachedGuess = "";
        }

        public void EliminatePassword(string guess, int likeness)
        {
            if (!_possiblePasswords.Contains(guess))
                throw new InvalidPasswordException("Guessed password is not a possible password");

            _possiblePasswords.Remove(guess);
            _eliminatedPasswords.Add(guess);

            //Find all passwords with the given likness to the guess and erliminate them
            var toRemove = new List<string>();
            foreach (var pass in _possiblePasswords)
            {
                int thisLikeness = 0;
                for (int i = 0; i < pass.Length; i++)
                {
                    if (pass[i] == guess[i]) thisLikeness++;
                }

                if (thisLikeness != likeness)
                    toRemove.Add(pass);
            }

            foreach (string deadPass in toRemove)
            {
                _eliminatedPasswords.Add(deadPass);
                _possiblePasswords.Remove(deadPass);
            }

            _cachedGuess = "";
        }

        public IImmutableSet<string> PossiblePasswords => ImmutableHashSet.Create(_possiblePasswords.ToArray());
        public IImmutableSet<string> EliminatedPasswords => ImmutableHashSet.Create(_eliminatedPasswords.ToArray());
    }

    public class InvalidPasswordException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class.
        /// </summary>
        public InvalidPasswordException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error. </param>
        public InvalidPasswordException(string message) : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception. </param><param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified. </param>
        public InvalidPasswordException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
