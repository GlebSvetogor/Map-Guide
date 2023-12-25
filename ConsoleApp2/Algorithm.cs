using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp2
{
    abstract class Algorithm
    {
        public abstract void FindRoute();
    }

    class FastestAlgorithm : Algorithm
    {
        public override void FindRoute()
        {

        }
    }

    class ShortestAlgorithm : Algorithm
    {
        public override void FindRoute()
        {

        }
    }

    abstract class AlgorithmCreator
    {
        public abstract Algorithm CreateAlgorithm();
    }

    class FastestAlgorithmCreator : AlgorithmCreator
    {
        public override Algorithm CreateAlgorithm()
        {
            return new FastestAlgorithm();
        }
    }

    class ShortestAlgorithmCreator : AlgorithmCreator
    {
        public override Algorithm CreateAlgorithm()
        {
            return new ShortestAlgorithm();
        }
    }
}
