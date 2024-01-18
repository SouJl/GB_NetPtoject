using Configs;
using Enumerators;
using System;
using UI;
using UnityEngine;

public class MainController : IDisposable
{
    private readonly Transform _placeForUi;
    private readonly LifeCycleController _lifeCycle;

    private readonly AuthenticationMenuController _authenticationController;

    public MainController(
        Transform placeForUi, 
        GameConfig gameConfig, 
        LifeCycleController lifeCycle)
    {
        _placeForUi = placeForUi;
        _lifeCycle = lifeCycle;

        _authenticationController = new AuthenticationMenuController(placeForUi, gameConfig);


        _lifeCycle.AddController(_authenticationController);
    }


    private void OnChangeGameState(GameState state)
    {
        DisposeControllers();
       
        switch (state)
        {
            default:
                break;
        }
    }

    private void DisposeControllers()
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        DisposeControllers();
    }

}
