using System;

namespace CI.HttpClient
{
    public interface IDispatcher
    {
        void Enqueue(Action action);
    }
}