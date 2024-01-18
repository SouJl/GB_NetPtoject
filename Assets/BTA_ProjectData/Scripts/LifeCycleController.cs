using Abstraction;
using System;
using System.Collections.Generic;

public class LifeCycleController : IDisposable
{
    private List<IOnStart> _onStarts = new List<IOnStart>();
    private List<IOnUpdate> _onUpdates = new List<IOnUpdate>();
    private List<IDisposable> _dispoisables = new List<IDisposable>();


    public void AddController(IController controller)
    {
        if (controller is IOnStart onStart)
        {
            _onStarts.Add(onStart);
        }

        if (controller is IOnUpdate onUpdate)
        {
            _onUpdates.Add(onUpdate);
        }

        if (controller is IDisposable onDisable)
        {
            _dispoisables.Add(onDisable);
        }
    }

    public void OnStart()
    {
        for (int i = 0; i < _onStarts.Count; i++)
        {
            var entity = _onStarts[i];
            entity.ExecuteStart();
        }
    }

    public void OnUpdate(float deltaTime)
    {
        for (int i = 0; i < _onUpdates.Count; i++)
        {
            var entity = _onUpdates[i];
            entity.ExecuteUpdate(deltaTime);
        }
    }


    public void Dispose()
    {
        for(int i =0; i < _dispoisables.Count; i++)
        {
            var entity = _dispoisables[i];
            entity?.Dispose();
        }

        _dispoisables.Clear();
        _onStarts.Clear();
        _onUpdates.Clear();
    }

}
