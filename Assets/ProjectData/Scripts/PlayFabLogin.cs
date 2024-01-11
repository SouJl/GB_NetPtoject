using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public class PlayFabLogin : MonoBehaviour
{
    [SerializeField]
    private string _titleId;
    [SerializeField]
    private string _userId;

    private void Start()
    {
        if (string.IsNullOrEmpty(PlayFabSettings.staticSettings.TitleId)) 
        {
            PlayFabSettings.staticSettings.TitleId = _titleId;
        }


        var request = new LoginWithCustomIDRequest
        {
            CustomId = _userId,
            CreateAccount = true
        };

        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginError);
    }


    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log($"[{result.PlayFabId}] - Login Complete");
    }

    private void OnLoginError(PlayFabError error)
    {
        var errorMessage = error.GenerateErrorReport();
        Debug.LogError(errorMessage);
    }
}
