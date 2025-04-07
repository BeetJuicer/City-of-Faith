using TMPro;
using Unity.Services.Authentication;
using UnityEngine;

public class VisitUIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_friendUsername;
    [SerializeField] private TextMeshProUGUI m_friendLevel;

    private async void Start()
    {
        string username = VisitCloudLoader.Instance.friendUsername;
        username = username.Split('#')[0]; // Remove the tag. Username format is username#1111

        m_friendUsername.text = $"{username}'s City";
        m_friendLevel.text = VisitCloudLoader.Instance.VillageData.level.ToString();
    }

    public void ReturnHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2);
    }

}
