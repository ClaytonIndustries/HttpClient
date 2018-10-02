using System;

namespace CI.TestRunner
{
    public class TestResult
    {
        public string Name { get; set; }
        public bool IsPassing { get; set; }
        public Exception Exception { get; set; }
    }
}