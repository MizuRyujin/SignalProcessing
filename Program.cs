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
            Console.Clear();
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

            // runner.TestPeaks(processor);
            runner.TestMovingAverage(processor);
        }
    }
}

namespace CSVReader
{
    public class ProjectRunner
    {
        string _projectPath;
        string _resultPath;
        string _answer;
        bool _validInput = false;
        int _window;

        public ProjectRunner(string path)
        {
            _projectPath = path;
        }

        private string AskForYesOrNo()
        {
            _validInput = false;
            string answer;

            do
            {
                answer = Console.ReadLine();
                answer = answer.ToLower();
                if (answer == "y" || answer == "n")
                {
                    _validInput = true;
                }
                else
                {
                    Console.WriteLine("Not a valid input...");
                }
            } while (!_validInput);

            return answer;
        }

        private int AskForInt()
        {
            _validInput = false;
            string answer;
            int window;

            do
            {
                answer = Console.ReadLine();
                if (Int32.TryParse(answer, out window))
                {
                    _validInput = true;
                }
                else
                {
                    Console.WriteLine("Not a valid input...");
                }
            } while (!_validInput);

            return window;
        }

        private void SaveToCSV(SignalProcessing processor)
        {
            Console.Write("\n Want to write values to CSV? ");

            _answer = AskForYesOrNo();

            if (_answer == "y")
            {
                string fileName;
                Console.Write("Please enter file name: ");

                do
                {
                    fileName = Console.ReadLine();
                } while (fileName == null);

                _resultPath = string.Concat(_projectPath, fileName, ".csv");

                FileStream fs = new FileStream(_resultPath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
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

                    file.WriteLine("# Moving Average");
                    string[] concatValues = new string[processor.AverageTuple.Item2.Length];
                    for (int i = 0; i < processor.AverageTuple.Item2.Length; i++)
                    {
                        if (processor.AverageTuple?.Item1[i] == null) continue;

                        concatValues[i] = processor.AverageTuple.Item1[i].ToString();
                    }

                    for (int i = 0; i < processor.AverageTuple.Item2.Length; i++)
                    {
                        if (processor.AverageTuple?.Item2[i] == null) continue;

                        concatValues[i] = concatValues[i] + "\t" + processor.AverageTuple.Item2[i].ToString();
                    }
                    for (int i = 0; i < concatValues.Length; i++)
                    {
                        file.WriteLine(concatValues[i]);
                    }
                }
                fs.Close();
            }
        }

        public void TestPeaks(SignalProcessing processor)
        {
            Console.WriteLine("Testing Peaks Method");
            Console.Write("\nInput the search window: ");

            _window = AskForInt();

            Console.WriteLine("Press a key to find peaks...");
            Console.ReadKey();

            processor.FindPeaks(_window);

            Console.Clear();
            Console.WriteLine("There are {0} peaks, with a window range of {1}",
                                processor.ListOfMaximums.Count, _window);

            foreach (Tuple<float, float> maximum in processor.ListOfMaximums)
            {
                System.Console.WriteLine(
                    "Peak {0} and position {1}", maximum.Item2, maximum.Item1);
            }

            Console.WriteLine("There are {0} minimums, with a window range of {1}",
                                processor.ListOfMinimus.Count, _window);

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
            Console.WriteLine("Testing Moving Average Method");
            Console.Write("\nInput the search window: ");

            _window = AskForInt();

            Console.WriteLine("Press a key to get the moving average...");
            Console.ReadKey();

            processor.GetMovingAverage(_window);

            System.Console.WriteLine("The values are...");
            string[] concatValues = new string[processor.AverageTuple.Item2.Length];
            for (int i = 0; i < processor.AverageTuple.Item2.Length; i++)
            {
                if (processor.AverageTuple?.Item1[i] == null) continue;

                concatValues[i] = processor.AverageTuple.Item1[i].ToString();
            }

            for (int i = 0; i < processor.AverageTuple.Item2.Length; i++)
            {
                if (processor.AverageTuple?.Item2[i] == null) continue;

                concatValues[i] = concatValues[i] + ", " + processor.AverageTuple.Item2[i].ToString();
            }

            for (int i = 0; i < concatValues.Length; i++)
            {
                System.Console.WriteLine(concatValues[i]);
            }

            SaveToCSV(processor);
        }
    }
}
