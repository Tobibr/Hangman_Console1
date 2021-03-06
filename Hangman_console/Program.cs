using System;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

class MainClass
{
    public static void Main(string[] args)
    {

        do
        {
            Console.WriteLine("Welcome to the Hangman Game!\n");

            string path = @"countries_and_capitals.txt";

            StreamWriter sw;
            StreamReader sr;
            string capitalWord = "";
            var line = "";
            // read words from file
            if (!File.Exists(path))
            {
                Console.WriteLine("The file with words do not exist! ");
                Console.WriteLine("Copy 'countries_and_capitals.txt' to .. \\Hangman_console\\bin\\Debug\\net5.0\\");
                break;
            }
            else
            {
                //rondom line from a file
                sr = File.OpenText(path);
                var lines = File.ReadAllLines(path);
                var r = new Random();
                var randomLineNumber = r.Next(0, lines.Length - 1);
                line = lines[randomLineNumber];
            }

            // split the line to a country and a captital
            string myText = line;
            string[] words = myText.Split(" | ");
            capitalWord = words[1].ToUpper();
            string countryWord = words[0];

            int lives = 5;
            int counter = -1;
            int trials = 0;
            string again;
            int wordLength = capitalWord.Length;
            char[] secretArray = capitalWord.ToCharArray();
            char[] printArray = new char[wordLength];
            char[] guessedLetters = new char[26];
            int numberStore = 0;
            bool victory = false;

            foreach (char letter in printArray)
            {
                counter++;

                printArray[counter] = '-';
            }

            Stopwatch stopWatch = new Stopwatch();

            while (lives > 0)
            {
                stopWatch.Start();
                counter = -1;
                trials++;
                string printProgress = String.Concat(printArray);
                bool letterFound = false;
                int multiples = 0;

                if (printProgress == capitalWord)
                {
                    victory = true;
                    break;
                }

                Hint(countryWord, lives, guessedLetters, printProgress);

                Console.WriteLine(GallowView(lives));
                Console.WriteLine("current progress: " + printProgress);
                Console.Write("\n");
                Console.WriteLine("You already used letters:");
                Console.WriteLine(guessedLetters);
                Console.Write("\n");

                do
                {
                    Console.WriteLine("Do you want guess a letter or a whole word? (L/W)");
                    ConsoleKeyInfo keyPress = Console.ReadKey();
                    again = keyPress.Key.ToString();

                    if (again.ToUpper() != "L" && again.ToUpper() != "W")
                    {
                        Console.WriteLine("Invalid response, please enter L or W");
                        continue;
                    }
                    else
                    {
                        break;
                    }

                } while (true);

                if (again.ToUpper() == "L")
                {
                    GuessedLetter(ref lives, ref counter, secretArray, printArray, guessedLetters, ref numberStore, ref letterFound, ref multiples);
                }

                else
                {
                    Console.Write("\nGuess a word: ");

                    string playerGuess = Console.ReadLine();


                    if (playerGuess.ToUpper() == capitalWord)
                    {

                        victory = true;
                        break;
                    }
                    else
                    {
                        lives = lives - 2;
                        Console.Clear();
                    }
                }

                stopWatch.Stop();

            }

            TimeSpan ts = stopWatch.Elapsed;
            int seconds = (ts.Minutes * 60) + ts.Seconds;

            string pathScore = @"score.txt";
            if (victory)
            {
                Console.WriteLine("\n\nThe capital was: {0}", capitalWord);
                Console.WriteLine("\n\n----- YOU WIN! -----\n");
                Console.WriteLine($"You guessed the capital after {trials} letters. It took you {seconds} seconds\n");
                Console.WriteLine("What is your name?");
                string playerName = Console.ReadLine();

                sw = SaveScoreToFile(capitalWord, trials, pathScore, playerName);

            }
            else
            {
                Console.WriteLine("\n\nThe capital was: {0}", capitalWord);
                Console.WriteLine("\n\n   YOU LOSE!\n");
                Console.WriteLine(GallowView(lives));
            }

            TenTopScores(pathScore);

            do
            {
                Console.WriteLine("\n----- Play again? (Y/N) -----");
                ConsoleKeyInfo keyPress = Console.ReadKey();
                again = keyPress.Key.ToString();

                if (again.ToUpper() != "Y" && again.ToUpper() != "N")
                {
                    Console.WriteLine("Invalid response, please enter Y or N");
                    continue;
                }
                else
                {
                    break;
                }

            } while (true);

            if (again.ToUpper() == "Y")
            {
                Console.WriteLine();
                Console.Clear();
                continue;
            }
            else
            {
                break;
            }

        } while (true);
    }

