using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace CSVReader
{
    public static class Test
    {
        public static IEnumerable<Tuple<int, float>> LocalMaxima(IEnumerable<float> source, int windowSize)
        {
            //* Round up to nearest odd value
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

                //* All() : Determines whether all elements of a sequence satisfy a condition.
                if (curVal > lastComparedValue.Max() && curVal >= leftHalf.Max())
                {
                    System.Console.WriteLine($"curVal after compare: {curVal}");
                    yield return Tuple.Create(index, curVal);
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
    }
}