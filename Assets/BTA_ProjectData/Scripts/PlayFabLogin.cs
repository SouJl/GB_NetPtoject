using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UI;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField]
    private string _titleId;
    [SerializeField]
    private PlayFabLogInUI _playFabLogInUI;
    [SerializeField]
    private DebugConsoleUI _debugConsoleUI;


    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) 
        {
            PlayFabSettings.staticSettings.TitleId = _titleId;
        }

        _playFabLogInUI.OnLogIn += ConnectToPlayFab;
    }

    private void ConnectToPlayFab(string userId)
    {
        if(userId == null || userId == "")
        {
            _debugConsoleUI.LogError("UserId is null or empty");
            return;
        }

        var request = new LoginWithCustomIDRequest
        {
            CustomId = userId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        var resultMessage = $"[{result.PlayFabId}] - Login Complete";

        _debugConsoleUI.Log(resultMessage);

        Debug.Log(resultMessage);
    }

    private void OnLoginError(PlayFabError error)
    {
        var resultMessage = error.GenerateErrorReport();

        _debugConsoleUI.LogError(resultMessage);

        Debug.LogError(resultMessage);
    }
}
