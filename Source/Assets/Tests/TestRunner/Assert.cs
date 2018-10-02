
using System;

namespace CI.TestRunner
{
    public static class Assert
    {
        public static void IsTrue(bool value)
        {
            if (!value)
            {
                throw new TestAssertionException("Expected true but was false");
            }
        }

        public static void IsFalse(bool value)
        {
            if (value)
            {
                throw new TestAssertionException("Expected false but was true");
            }
        }

        public static void AreEqual(object expected, object actual)
        {
            if (!expected.Equals(actual))
            {
                throw new TestAssertionException(string.Format("Expected {0} but was {1}", expected.ToString(), actual.ToString()));
            }
        }

        public static void AreNotEqual(object expected, object actual)
        {
            if (expected.Equals(actual))
            {
                throw new TestAssertionException(string.Format("Expected {0} not to equal {1}", expected.ToString(), actual.ToString()));
            }
        }

        public static void IsNull(object item)
        {
            if (item != null)
            {
                throw new TestAssertionException(string.Format("Expected object to be null"));
            }
        }

        public static void IsNotNull(object item)
        {
            if (item == null)
            {
                throw new TestAssertionException(string.Format("Expected object not to be null"));
            }
        }

        public static void Throws<T>(Action function)
        {
            try
            {
                function();
            }
            catch (Exception e)
            {
                if(!(e is T))
                {
                    throw new TestAssertionException(string.Format("Expected {0} to be thrown, but was {1}", typeof(T).ToString(), e.GetType().ToString()));
                }
            }

            throw new TestAssertionException(string.Format("Expected {0} to be thrown, but no exception was thrown", typeof(T).ToString()));
        }
    }
}