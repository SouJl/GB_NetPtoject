using Abstraction;
using Authentication.View;
using Prefs;
using Tools;
using UnityEngine;

namespace Authentication
{
    public class NoDataFoundController : BaseController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/Authentication/NoDataFoundUI");

        private readonly NoDataFoundUI _view;
        private readonly AuthenticationPrefs _prefs;
        public NoDataFoundController(
            Transform placeForUI,
            AuthenticationPrefs prefs)
        {
            _view = LoadView(placeForUI);
            _view.InitUI();

            _prefs = prefs;

            Subscribe();
        }

        private NoDataFoundUI LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<NoDataFoundUI>();
        }

        private void Subscribe()
        {
            _view.OnSignIn += SigIn;
            _view.OnRegister += Register;
        }

        private void Unsubscribe()
        {
            _view.OnSignIn -= SigIn;
            _view.OnRegister -= Register;
        }

        private void SigIn()
        {
            _prefs.ClickSound.Play();

            _prefs.SetUserdata(new BaseGameUser
            {
                Name = _view.Name,
                Password = _view.Password
            });

            _prefs.ChangeState(Enumerators.AuthenticationState.SigIn);
        }

        private void Register()
        {
            _prefs.ClickSound.Play();

            _prefs.ChangeState(Enumerators.AuthenticationState.Register);
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            
            _view?.Dispose();

            Unsubscribe();
        }
    }
}
