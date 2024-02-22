using Tools;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace UI
{
    public class GameWonScreenUI : MonoBehaviour
    {
        [SerializeField]
        private Button _mainMenuButton;
        [SerializeField]
        private Button _exitGameButton;
        [SerializeField]
        private TMP_Text _earnerExperience;
        [SerializeField]
        private TMP_Text _currentLevel;
        [SerializeField]
        private TMP_Text _nextLevel;
        [SerializeField]
        private Slider _levelProgress;
        [SerializeField]
        private float _speedProgress = 0.5f;

        public Button MainMenuButton => _mainMenuButton;
        public Button ExitGameButton => _exitGameButton;

        private bool _isEnable;
        private float tempProgress;
        private int _partIndex;

        private int _currentLevelValue;
        private float _currentProgressValue;
        private float _earnedPoints;

        private List<float> _progressPartition = new();

        public void InitUI(int currentLevel, float currentProgress)
        {
            _currentLevel.text = currentLevel.ToString();
            _nextLevel.text = (currentLevel + 1).ToString();
            
            _levelProgress.maxValue = 100;
            _levelProgress.minValue = 0;

            _levelProgress.value = currentProgress;

            _currentLevelValue = currentLevel;
            _currentProgressValue = currentProgress;
        }

        public void UpdateProgression(float earnedPoints)
        {
            _earnedPoints = earnedPoints;
            _earnerExperience.text = _earnedPoints.ToString();

            _currentProgressValue = earnedPoints;

            int partiotionCount = 0;
            var partiotionProgressValue = _currentProgressValue;

            while (partiotionProgressValue / 100 > 1)
            {
                partiotionProgressValue -=  100;

                _progressPartition.Add(100);

                partiotionCount++;
            }
            _progressPartition.Add(partiotionProgressValue);

            _partIndex = 0;

            _isEnable = true;


            Debug.Log(_currentProgressValue);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }


        private void Update()
        {
            if (!_isEnable)
                return;

            _levelProgress.value
                    = Mathf.MoveTowards(_levelProgress.value, _progressPartition[_partIndex], Time.deltaTime * _speedProgress);
            
            _earnedPoints = Mathf.MoveTowards(_earnedPoints, 0, Time.deltaTime * _speedProgress);

            _earnerExperience.text = Mathf.Round(_earnedPoints).ToString();

            if (_levelProgress.value == 100)
            {
                _partIndex++;
                _levelProgress.value = 0;
                
                _currentLevelValue++;
                _currentLevel.text = _currentLevelValue.ToString();
                _nextLevel.text = (_currentLevelValue + 1).ToString();

                if (_partIndex == _progressPartition.Count)
                {
                    _partIndex = 0;
                    _isEnable = false;
                }
            }

        }

    }
}
