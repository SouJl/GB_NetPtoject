using Configs;
using MultiplayerService;
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
    [SerializeField]
    private AudioSource _backgroundMusic;
    [SerializeField]
    private AudioSource _clickSound;

    private float _deltaTime;

    private LifeCycleController _gameLifecycle;
    private MainController _mainController;

    private void Awake()
    {
        _deltaTime = Time.deltaTime;
        _gameLifecycle = new LifeCycleController();

        _mainController
             = new MainController(
                 _placeForUI,
                 _gameConfig,
                 _gameLifecycle,
                 _netManager,
                 _stateTransition,
                 _backgroundMusic,
                 _clickSound);
    }

    private void Start()
    {
        _gameLifecycle.OnStart();
    }

    private void Update()
    {
        _gameLifecycle.OnUpdate(_deltaTime);
    }


    private void OnDestroy()
    {
        _gameLifecycle?.Dispose();

        _mainController?.Dispose();
    }

}
