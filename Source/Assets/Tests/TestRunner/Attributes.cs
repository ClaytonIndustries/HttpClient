using System;

namespace CI.TestRunner
{
    public class TestFixture : Attribute
    {
    }

    public class Test : Attribute
    {
    }

    public class Setup : Attribute
    {
    }

    public class TearDown : Attribute
    {
    }
}