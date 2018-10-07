using System;
using CI.HttpClient.Core;
using UnityEngine;

namespace CI.TestRunner
{
    public class TestDispatcher : MonoBehaviour, IDispatcher
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Enqueue(Action action)
        {
            action();
        }
    }
}