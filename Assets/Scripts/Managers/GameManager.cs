using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Maze;
using UCM.IAV.Navegacion;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    MazeGenerator mazeGen;

    Graph graph;
    TesterGraph testGraph;
    GameObject playerInstance;



    // Start is called before the first frame update
    void Awake(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            mazeGen = new MazeGenerator();
        }
        else{
            Destroy(this.gameObject);
        }
    }

    public void Update()
    {

    }


    public void ChangeHeuristica()
    {
        testGraph.heu = (testGraph.heu == AStarHeuristic.Manhattan) ? AStarHeuristic.Euclideo : AStarHeuristic.Manhattan;
    }

    public void SetGraph(Graph gph){
        this.graph = gph;
    }
    public Graph GetGraph()
    {
        return this.graph;
    }
    public void SetTesterGraph(TesterGraph gph)
    {
        this.testGraph= gph;
    }
    public TesterGraph GetTesterGraph()
    {
        return this.testGraph;
    }

    public void SetPlayer(GameObject player)
    {
        playerInstance = player;
    }
    public GameObject GetPlayer()
    {
        return playerInstance;
    }


    public void createMap(int width, int height){
        mazeGen.setSize(width, height);

        while (!mazeGen.getStart() || !mazeGen.getFinish()){
            mazeGen.Generate();
            mazeGen.Display();

        }
    }
    
    public void changeScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void win()
    {
        SceneManager.LoadScene("Win");
    }

    public void loadMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}
