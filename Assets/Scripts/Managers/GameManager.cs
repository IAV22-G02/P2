using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Maze;
using UCM.IAV.Navegacion;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    MazeGenerator mazeGen;

    Graph graph;
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
    public void SetGraph(Graph gph){
        this.graph = gph;
    }
    public Graph GetGraph()
    {
        return this.graph;
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
}
