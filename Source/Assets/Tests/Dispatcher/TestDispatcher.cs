using System;
using CI.HttpClient.Core;
using UnityEngine;

namespace CI.TestRunner
{
    public class TestDispatcher : MonoBehaviour, IDispatcher
    {
        public void Enqueue(Action action)
        {
            action();
        }
    }
}