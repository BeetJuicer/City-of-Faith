using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using TMPro;

public class PasswordClickDebug : MonoBehaviour
{
    [SerializeField] string pass;
    [SerializeField] TMP_InputField password;
    public void SetPass()
    {
        password.text = pass;
    }
}
