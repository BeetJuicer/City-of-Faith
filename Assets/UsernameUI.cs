using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TMPro;
public class UsernameUI : MonoBehaviour
{
    [SerializeField] private TMP_Text usernameText;
    Database database;
    // Start is called before the first frame update
    void Start()
    {
        database = FindObjectOfType<Database>();
        usernameText.text = database.Username;
    }

}