    private static void Hint(string countryWord, int lives, char[] guessedLetters, string printProgress)
    {
        if (lives > 1)
        {
            Console.WriteLine($"You have {lives} lives!");
        }
        else
        {
            Console.WriteLine($"You only have {lives} life left!!");
            Console.WriteLine("\n--------------- HINT! ---------------\n");
            Console.WriteLine("What is the capital of {0} ?\n", countryWord);
        }
    }

    private static void GuessedLetter(ref int lives, ref int counter, char[] secretArray, char[] printArray, char[] guessedLetters, ref int numberStore, ref bool letterFound, ref int multiples)
    {
        Console.Write("\nGuess a letter: ");
        string playerGuess = Console.ReadLine();

        //test to make sure a single letter
        bool guessTest = playerGuess.All(Char.IsLetter);

        while (guessTest == false || playerGuess.Length != 1)
        {
            Console.WriteLine("Please enter only a single letter!");
            Console.Write("Guess a letter: ");
            playerGuess = Console.ReadLine();
            guessTest = playerGuess.All(Char.IsLetter);
        }
        Console.Clear();
        playerGuess = playerGuess.ToUpper();
        char playerChar = Convert.ToChar(playerGuess);

        if (guessedLetters.Contains(playerChar) == false)
        {

            guessedLetters[numberStore] = playerChar;
            numberStore++;

            foreach (char letter in secretArray)
            {

                counter++;
                if (letter == playerChar)
                {
                    printArray[counter] = playerChar;
                    letterFound = true;
                    multiples++;
                }

            }

            if (letterFound)
            {
                Console.WriteLine("Found {0} letter {1}!", multiples, playerChar);
            }
            else
            {
                Console.WriteLine("No letter {0}!", playerChar);
                lives--;
            }

        }
        else
        {
            Console.WriteLine("You already guessed {0}!!", playerChar);
        }
    }

    private static StreamWriter SaveScoreToFile(string capitalWord, int trials, string pathScore, string playerName)
    {
        StreamWriter sw;
        // Save scores to a file

        if (!File.Exists(pathScore))
        {
            sw = File.CreateText(pathScore);
        }
        else
        {
            sw = new StreamWriter(pathScore, true);
        }

        if (trials < 10)
        {
            string tekst = $"0{trials}   | {playerName} | {DateTime.Now} | {capitalWord}";
            sw.WriteLine(tekst);
        }
        else
        {
            string tekst2 = $"{trials}   | {playerName} | {DateTime.Now} | {capitalWord}";   
            sw.WriteLine(tekst2);
        } 
        sw.Close();
        return sw;
    }

    private static void TenTopScores(string pathScore)
    {
        string pathTopScore = @"TopScore.txt";

        List<string> linesTop = new List<string>(File.ReadAllLines(pathScore));
        linesTop.Sort();

        File.WriteAllLines(pathTopScore, linesTop);
        Console.WriteLine("\n  - - - - - - - - -    Top Ten Score     - - - - - - - - -");
        Console.WriteLine("\n  trials |   name  |        data         | guessed capital ");

        int i = 1;
        foreach (var lineTop in linesTop)
        {
            Console.WriteLine($"{i}.  {lineTop}");
            i++;
            if (i > 10)
            {
                break;
            }
        }
    }

    private static string GallowView(int livesLeft)
    {
        //simple function to print out the hangman

        string drawHangman = "";

        if (livesLeft == 5)
        {
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
        }

        if (livesLeft == 4)
        {
            drawHangman += "   ========\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
        }

        if (livesLeft == 3)
        {
            drawHangman += "   ========\n";
            drawHangman += "   ||     |\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
        }

        if (livesLeft == 2)
        {
            drawHangman += "   ========\n";
            drawHangman += "   ||     |\n";
            drawHangman += "   ||     O\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
        }

        if (livesLeft == 1)
        {
            drawHangman += "   ========\n";
            drawHangman += "   ||     |\n";
            drawHangman += "   ||     O\n";
            drawHangman += "   ||    /|\\ \n";
            drawHangman += "   ||\n";
            drawHangman += "   ||\n";
        }

        if (livesLeft == 0)
        {
            drawHangman += "   ========\n";
            drawHangman += "   ||     |\n";
            drawHangman += "   ||     O\n";
            drawHangman += "   ||    /|\\ \n";
            drawHangman += "   ||    / \\ \n";
            drawHangman += "   ||\n";
        }

        return drawHangman;

    }

}