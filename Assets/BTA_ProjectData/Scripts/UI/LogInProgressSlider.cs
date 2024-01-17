using UnityEngine;
using UnityEngine.UI;

public class LogInProgressSlider : MonoBehaviour
{
    [SerializeField]
    private Slider _progressSlider;
    [SerializeField]
    private float _minValue = 0f;
    [SerializeField]
    private float _maxValue = 1f;
    [SerializeField]
    private float _smooth = 0.2f;


    private bool _startProgress;
    private float _currentProgress;

    public void Init()
    {
        _progressSlider.minValue = _minValue;
        _progressSlider.maxValue = _maxValue;

        _currentProgress = 0;
        _progressSlider.value = _currentProgress;

        _startProgress = false;

        Hide();
    }

    public void StartProgress()
    {
        _currentProgress = 0;
        _progressSlider.value = _currentProgress;

        _startProgress = true;

        Show();
    }

    public void Stop()
    {
        _currentProgress = 0;
        _progressSlider.value = _currentProgress;

        _startProgress = false;

        Hide();
    }

    public void UpdateProgress(float deltaTime)
    {
        if (!_startProgress)
            return;

        var resultValue = Mathf.Lerp(_progressSlider.minValue, _progressSlider.maxValue, _currentProgress);

        _currentProgress += deltaTime * _smooth;

        _progressSlider.value = resultValue;

        if (_currentProgress >= _progressSlider.maxValue)
            _currentProgress = 0;
    }

    private void Show() 
        => gameObject.SetActive(true);
    private void Hide() 
        => gameObject.SetActive(false);

}
