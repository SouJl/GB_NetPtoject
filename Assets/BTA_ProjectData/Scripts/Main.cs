using Configs;
using Tools;
using UnityEngine;

public class Main : MonoBehaviour
{
    [SerializeField]
    private Transform _placeForUI;
    [SerializeField]
    private GameConfig _gameConfig;
    [SerializeField]
    private GameNetManager _netManager;
    [SerializeField]
    private StateTransition _stateTransition;

    private float _deltaTime;

    private LifeCycleController _gameLifecycle;
    private MainController _mainController;

    private void Awake()
    {
        _deltaTime = Time.deltaTime;
        _gameLifecycle = new LifeCycleController();
        _mainController 
            = new MainController(_placeForUI, _gameConfig, _gameLifecycle, _netManager, _stateTransition);
    }

    private void Start()
    {
        _gameLifecycle.OnStart();
    }

    private void Update()
    {
        _gameLifecycle.OnUpdate(_deltaTime);
    }
}
