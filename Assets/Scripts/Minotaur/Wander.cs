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

        bool objectiveReached;

        GameObject[] mapCells;
        int target = -1;

        List<Vertex> path;
        int pathCount = 0;

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
            //if (!objectiveReached && mapCells != null && target >= 0)
            //{
            //    objectiveReached = (graph.GetNearestVertex(this.gameObject.transform.position) == graph.GetNearestVertex(mapCells[target].transform.position));
            //}
        }


        public override Direccion GetDirection(){
            Direccion direction = new Direccion();
            //Decidir si continuar o cambiar
            if (objectiveReached || timeSinceLastChange >= timeToChange)
            {
                //Obtencion de las posibles casillas del tablero
                mapCells = graph.getVertex();
                //Destino objetivo random
                int target = Random.Range(0, mapCells.Length);
                //Asignacion de objetivo
                path = testGraph.getPathToNodeFrom(mapCells[target], this.gameObject);
                pathCount = 0;
                //Tiempo límite para alcanzar el lugar
                timeToChange = timeSinceLastChange + Random.Range(minTimeToChange, maxTimeToChange) / 1000;
                //Reset timer y objetivo
                objectiveReached = false;
            }
            else
            {
                //Seguir camino si existe
                if (path.Count > 0 && pathCount >= 0){
                    direction.lineal = path[pathCount].transform.position - this.gameObject.transform.position;   //Sigue el Camino
                    //Si ha llegado a la siguiente casilla
                    if (direction.lineal.magnitude <= 0.2) {
                        if (pathCount < path.Count - 1)
                            pathCount++;    //Avanza el camino
                        else
                            objectiveReached = true;    //Objetivo alcanzado
                    }
                    direction.lineal.Normalize();
                    direction.lineal *= agente.aceleracionMax;
                }
                timeSinceLastChange += Time.deltaTime;
            }
            //test
            this.gameObject.GetComponent<Rigidbody>().AddForce(direction.lineal);

            return direction;
        }
    }
}
