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

        //* Properties
        public float[] XValues => _xValues;
        public float[] YValues => _yValues;
        public List<Tuple<float, float>> ListOfMaximums => _listOfMaximums;
        public List<Tuple<float, float>> ListOfMinimus => _listOfMinimums;

        public SignalProcessing(string path)
        {
            ReadCSV(path);
            _listOfMaximums = new List<Tuple<float, float>>();
            _listOfMinimums = new List<Tuple<float, float>>();
        }

        //* Private methods
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

        //* For how this method works:
        //* https://stackoverflow.com/questions/5269000/finding-local-maxima-over-a-dynamic-range
        private IEnumerable<Tuple<float, float>> LocalMaxima(IEnumerable<float> source, int windowSize)
        {
            // Round up to nearest odd value
            windowSize = windowSize - windowSize % 2 + 1;
            int halfWindow = windowSize / 2;

            int index = 0;
            Queue<float> before = new Queue<float>(Enumerable.Repeat(float.NegativeInfinity, halfWindow));
            Queue<float> after = new Queue<float>(source.Take(halfWindow + 1));

            foreach (float d in source.Skip(halfWindow + 1).Concat(Enumerable.Repeat(float.NegativeInfinity, halfWindow + 1)))
            {
                float curVal = after.Dequeue();
                if (before.All(x => curVal > x) && after.All(x => curVal >= x))
                {
                    yield return Tuple.Create(_xValues[index], curVal);
                }

                before.Dequeue();
                before.Enqueue(curVal);
                after.Enqueue(d);
                index++;
            }
        }

        private IEnumerable<Tuple<float, float>> LocalMinima(IEnumerable<float> source, int windowSize)
        {
            // Round up to nearest odd value
            windowSize = windowSize - windowSize % 2 + 1;
            int halfWindow = windowSize / 2;

            int index = 0;
            Queue<float> before = new Queue<float>(Enumerable.Repeat(float.NegativeInfinity, halfWindow));
            Queue<float> after = new Queue<float>(source.Take(halfWindow + 1));

            foreach (float d in source.Skip(halfWindow + 1).Concat(Enumerable.Repeat(float.NegativeInfinity, halfWindow + 1)))
            {
                float curVal = after.Dequeue();
                if (before.All(x => curVal < x) && after.All(x => curVal <= x))
                {
                    yield return Tuple.Create(_xValues[index], curVal);
                }

                before.Dequeue();
                before.Enqueue(curVal);
                after.Enqueue(d);
                index++;
            }
        }

        private Tuple<float[], int[]> MovingAverage(
                    float[] xValue, int[] yValue, int dim, int k, int step = 1)
        {

            List<float> BioValues = new List<float>(xValue);
            List<int> samples = new List<int>();

            for (int x = k; x < dim - k; x += step) //! It must change by n value
            {
                // float ySum = 0;

                for (int y = 0; y < yValue.Length; y++)
                {

                }
            }

            return new Tuple<float[], int[]>(BioValues.ToArray(), samples.ToArray());
        }

        //* Public methods
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
    }
}