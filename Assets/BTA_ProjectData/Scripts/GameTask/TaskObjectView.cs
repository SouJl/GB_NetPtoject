using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace GameTask
{
    public class TaskObjectView : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _discription;
        [SerializeField]
        private Toggle _completeState;


        public void InitUI(TaskModel task)
        {
            _discription.text = task.Discription;
            _completeState.isOn = false;
        }

        public void SetCompele()
        {
            _completeState.isOn = true;
            _discription.color = Color.green;
        }
    }
}
