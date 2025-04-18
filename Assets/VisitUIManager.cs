using TMPro;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.UI;

public class VisitUIManager : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private GameObject HUDCanvas;
    [SerializeField] private TextMeshProUGUI m_friendUsername;
    [SerializeField] private TextMeshProUGUI m_friendLevel;
    FriendCentralHall friendHall;

    [Header("Central Hall UI")]
    [SerializeField] public GameObject friendHallCanvasGO; //cant serialize interfaces.
    private ICanvasView friendHallCanvas; //workaround for interface serialization.

    private void Start()
    {
        string username = VisitCloudLoader.Instance.friendUsername;
        username = username.Split('#')[0]; // Remove the tag. Username format is username#1111

        m_friendUsername.text = $"{username}'s City";
        m_friendLevel.text = VisitCloudLoader.Instance.VillageData.level.ToString();

        //FriendHallCanvas stuff.
        friendHallCanvas = friendHallCanvasGO.GetComponent<ICanvasView>();
        friendHall = FindFirstObjectByType<FriendCentralHall>();

        friendHall.FriendHallClicked += FriendHallClicked;
        friendHallCanvas.Canvas_Deactivated += Canvas_Deactivated;
    }

    private void OnDestroy()
    {
        friendHall.FriendHallClicked -= FriendHallClicked;
        friendHallCanvas.Canvas_Deactivated -= Canvas_Deactivated;
    }

    private void Canvas_Deactivated()
    {
        HUDCanvas.SetActive(true);
    }

    private void FriendHallClicked()
    {
        HUDCanvas.SetActive(false);
        friendHallCanvas.Activate();
    }

    public void ReturnHome()
    {
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(2);
    }

}
