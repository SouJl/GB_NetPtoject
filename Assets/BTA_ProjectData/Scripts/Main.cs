using Abstraction;
using Configs;
using MultiplayerService;
using ParrelSync;
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
    private bool _isCloneEditor = false;
    [SerializeField]
    [DrawIf("_isCloneEditor", true)]
    private string _cloneUserId = "28B6E54FE89BE10E";
    [SerializeField]
    [DrawIf("_isCloneEditor", true)]
    private string _cloneUserName = "UserTest";
    [SerializeField]
    [DrawIf("_isCloneEditor", true)]
    private string _clonePassword = "qwe123";

    private float _deltaTime;

    private LifeCycleController _gameLifecycle;
    private MainController _mainController;

    private void Awake()
    {
        _deltaTime = Time.deltaTime;
        _gameLifecycle = new LifeCycleController();

        if (_isCloneEditor)
        {
            var userData = new UserData
            {
                Id = _cloneUserId,
                UserName = _cloneUserName,
                Password = _clonePassword
            };

            _mainController
                = new MainController(
                    _placeForUI, 
                    _gameConfig, 
                    _gameLifecycle, 
                    _netManager, 
                    _stateTransition, 
                    userData);
        }
        else
        {
            _mainController 
                = new MainController(
                    _placeForUI, 
                    _gameConfig, 
                    _gameLifecycle, 
                    _netManager, 
                    _stateTransition);
        }
       
    }

    private void Start()
    {
        _gameLifecycle.OnStart();
    }

    private void Update()
    {
        _isCloneEditor = ClonesManager.IsClone();

        _gameLifecycle.OnUpdate(_deltaTime);
    }


    private void OnDestroy()
    {
        _gameLifecycle?.Dispose();

        _mainController?.Dispose();
    }

}
