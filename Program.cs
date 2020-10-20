using System;
using System.IO;
using System.Collections.Generic;

namespace CSVReader
{
    public class Program
    {
        //* Need to give path to where source is located
        private static void Main(string[] args)
        {
            // string projectPath = @"D:\Miguel\Documents\Projetos\CSVReader\";
            string projectPath;

            if (args == null)
            {
                throw new Exception("No path given");
            }
            else
            {
                projectPath = args[0];
            }


            string csvPath = string.Concat(projectPath, "scr.csv");

            SignalProcessing processor = new SignalProcessing(csvPath);
            ProjectRunner runner = new ProjectRunner(projectPath);

            Console.WriteLine("There are {0} values to process",
                                    processor.XValues.Length);

            runner.TestPeaks(processor);
        }
    }
}

namespace CSVReader
{
    public class ProjectRunner
    {
        string projectPath;
        string resultPath;
        string answer;
        bool validInput = false;
        int window;

        public ProjectRunner(string path)
        {
            projectPath = path;
        }

        public void TestPeaks(SignalProcessing processor)
        {
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

            processor.FindPeaks(window);

            Console.Clear();
            Console.WriteLine("There are {0} peaks, with a window range of {1}",
                                processor.ListOfMaximums.Count, window);

            foreach (Tuple<float, float> maximum in processor.ListOfMaximums)
            {
                System.Console.WriteLine(
                    "Peak {0} and position {1}", maximum.Item2, maximum.Item1);
            }

            Console.WriteLine("There are {0} minimums, with a window range of {1}",
                                processor.ListOfMinimus.Count, window);

            Console.WriteLine();

            foreach (Tuple<float, float> minimums in processor.ListOfMinimus)
            {
                Console.WriteLine("Minimum {0} and position {1}",
                                    minimums.Item2, minimums.Item1);
            }

            SaveToCSV(processor);
        }

        public void TestMovingAverage(SignalProcessing processor)
        {

        }

        public void SaveToCSV(SignalProcessing processor)
        {
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
                    for (int i = 0; i < processor.ListOfMaximums.Count; i++)
                    {
                        file.WriteLine("{0}\t{1}",
                                        processor.ListOfMaximums[i].Item1,
                                        processor.ListOfMaximums[i].Item2);
                    }

                    file.WriteLine("# Minimum values");
                    for (int i = 0; i < processor.ListOfMinimus.Count; i++)
                    {
                        file.WriteLine("{0}\t{1}",
                                        processor.ListOfMinimus[i].Item1,
                                        processor.ListOfMinimus[i].Item2);
                    }
                }
                fs.Close();
            }
        }
    }
}
