using System.Collections.Generic;

namespace FalloutHacker
{
    public class TerminalData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:FalloutHacker.TerminalData"/> class.
        /// </summary>
        public TerminalData(IEnumerable<string> lines, IEnumerable<string> words)
        {
            Lines = lines;
            Words = words;
        }

        /// <summary>
        /// Identified words on the terminal
        /// </summary>
        public IEnumerable<string> Words { get; }

        /// <summary>
        /// Raw lines of input from the terminal, to identify pairs of brackets
        /// </summary>
        public IEnumerable<string> Lines { get; } 
    }
}