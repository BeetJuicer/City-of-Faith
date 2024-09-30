using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Sequence,
    Gameplay,
    Edit_Mode,
}

public class GameManager : MonoBehaviour
{
    public GameState CurrentGameState { get; private set; }

    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } private set { } }

    private void Awake()
    {
        if (_instance != null)
            Destroy(this);
        else
            _instance = this;
    }

    public void ChangeGameState(GameState state)
    {
        CurrentGameState = state;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
