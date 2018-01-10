using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Manzanita4
{
    public interface IOutputTools
    {
        int TailWidth();
        void Divert(string verb, string noun);
        void UnDivert();
        void AppendLevel(double level);
    }

    public class ConsoleOutputTools: IOutputTools
    {
        struct Diversion {
            internal string Verb;
            internal string Noun;
        }

        private readonly List<Diversion> _diversions;
        private bool _shownDiversion;

        public int ScreenWidth() => 80;
        public int TailPrefix() => 60;
        public int TailWidth() => ScreenWidth() - TailPrefix();

        public ConsoleOutputTools()
        {
            _diversions = new List<Diversion>();
            _shownDiversion = false;
        }

        public void Divert(string verb, string noun)
        {
            Diversion d = new Diversion {Verb = verb, Noun = noun};
            _diversions.Add(d);
            InitiallyPresentDiversion(d);
            _shownDiversion = false;

        }

        public void UnDivert()
        {
            _diversions.RemoveAt(_diversions.Count - 1);
            Console.WriteLine();
            _shownDiversion = false;
        }

        private void InitiallyPresentDiversion(Diversion d)
        {
            var oldFg = Console.ForegroundColor;
            Console.Write(new string(' ', (_diversions.Count - 1) * 2));
            try
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("[Task: ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(d.Verb);
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(d.Noun);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write("]");
                Console.WriteLine();
            }
            finally
            {
                Console.ForegroundColor = oldFg;
            }
        }

        private void ShowDiversion(Diversion d)
        {
            var oldFg = Console.ForegroundColor;
            Console.Write(new string(' ', (_diversions.Count - 1) * 2 + 1));
            try
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(d.Verb);
                Console.Write(" ");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write(d.Noun);
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(": ");

                /*
                // put the character readout on a new line
                Console.WriteLine();
                Console.Write(new string(' ', TailPrefix()));
                */
            }
            finally
            {
                Console.ForegroundColor = oldFg;
            }
        }

        public void AppendLevel(double level) // level is from 0.0 to 1.0. greater than 1.0 is a clip glyph
        {
            if (!_shownDiversion)
            {
                ShowDiversion(_diversions[_diversions.Count - 1]);
                _shownDiversion = true;
            }
            var oldFg = Console.ForegroundColor;
            try
            {
                if (level > 1.0)
                {
                    Console.ForegroundColor = ConsoleColor.Red; // signifies clipping
                    Console.Write("!");
                }
                else if (level > 0.8888)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow; 
                    Console.Write("#");
                }
                else if (level > 0.7777)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Write("#");
                }
                else if (level > 0.6666)
                {
                    Console.ForegroundColor = ConsoleColor.White; 
                    Console.Write("=");
                }
                else if (level > 0.5555)
                {
                    Console.ForegroundColor = ConsoleColor.White; 
                    Console.Write("+");
                }
                else if (level > 0.4444)
                {
                    Console.ForegroundColor = ConsoleColor.White; 
                    Console.Write("-");
                }
                else if (level > 0.3333)
                {
                    Console.ForegroundColor = ConsoleColor.Gray; 
                    Console.Write("-");
                }
                else if (level > 0.2222)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray; 
                    Console.Write("-");
                }
                else if (level > 0.1111)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray; 
                    Console.Write("~");
                }
                else 
                {
                    Console.ForegroundColor = ConsoleColor.DarkBlue;
                    Console.Write(".");
                }
            }
            finally
            {
                Console.ForegroundColor = oldFg;
            }
        }
    }
}