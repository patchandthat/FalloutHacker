using System;
using System.Linq;
using Ploeh.AutoFixture;
using Xunit;

namespace FalloutHacker.Tests
{
    public class TerminalSolverTests
    {
        public class DefaultState
        {
            private readonly ITerminalSolver _sut = new TerminalSolver();

            [Fact]
            public void PossiblePasswordsIsEmptyList()
            {
                Assert.Empty(_sut.PossiblePasswords);
            }

            [Fact]
            public void EliminatedPasswordsIsEmptyList()
            {
                Assert.Empty(_sut.EliminatedPasswords);
            }

            [Fact]
            public void BestGuessIsNull()
            {
                Assert.Null(_sut.BestGuess);
            }
        }

        public class AddingPasswords
        {
            private readonly ITerminalSolver _sut = new TerminalSolver();
            private readonly Fixture _fixture = new Fixture();

            [Fact]
            public void GivenPasswordAddedThenPossiblePasswordsContainsNewItem()
            {
                var pass = _fixture.Create<string>();
                _sut.AddPassword(pass);

                Assert.Contains(pass, _sut.PossiblePasswords);
                Assert.Equal(1, _sut.PossiblePasswords.Count);
            }

            [Fact]
            public void GivenPasswordAddedToCollectionThenBestGuessIsNotNull()
            {
                var pass = _fixture.Create<string>();
                _sut.AddPassword(pass);

                Assert.NotNull(_sut.BestGuess);
            }

            [Fact]
            public void GivenOnlyOnePossiblePasswordBestGuessIsOnlyPossibleOption()
            {
                var pass = _fixture.Create<string>();
                _sut.AddPassword(pass);

                Assert.Equal(1, _sut.PossiblePasswords.Count);
                Assert.Equal(pass, _sut.BestGuess);
            }

            [Fact]
            public void PossiblePasswordsOnlyContainsUniqueEntries()
            {
                var pass = _fixture.Create<string>();

                _sut.AddPassword(pass);
                _sut.AddPassword(pass);

                Assert.Equal(1, _sut.PossiblePasswords.Count);
                Assert.Equal(pass, _sut.PossiblePasswords.First());
            }

            [Fact]
            public void AddingPasswordsOfDifferentLengthsToPreviousPasswordsThrowsInvalidPasswordException()
            {
                _sut.AddPassword("one");

                Exception ex = Assert.Throws<InvalidPasswordException>(() => _sut.AddPassword("Another"));
            }
        }

        public class EliminatingPasswords
        {
            private readonly ITerminalSolver _sut = new TerminalSolver();
            private readonly Fixture _fixture = new Fixture();

            [Fact]
            public void EliminatingPasswordThatIsNotAPossiblePasswordThrowsInvalidPasswordException()
            {
                _sut.AddPassword(_fixture.Create<string>());

                Exception ex = Assert.Throws<InvalidPasswordException>(() => _sut.EliminatePassword("Another", 0));
            }

            [Fact]
            public void EliminatingPasswordRemovesFromPossibleCollection()
            {
                var pass = _fixture.Create<string>();

                _sut.AddPassword(pass);
                _sut.EliminatePassword(pass, 0);

                Assert.Empty(_sut.PossiblePasswords);
            }

            [Fact]
            public void EliminatingPasswordAddsToEliminatedCollection()
            {
                var pass = _fixture.Create<string>();

                _sut.AddPassword(pass);
                _sut.EliminatePassword(pass, 0);

                Assert.Equal(1, _sut.EliminatedPasswords.Count);
                Assert.Contains(pass, _sut.EliminatedPasswords);
            }

            [Fact]
            public void EliminatedPasswordIsNotSuggestedAsBestGuess()
            {
                _sut.AddPassword("One");
                _sut.AddPassword("Two");

                Assert.Equal("One", _sut.BestGuess);
                _sut.EliminatePassword("One", 0);
                Assert.Equal("Two", _sut.BestGuess);
            }

            [Fact]
            public void EliminatingWithLikeness()
            {
                Assert.True(false);
            }
        }

        public class BestGuess
        {
            private readonly ITerminalSolver _sut = new TerminalSolver();

            [Fact]
            public void GivenNoPasswordOptionsThenBestGuessIsNull()
            {
                Assert.Equal(0, _sut.PossiblePasswords.Count);
                Assert.Null(_sut.BestGuess);
            }
            
            [Fact]
            public void GivenOnlyOnePasswordthenBestGuessIsPassword()
            {
                _sut.AddPassword("pass");

                Assert.Equal(1, _sut.PossiblePasswords.Count);
                Assert.Equal("pass", _sut.BestGuess);
            }

            [Fact]
            public void GivenTwoPossiblePasswordsThenBestGuessIsFirst()
            {
                _sut.AddPassword("ONE");
                _sut.AddPassword("TWO");

                Assert.Equal(3, _sut.PossiblePasswords.Count);
                Assert.Equal("ONE", _sut.BestGuess);
            }

            [Fact]
            public void GivenMultiplePossibilitiesThenBestGuessHasMostCommonality()
            {
                _sut.AddPassword("AAA");
                _sut.AddPassword("FAT");
                _sut.AddPassword("CAT");
                _sut.AddPassword("CAN");

                Assert.Equal(4, _sut.PossiblePasswords.Count);
                Assert.Equal("CAT", _sut.BestGuess);
            }
        }
    }
}
