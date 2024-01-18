using UnityEngine;
using TMPro;

namespace UI
{
    public class DebugMessageUI : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Text _messageText;

        private void Awake()
        {
            _messageText.text = "";
        }

        public void SetMessage(string message, Color color)
        {
            _messageText.color = color;
            _messageText.text = message;
        }
    }
}
