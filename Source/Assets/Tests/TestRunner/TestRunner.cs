using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;

namespace CI.TestRunner
{
    public class TestRunner
    {
        public List<TestFixtureResult> Run()
        {
            List<TestFixtureResult> testFixtureResults = new List<TestFixtureResult>();

            var testFixtures = GetTestFixtures();

            foreach (Type testFixture in testFixtures)
            {
                var result = RunFixture(testFixture);

                testFixtureResults.Add(result);
            }

            return testFixtureResults;
        }

        private TestFixtureResult RunFixture(Type testFixture)
        {
            var testFixtureResult = new TestFixtureResult()
            {
                Name = testFixture.Name
            };

            var setupMethod = GetMethod(testFixture, typeof(Setup));
            var testMethods = GetTestMethods(testFixture);
            var tearDownMethod = GetMethod(testFixture, typeof(TearDown));

            if (!testMethods.Any())
            {
                return testFixtureResult;
            }

            object classInstance = Activator.CreateInstance(testFixture);

            foreach (MethodInfo testMethod in testMethods)
            {
                var result = RunTest(testMethod, setupMethod, tearDownMethod, classInstance);

                testFixtureResult.TestResults.Add(result);
            }

            return testFixtureResult;
        }

        private TestResult RunTest(MethodInfo testMethod, MethodInfo setupMethod, MethodInfo tearDownMethod, object classInstance)
        {
            TestResult testResult = new TestResult()
            {
                Name = testMethod.Name
            };

            try
            {
                InvokeMethod(setupMethod, classInstance);
                InvokeTest(testMethod, classInstance);
                InvokeMethod(tearDownMethod, classInstance);

                testResult.IsPassing = true;
            }
            catch (TargetInvocationException e)
            {
                testResult.Exception = e.InnerException;
            }

            return testResult;
        }

        private IEnumerable<Type> GetTestFixtures()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            return assembly.GetTypes()
                           .Where(x => x.GetCustomAttributes(typeof(TestFixture), false).Any());
        }

        private MethodInfo GetMethod(Type testFixture, Type attribute)
        {
            return testFixture.GetMethods()
                  .Where(x => x.GetCustomAttributes(attribute, false).Any())
                  .FirstOrDefault();
        }

        private IEnumerable<MethodInfo> GetTestMethods(Type testFixture)
        {
            return testFixture.GetMethods()
                              .Where(x => x.GetCustomAttributes(typeof(Test), false).Any());
        }

        private void InvokeTest(MethodInfo testMethod, object classInstance)
        {
            testMethod.Invoke(classInstance, null);
        }

        private void InvokeMethod(MethodInfo method, object classInstance)
        {
            if (method == null)
            {
                return;
            }

            method.Invoke(classInstance, null);
        }
    }
}