using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CI.HttpClient
{
    public class Dispatcher : MonoBehaviour
    {
        private static readonly Queue<Action> _queue = new Queue<Action>();
        private static readonly object _lock = new object();

        private static Dispatcher _instance = null;

        private void Awake()
        {
            if(_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        private void OnDestroy()
        {
            _instance = null;
        }

        public static Dispatcher Instance()
        {
            return _instance;
        }

        public void Update()
        {
            lock (_lock)
            {
                while (_queue.Count > 0)
                {
                    _queue.Dequeue().Invoke();
                }
            }
        }

        public void Enqueue(Action action)
        {
            lock (_lock)
            {
                _queue.Enqueue(action);
            }
        }
    }
}