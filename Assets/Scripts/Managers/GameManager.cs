using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Maze;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    MazeGenerator mazeGen;
    // Start is called before the first frame update
    void Start(){
        if (instance == null){
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            mazeGen = new MazeGenerator();
        }
        else{
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createMap(int width, int height){
        Debug.Log("hola");
        mazeGen.setSize(width, height);

        while (!mazeGen.getStart() || !mazeGen.getFinish()){
            mazeGen.Generate();
            mazeGen.Display();

        }
    }
}
