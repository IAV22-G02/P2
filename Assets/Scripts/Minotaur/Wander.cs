namespace UCM.IAV.Movimiento {

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UCM.IAV.Navegacion;

    public class Wander : ComportamientoAgente{

        GraphGrid graph;
        TesterGraph testGraph;
        GameManager gM;

        float minTimeToChange, maxTimeToChange, timeToChange, timeSinceLastChange;

        Direccion direction;
        bool objectiveReached;

        GameObject[] mapCells;
        int target = -1;

        public override void Start()
        {
            base.Start();
            gM = GameManager.instance;
            graph = gM.GetGraph() as GraphGrid;
            testGraph = gM.GetTesterGraph() as TesterGraph; 
            objectiveReached = false;
            minTimeToChange = 10000;
            maxTimeToChange = minTimeToChange + 8000;
            timeToChange = 0;
            timeSinceLastChange = 0;
        }

        public override void Update()
        {
            base.Update();
            //Comprobar si ha llegado al destino
            if (!objectiveReached && mapCells != null) {
                objectiveReached = (graph.GetNearestVertex(this.gameObject.transform.position) == graph.GetNearestVertex(mapCells[target].transform.position));
            }
        }


        public override Direccion GetDirection(){
            //Decidir si continuar o cambiar
            if (objectiveReached || timeSinceLastChange >= timeToChange){
                //Obtencion de la posicion relativa respecto al tablero
                //GameObject ori = graph.GetNearestVertex(this.gameObject.transform.position);
                //Obtencion de las posibles casillas del tablero
                mapCells = graph.getVertex();
                //Destino objetivo random
                int target = Random.Range(0, mapCells.Length);
                //Asignacion de objetivo
                testGraph.getPathToNodeFrom(this.gameObject, mapCells[target]);
                //Tiempo límite para alcanzar el lugar
                timeToChange = timeSinceLastChange + Random.Range(minTimeToChange, maxTimeToChange) / 1000;
                //Reset timer y objetivo
                objectiveReached = false;
            }
            else
                timeSinceLastChange += Time.deltaTime;

            Debug.Log(timeSinceLastChange + "Yepa" + timeToChange);
            return direction;
        }
    }
}
