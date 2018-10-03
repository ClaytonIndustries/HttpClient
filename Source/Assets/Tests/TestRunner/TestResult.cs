using System;

namespace CI.TestRunner
{
    public class TestResult
    {
        public string FixtureName { get; set; }
        public string TestName { get; set; }
        public bool IsPassing { get; set; }
        public Exception Exception { get; set; }
    }
}