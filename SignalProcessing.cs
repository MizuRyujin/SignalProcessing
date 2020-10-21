using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader
{
    public class SignalProcessing
    {
        //* Variables
        private float[] _xValues = default;
        private float[] _yValues = default;
        private IEnumerable<Tuple<float, float>> _maximums = default;
        private IEnumerable<Tuple<float, float>> _minimums = default;
        private List<Tuple<float, float>> _listOfMaximums = default;
        private List<Tuple<float, float>> _listOfMinimums = default;
        private List<float> _averages = default;

        //* Properties
        public float[] XValues => _xValues;
        public float[] YValues => _yValues;
        public List<Tuple<float, float>> ListOfMaximums => _listOfMaximums;
        public List<Tuple<float, float>> ListOfMinimus => _listOfMinimums;
        public List<float> Averages => _averages;

        //* Constructor
        public SignalProcessing()
        {
            _listOfMaximums = new List<Tuple<float, float>>();
            _listOfMinimums = new List<Tuple<float, float>>();
        }

        public SignalProcessing(string path)
        {
            ReadCSV(path);
            _listOfMaximums = new List<Tuple<float, float>>();
            _listOfMinimums = new List<Tuple<float, float>>();
        }

        #region Private Methods
        private void ReadCSV(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            StreamReader file = new StreamReader(fs);

            List<float> xValues = new List<float>();
            List<float> yValues = new List<float>();

            string line;
            string[] splitString;

            while ((line = file.ReadLine()) != null)
            {
                splitString = line.Split("\t");
                xValues.Add(Convert.ToSingle(splitString[0]));
                yValues.Add(Convert.ToSingle(splitString[1]));
            }

            _xValues = xValues.ToArray();
            _yValues = yValues.ToArray();

            file.Close();
        }

        //* Reference for this method works:
        //* https://stackoverflow.com/questions/5269000/finding-local-maxima-over-a-dynamic-range
        //* Using the awnser from: https://stackoverflow.com/users/2805511/jeroen-cranendonk
        private IEnumerable<Tuple<float, float>> LocalMaxima(IEnumerable<float> source, int windowSize)
        {
            // Round up to nearest odd value
            windowSize = windowSize - windowSize % 2 + 1;
            int halfWindow = windowSize / 2;

            int index = 0;
            //* where last compared value is gonna go
            Queue<float> lastComparedValue = new Queue<float>(Enumerable.Repeat(float.NegativeInfinity, halfWindow));

            //* Left half of the window
            Queue<float> leftHalf = new Queue<float>(source.Take(halfWindow + 1));

            //* goes through collection from half point + 1 
            foreach (float d in source.Skip(halfWindow + 1).Concat(Enumerable.Repeat(float.NegativeInfinity, halfWindow + 1)))
            {
                //* Removes FIFO element from the queue
                float curVal = leftHalf.Dequeue();

                if (curVal > lastComparedValue.Max() && curVal >= leftHalf.Max())
                {
                    yield return Tuple.Create(_xValues[index], curVal);
                }

                //* Removes first instance of Negative Infinity and eventually previous stored curVal
                lastComparedValue.Dequeue();
                //* Add the current analysed value
                lastComparedValue.Enqueue(curVal);
                //* 
                leftHalf.Enqueue(d);
                index++;
            }
        }

        //* Reference for this method works:
        //* https://stackoverflow.com/questions/5269000/finding-local-maxima-over-a-dynamic-range
        //* Using the awnser from: https://stackoverflow.com/users/2805511/jeroen-cranendonk
        private IEnumerable<Tuple<float, float>> LocalMinima(IEnumerable<float> source, int windowSize)
        {
            // Round up to nearest odd value
            windowSize = windowSize - windowSize % 2 + 1;
            int halfWindow = windowSize / 2;

            int index = 0;
            Queue<float> lastComparedValue = new Queue<float>(Enumerable.Repeat(float.NegativeInfinity, halfWindow));
            Queue<float> leftHalf = new Queue<float>(source.Take(halfWindow + 1));

            //* Save the collection before using it
            foreach (float d in source.Skip(halfWindow + 1).Concat(Enumerable.Repeat(float.NegativeInfinity, halfWindow + 1)))
            {
                float curVal = leftHalf.Dequeue();

                if (curVal < lastComparedValue.Min() && curVal <= leftHalf.Min())
                {
                    yield return Tuple.Create(_xValues[index], curVal);
                }

                lastComparedValue.Dequeue();
                lastComparedValue.Enqueue(curVal);
                leftHalf.Enqueue(d);
                index++;
            }
        }

        private List<float> MovingAverage(
            float[] yValue, int window, int step = 1)
        {
            float[] before = new float[window];
            before = Enumerable.Repeat(
                        yValue.First(), before.Length).ToArray();

            float[] after = new float[window];
            after = Enumerable.Repeat(
                        yValue.Last(), after.Length).ToArray();

            float[] concatArray = new float[before.Length + yValue.Length + after.Length];
            concatArray = Enumerable.Concat(before, yValue).Concat(after).ToArray();

            List<float> movAverage = new List<float>();

            for (int i = window; i < yValue.Length + window; i += step)
            {
                float ySum = 0f;
                for (int j = -window; j < window; j++)
                {
                    ySum += concatArray[i + j];
                }
                ySum = ySum / (2 * window);
                movAverage.Add(MathF.Round(ySum, 4));
            }

            return movAverage;
        }

        #endregion

        #region Public methods
        public void FindPeaks(int window)
        {
            _maximums = LocalMaxima(_yValues, window);
            _minimums = LocalMinima(_yValues, window);

            foreach (Tuple<float, float> maximum in _maximums)
            {
                _listOfMaximums.Add(maximum);
            }

            foreach (Tuple<float, float> minimum in _minimums)
            {
                _listOfMinimums.Add(minimum);
            }
        }

        public void GetMovingAverage(int window, int step = 1)
        {
            _averages = MovingAverage(_yValues, window, step);
        }

        public void EDAConversion(int value)
        {
            int n = 10;
            float vcc = 3.3f;

            float EDA = ((value / (MathF.Pow(2, n)) * vcc) / 0.132f);
        }

        #endregion
    }
}