using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;
using UnityEngine.UI;

public class SmoothPathButton : MonoBehaviour
{
    public Button smoothBut;
    TesterGraph testGraph;
    GameManager gM;
    // Start is called before the first frame update
    void Start()
    {
        gM = GameManager.instance;
        testGraph = gM.GetTesterGraph() as TesterGraph;

        Button btn = smoothBut.GetComponent<Button>();
        btn.onClick.AddListener(onClick);

        smoothBut.GetComponentInChildren<Text>().text = detectSmooth();
    }

    void onClick()
    {
        Debug.Log(testGraph.heu);
        gM.ChangeSmoothPath();
        smoothBut.GetComponentInChildren<Text>().text = detectSmooth();
    }

    string detectSmooth()
    {
        if (!testGraph.smoothPath)
            return "Inactive";
        else
            return "Active";
    }
}