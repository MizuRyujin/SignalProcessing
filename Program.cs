﻿using System;
using System.IO;
using System.Collections.Generic;

namespace CSVReader
{
    public class Program
    {
        private static void Main(string[] args)
        {
            string projectPath = @"D:\Miguel\Documents\Projetos\CSVReader\";

            string csvPath = string.Concat(projectPath, "scr.csv");
            string resultPath;
            string answer;
            bool validInput = false;
            int window;


            SignalProcessing _signalProcesser = new SignalProcessing(csvPath);

            Console.WriteLine("There are {0} values to process",
                                    _signalProcesser.XValues.Length);
            Console.Write("\nInput the search window: ");

            do
            {
                answer = Console.ReadLine();
                if (Int32.TryParse(answer, out window))
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("Not a valid input...");
                }
            } while (!validInput);

            Console.WriteLine("Press a key to find peaks...");
            Console.ReadKey();

            _signalProcesser.FindPeaks(window);

            Console.WriteLine("There are {0} peaks, with a window range of {1}",
                                _signalProcesser.ListOfMaximums.Count, window);

            Console.ReadKey();

            foreach (Tuple<float, float> maximum in _signalProcesser.ListOfMaximums)
            {
                System.Console.WriteLine(
                    "Peak{0} and position {1}", maximum.Item2, maximum.Item1);
            }

            Console.WriteLine("There are {0} minimums, with a window range of 70",
                                _signalProcesser.ListOfMinimus.Count);

            Console.ReadKey();
            Console.WriteLine();

            foreach (Tuple<float, float> minimums in _signalProcesser.ListOfMinimus)
            {
                Console.WriteLine("Minimum{0} and position {1}",
                                    minimums.Item2, minimums.Item1);
            }

            validInput = false;

            Console.Write("\n Want to write values to CSV? ");

            do
            {
                answer = Console.ReadLine();
                answer = answer.ToLower();
                if (answer == "y" || answer == "n")
                {
                    validInput = true;
                }
                else
                {
                    Console.WriteLine("Not a valid input...");
                }
            } while (!validInput);

            if (answer == "y")
            {
                string fileName;
                Console.Write("Please enter file name: ");

                do
                {
                    fileName = Console.ReadLine();
                } while (fileName == null);

                resultPath = string.Concat(projectPath, fileName, ".csv");

                FileStream fs = new FileStream(resultPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                using (StreamWriter file = new StreamWriter(fs))
                {
                    file.WriteLine("# Maximum values");
                    for (int i = 0; i < _signalProcesser.ListOfMaximums.Count; i++)
                    {
                        file.WriteLine("{0}\t{1}",
                                        _signalProcesser.ListOfMaximums[i].Item1,
                                        _signalProcesser.ListOfMaximums[i].Item2);
                    }

                    file.WriteLine("# Minimum values");
                    for (int i = 0; i < _signalProcesser.ListOfMinimus.Count; i++)
                    {
                        file.WriteLine("{0}\t{1}",
                                        _signalProcesser.ListOfMinimus[i].Item1,
                                        _signalProcesser.ListOfMinimus[i].Item2);
                    }
                }
                fs.Close();
            }
        }
    }
}