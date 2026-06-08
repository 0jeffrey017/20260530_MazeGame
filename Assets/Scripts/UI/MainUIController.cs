using TMPro;
using UnityEngine;
using VContainer;

namespace UI
{
    public class MainUIController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        
        private GameMode _gameMode;

        [Inject]
        public void Construct(GameMode gameMode)
        {
            _gameMode = gameMode;
        }

        private void Start()
        {
            if (_gameMode != null)
            {
                _gameMode.OnHaveKeyChanged += OnHaveKeyChanged;
                
                UpdateVisual(_gameMode.CurrentHaveKeyStatus);
            }
        }

        private void OnDestroy()
        {
            if (_gameMode != null)
            {
                _gameMode.OnHaveKeyChanged -= OnHaveKeyChanged;
            }
        }
        private void OnHaveKeyChanged(bool previousValue, bool newValue)
        {
            UpdateVisual(newValue);
        }

        private void UpdateVisual(bool hasKey)
        {
            text.text = $"HaveKey: {hasKey}";
        }
    }
}