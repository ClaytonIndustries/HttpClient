using System.Collections.Generic;

namespace CI.TestRunner
{
    public class TestFixtureResult
    {
        public string Name { get; set; }
        public List<TestResult> TestResults { get; set; }

        public TestFixtureResult()
        {
            TestResults = new List<TestResult>();
        }
    }
}