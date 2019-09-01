using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawingTabletServer
{
    public class ConsoleUi
    {
        /// <summary>
        /// Creates a Selection Grid for based on the string values Array
        /// </summary>
        /// <param name="instructions">Printed before the selection grid</param>
        /// <param name="values">List of values to be choosen from</param>
        /// <param name="clearConsole">Console Clear command after selection</param>
        /// <returns>Choosen value</returns>
        public static string CreateSelectionGridForArray(string instructions, IEnumerable<string> values, bool clearConsole = true)
        {
            Console.WriteLine(instructions);
            (int Left, int Top) startPos = (Console.CursorLeft, Console.CursorTop);
            var indicesOfEnumNames = new List<((int Left, int Top) pos, string selection)>();
            var maxLengthName = values.Max(x => x.Length) + 2;
            foreach (var name in values)
            {
                if (maxLengthName + 2 + Console.CursorLeft > Console.WindowWidth)
                    Console.WriteLine();
                indicesOfEnumNames.Add(
                    ((Console.CursorLeft, Console.CursorTop), name));
                Console.Write(name.PadRight(maxLengthName, ' '));
            }

            var updownValue = indicesOfEnumNames.Count(x => x.pos.Top == indicesOfEnumNames.First().pos.Top);

            bool selected;
            int current = 0;
            SetConsoleCursor(startPos);
            do
            {
                var pressedKey = Console.ReadKey(true);
                selected = pressedKey.Key == ConsoleKey.Enter || pressedKey.Key == ConsoleKey.Select;

                switch (pressedKey.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (current - 1 < 0)
                            current = indicesOfEnumNames.Count - 1;
                        else
                            current--;
                        SetConsoleCursor(indicesOfEnumNames[current].pos);
                        break;
                    case ConsoleKey.UpArrow:
                        if (current - updownValue < 0)
                            current = 0;
                        else
                            current -= updownValue;
                        SetConsoleCursor(indicesOfEnumNames[current].pos);
                        break;
                    case ConsoleKey.RightArrow:
                        if (current + 1 >= indicesOfEnumNames.Count)
                            current = indicesOfEnumNames.Count - 1;
                        else
                            current++;
                        SetConsoleCursor(indicesOfEnumNames[current].pos);
                        break;
                    case ConsoleKey.DownArrow:
                        if (current + updownValue >= indicesOfEnumNames.Count)
                            current = indicesOfEnumNames.Count - 1;
                        else
                            current += updownValue;
                        SetConsoleCursor(indicesOfEnumNames[current].pos);
                        break;
                    default:
                        break;
                }
            } while (!selected);
            if (clearConsole)
                Console.Clear();
            Console.CursorTop = indicesOfEnumNames.Max(x => x.pos.Top);
            Console.WriteLine();
            return indicesOfEnumNames[current].selection;
        }


        /// <summary>
        /// Set the Console Cursor Position
        /// </summary>
        /// <param name="positon">Tuple of the position</param>
        private static void SetConsoleCursor((int Left, int Top) positon)
        {
            Console.CursorLeft = positon.Left;
            Console.CursorTop = positon.Top;
        }
    }
}
