using UnityEngine;
using TMPro;

namespace BTAPlayer
{
    public class PlayerUI : MonoBehaviour
    {
        [SerializeField]
        private Canvas _uiCanvas;
        [SerializeField]
        private Transform _orientation;
        [SerializeField]
        private TMP_Text _name;

        private Camera _camera;

        public void Init(Camera camera, string name)
        {
            _camera = camera;

            _uiCanvas.worldCamera = _camera;

            SetName(name);
        }

        public void SetName(string name)
        {
            _name.text = name;
        }

        private void LateUpdate()
        {
            var lookPos = transform.position - _camera.transform.position;
            transform.rotation = Quaternion.LookRotation(lookPos);
        }
    }

}
