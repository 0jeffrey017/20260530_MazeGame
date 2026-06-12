using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameClearView :MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI message;
        [SerializeField] private Image backGround;
        [SerializeField] private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup.alpha = 0;
        }

        public void SetGameClearUI(bool isWinner)
        {
            canvasGroup.alpha = 1;
            message.text = isWinner ? "You Win!" : "You Lose!";
            backGround.color = isWinner ? Color.darkGreen : Color.brown;
        }
    }
}