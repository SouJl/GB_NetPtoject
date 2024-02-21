using System.Collections.Generic;
using UnityEngine;

namespace GameTask
{
    public class TaskManagerView : MonoBehaviour
    {
        [SerializeField]
        private Transform _container;
        [SerializeField]
        private GameObject _taskPrefab;

        private Dictionary<int, TaskObjectView> _taskCollection = new();

        public void AddTask(TaskModel task)
        {
            var playerUI = CreateTaskView(task);

            _taskCollection[task.Id] = playerUI;
        }

        private TaskObjectView CreateTaskView(TaskModel task)
        {
            GameObject objectView = Instantiate(_taskPrefab, _container, false);
            var view = objectView.GetComponent<TaskObjectView>();

            view.InitUI(task);

            return view;
        }

        public void CompeleteTask(int id)
        {
            var task = _taskCollection[id];
            task.SetCompele();
        }
    }
}
