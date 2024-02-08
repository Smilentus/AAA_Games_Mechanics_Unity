using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class DebugNetworkCanvasController : MonoBehaviour
{
    public Button hostButton;
    public Button serverButton;
    public Button clientButton;


    private void Start()
    {
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartHost();
                this.gameObject.SetActive(false);
            });
        }

        if (serverButton != null)
        {
            serverButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartServer();
                this.gameObject.SetActive(false);
            });
        }

        if (clientButton != null)
        {
            clientButton.onClick.AddListener(() =>
            {
                NetworkManager.Singleton.StartClient();
                this.gameObject.SetActive(false);
            });
        }
    }
}
