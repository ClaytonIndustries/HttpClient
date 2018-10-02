using System;
using CI.HttpClient.Core;
using UnityEngine;

namespace CI.UnityTestRunner
{
    public class TestDispatcher : MonoBehaviour, IDispatcher
    {
        public void Enqueue(Action action)
        {
            action();
        }
    }
}