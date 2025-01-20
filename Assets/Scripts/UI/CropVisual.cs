using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropVisual : MonoBehaviour
{
    private GameObject growingGO;
    private GameObject ripeGO;

    private const string GROWING_GO_STRING = "V_Growing";
    private const string RIPE_GO_STRING = "V_Ripe";

    private void Awake()
    {
        growingGO = transform.Find(GROWING_GO_STRING).gameObject;
        ripeGO = transform.Find(RIPE_GO_STRING).gameObject;

        Debug.Assert(growingGO != null, "Growing Visual not found!");
        Debug.Assert(ripeGO != null, "Ripe Visual not found!");
    }

    public void UpdateVisual(Plot.PlotState state)
    {
        print($"db_logs: updating state as {state}");
        switch (state)
        {
            case Plot.PlotState.EMPTY:
                growingGO.SetActive(false);
                ripeGO.SetActive(false);
                break;
            case Plot.PlotState.GROWING:
                growingGO.SetActive(true);
                break;
            case Plot.PlotState.RIPE:
                growingGO.SetActive(false);
                ripeGO.SetActive(true);
                break;
        }
    }
}
