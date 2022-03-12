using System.Collections;
using System.Collections.Generic;
using UCM.IAV.Navegacion;
using UnityEngine;
using UnityEngine.UI;

public class HeuristicButton : MonoBehaviour
{
    public Button heuButton;
    TesterGraph testGraph;
    GameManager gM;
    // Start is called before the first frame update
    void Start()
    {
        Button btn = heuButton.GetComponent<Button>();
        btn.onClick.AddListener(onClick);  

        heuButton.GetComponentInChildren<Text>().text = detectHeuristic();

        gM = GameManager.instance;
        testGraph = gM.GetTesterGraph() as TesterGraph;
    }

    void onClick()
    {
        Debug.Log(testGraph.heu);
        heuButton.GetComponentInChildren<Text>().text = detectHeuristic();
        gM.ChangeHeuristica();
    }

    string detectHeuristic()
    {
        if (testGraph.heu == AStarHeuristic.Manhattan)
        {
            return "EUCLIDEO";
        }
        else if (testGraph.heu == AStarHeuristic.Euclideo)
        {
            return "MANHATTAN";
        }
        else return "Maze_Heuristic_01";
    }
}