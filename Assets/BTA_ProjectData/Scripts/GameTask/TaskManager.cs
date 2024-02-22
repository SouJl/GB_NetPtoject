using UnityEngine;

namespace GameTask
{
    public  class TaskManager : MonoBehaviour
    {
        [SerializeField]
        private TaskManagerView _view;

        private static int _taskId;

        private static TaskManagerView _staticView;

        private void Awake()
        {
            _taskId = 0;
            _staticView = _view;
        }

        public static int AddNewTask(string discriprion)
        {
            _staticView.AddTask(new TaskModel
            {
                Id = _taskId,
                Discription = discriprion
            });
            return _taskId++;
        }

        public static void TaskCompeleted(int id)
        {
            _staticView.CompeleteTask(id);
        }

        public static void Release()
        {
            _staticView.Clear();
            _taskId = 0;
        }
    }
}
