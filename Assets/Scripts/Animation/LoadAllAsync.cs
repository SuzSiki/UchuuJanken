using UnityEngine;

using System.Threading.Tasks;
using System.Runtime.CompilerServices;
public class LoadAllAsync<T>
where T : Object
{
    string path;
    LoadAllAwaiter<T> _awaiter;
    public LoadAllAwaiter<T> GetAwaiter()
    {
        return _awaiter;
    }

    public LoadAllAsync(string path)
    {
        _awaiter = new LoadAllAwaiter<T>(path);
    }
}

public class LoadAllAwaiter<T> : INotifyCompletion
where T : Object
{
    private Task _task;
    T[] _result;

    public LoadAllAwaiter(string path)
    {
        System.Action action = () =>
        {
            _result = Resources.LoadAll<T>(path);
        };

        _task = new Task(action);

        _task.Start();
    }

    public bool IsCompleted { get { return _task.IsCompleted; } }
    public void OnCompleted(System.Action continuation) { }
    public T[] GetResult() { return _result; }
}