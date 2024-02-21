using UnityEngine;

namespace BTAPlayer
{
    public class PlayerBodyObjects : MonoBehaviour
    {
        [SerializeField]
        private GameObject[] _bodyParts;


        public void ShowBody()
        {
            for(int i =0; i < _bodyParts.Length; i++)
            {
                _bodyParts[i].SetActive(true);
            }
        }

        public void HideBody()
        {
            for (int i = 0; i < _bodyParts.Length; i++)
            {
                _bodyParts[i].SetActive(false);
            }
        }
    }
}
