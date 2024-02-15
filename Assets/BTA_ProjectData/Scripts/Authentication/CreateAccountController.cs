using Abstraction;
using Enumerators;
using Prefs;
using Tools;
using UnityEngine;

namespace Authentication
{
    public class CreateAccountController : BaseController
    {
        private readonly ResourcePath _viewPath = new ResourcePath("Prefabs/UI/CreateAccountUI");

        private readonly CreateAccountView _view;
        private readonly AuthenticationPrefs _prefs;
        
        public CreateAccountController(
            Transform placeForUI, 
            AuthenticationPrefs prefs)
        {
            _view = LoadView(placeForUI);
            _view.InitUI();

            _prefs = prefs;

            Subscribe();
        }

        private CreateAccountView LoadView(Transform placeForUI)
        {
            var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUI, false);

            AddGameObject(objectView);

            return objectView.GetComponent<CreateAccountView>();
        }

        private void Subscribe()
        {
            _view.OnCreateAccount += CreateAccount;
            _view.OnBack += ReturnBack;
        }
        private void Unsubscribe()
        {
            _view.OnCreateAccount -= CreateAccount;
            _view.OnBack -= ReturnBack;
        }

        private void CreateAccount(UserAccountData userData)
        {
            
        }

        private void ReturnBack()
        {
            _prefs.ChangeState(AuthenticationState.Main);
        }
      
        protected override void OnDispose()
        {
            base.OnDispose();

            Unsubscribe();
        }
    }
}
