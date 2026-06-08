using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class NetworkUiManager : MonoBehaviour
    {
        [SerializeField] private Button clientButton;
        [SerializeField] private Button serverButton;
        [SerializeField] private CanvasGroup canvasGroup;

        void OnEnable()
        {
            clientButton.onClick.AddListener(OnClientButtonClicked);
            serverButton.onClick.AddListener(OnServerButtonClicked);
        }
        private void OnDisable()
        {
            clientButton.onClick.RemoveListener(OnClientButtonClicked);
            serverButton.onClick.RemoveListener(OnServerButtonClicked);
        }
        private void OnClientButtonClicked()
        { 
            NetworkManager.Singleton.StartClient();
            HideMenuCanvas();
        }

        private void OnServerButtonClicked()
        {
            NetworkManager.Singleton.StartServer();
            HideMenuCanvas();
        }

        private void HideMenuCanvas()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}