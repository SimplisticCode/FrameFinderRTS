using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FrameFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = new List<Task>();
            using (var reader = new StreamReader(@"/Users/simonthranehansen/Downloads/Hwk1table1.csv"))
            {
                reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var temp = new Task();
                    var values = new List<string>(line.Split(';'));
                    if (string.IsNullOrEmpty(values[0])) continue;
                    temp.FromString(values);
                    data.Add(temp);
                }
            }
            var H = Calculator.FindHyperPeriod(data);
            var possibleFrameSize = Calculator.FindFrameSize(data, H);
            var i = 2;
        }



        public class Task
        {
            public int JobNumber { get; set; }
            public int Period { get; set; }
            public int Deadline { get; set; }
            public double ExecutionTime { get; set; }
            public double Utilization { get; set; }

            public void FromString(List<string> values)
            {
                JobNumber = Convert.ToInt32(values[0]);
                Period = Convert.ToInt32(values[2]);
                Deadline = Convert.ToInt32(values[4]);
                ExecutionTime = Convert.ToDouble(values[3])/10000;
                Utilization = calcUtilization();

            }

            private double calcUtilization()
            {
                return Utilization / Period;
            }
        }

        public class Calculator 
        {
            
            public static List<int> FindFrameSize(List<Task> data, int hyperPeriod)
            {
                //Criteria 1:
                var maxExecutionTime = data.Max(o => o.ExecutionTime);

                var dividesHyperPeriod = DivideEvenly((int) maxExecutionTime, hyperPeriod);

                var result = multipleExecution(dividesHyperPeriod, data);
                return result;
            }

            private static List<int> multipleExecution(IEnumerable<int> dividesHyperPeriod, List<Task> data)
            {
                var result = (from f in dividesHyperPeriod from d in data where 2 * f - MathUtils.GCD(d.Period, f) < d.Deadline select f);
                return result.Distinct().ToList();
            }

            public static int FindHyperPeriod(List<Task> data)
            {
                var hyperPeriod =  MathUtils.LCM(data.Select(o => o.Period).Distinct().ToList());
                return hyperPeriod;
            }
            
            
            public static IEnumerable<int> DivideEvenly(int minFrameSize, int HyperPeriod)
            {
                var result = new List<int>();
                for (var i=minFrameSize; i < HyperPeriod; i++)
                {
                    if(HyperPeriod % i == 0)
                        result.Add(i);
                }

                return result;
            }

        }
        
        public static class MathUtils
        {
            /// <summary>
            /// Calculates the least common multiple of 2+ numbers.
            /// </summary>
            /// <remarks>
            /// Uses recursion based on lcm(a,b,c) = lcm(a,lcm(b,c)).
            /// Ported from http://stackoverflow.com/a/2641293/420175.
            /// </remarks>
            public static int LCM(List<int> numbers)
            {
                if (numbers.Count < 2)
                    throw new ArgumentException("you must pass two or more numbers");
                return LCM(numbers, 0);
            }

            private static int LCM(List<int> numbers, int i)
            {
                // Recursively iterate through pairs of arguments
                // i.e. lcm(args[0], lcm(args[1], lcm(args[2], args[3])))

                if (i + 2 == numbers.Count)
                {
                    return LCM(numbers[i], numbers[i+1]);
                }
                else
                {
                    return LCM(numbers[i], LCM(numbers, i+1));
                }
            }

            public static int LCM(int a, int b)
            {
                return (a * b / GCD(a, b));
            }

            /// <summary>
            /// Finds the greatest common denominator for 2 numbers.
            /// </summary>
            /// <remarks>
            /// Also from http://stackoverflow.com/a/2641293/420175.
            /// </remarks>
            public static int GCD(int a, int b)
            {
                // Euclidean algorithm
                int t;
                while (b != 0)
                {
                    t = b;
                    b = a % b;
                    a = t;
                }
                return a;
            }
        }
    }
}