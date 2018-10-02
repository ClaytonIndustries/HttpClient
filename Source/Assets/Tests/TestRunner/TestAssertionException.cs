using System;

namespace CI.TestRunner
{
    public class TestAssertionException : Exception
    {
        public TestAssertionException(string message)
            : base(message)
        {
        }
    }
}
