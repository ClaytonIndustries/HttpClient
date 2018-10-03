using System.Reflection;
using System.Linq;
using System;
using System.Collections.Generic;

namespace CI.TestRunner
{
    public class TestRunner
    {
        public Action<TestResult> TestFinished { get; set; }

        public void Run()
        {
            var testFixtures = GetTestFixtures();

            foreach (Type testFixture in testFixtures)
            {
                RunFixture(testFixture);
            }
        }

        private void RunFixture(Type testFixture)
        {
            var setupMethod = GetMethod(testFixture, typeof(Setup));
            var testMethods = GetTestMethods(testFixture);
            var tearDownMethod = GetMethod(testFixture, typeof(TearDown));

            if (!testMethods.Any())
            {
                return;
            }

            object classInstance = Activator.CreateInstance(testFixture);

            foreach (MethodInfo testMethod in testMethods)
            {
                RunTest(testMethod, setupMethod, tearDownMethod, classInstance, testFixture.Name);
            }
        }

        private void RunTest(MethodInfo testMethod, MethodInfo setupMethod, MethodInfo tearDownMethod, object classInstance, string fixtureName)
        {
            TestResult testResult = new TestResult()
            {
                FixtureName = fixtureName,
                TestName = testMethod.Name
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

            TestFinished?.Invoke(testResult);
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