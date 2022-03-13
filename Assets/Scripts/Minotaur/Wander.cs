namespace UCM.IAV.Movimiento {

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UCM.IAV.Navegacion;

    public class Wander : ComportamientoAgente{

        GraphGrid graph;
        TesterGraph testGraph;
        GameManager gM;

        ControlMinotaur ctrlMinot;

        float minTimeToChange, maxTimeToChange, timeToChange, timeSinceLastChange;

        bool objectiveReached;

        GameObject[] mapCells;
        int target = -1;

        List<Vertex> path;
        int pathCount = 0;

        Vertex lastCell, currentCell;

        double time = 0;
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
            ctrlMinot = GetComponent<ControlMinotaur>();
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
                Vector3 directionRay = -transform.up;
                Vector3 from = this.transform.position;
                from.y += 0.5f;
                RaycastHit hit;
                if (Physics.Raycast(from, directionRay, out hit, 1))
                {
                    Debug.DrawRay(from, directionRay, Color.yellow);
                    GameObject vert = hit.collider.gameObject;
                    if (vert.GetComponent<Vertex>())
                    {
                        path = testGraph.getPathToNodeFrom(mapCells[target], vert, ref time);
                        pathCount = 0;
                        lastCell = currentCell = vert.GetComponent<Vertex>();
                        ctrlMinot.setCostCell(ref lastCell, graph.minotaurCost);
                        //Tiempo límite para alcanzar el lugar
                        timeToChange = timeSinceLastChange + Random.Range(minTimeToChange, maxTimeToChange) / 1000;
                        //Reset timer y objetivo
                        objectiveReached = false;
                    }
                }
            }
            else
            {
                //Seguir camino si existe
                if (path.Count > 0 && pathCount >= 0){
                    direction.lineal = path[pathCount].transform.position - this.gameObject.transform.position;   //Sigue el Camino
                    //Si ha llegado a la siguiente casilla
                    if (direction.lineal.magnitude <= 0.4) {
                        if (pathCount < path.Count - 1){
                            pathCount++;    //Avanza el camino
                            lastCell = path[pathCount - 1];
                            ctrlMinot.setCostCell(ref lastCell, graph.defaultCost);
                            currentCell = path[pathCount];
                            ctrlMinot.setCostCell(ref currentCell, graph.minotaurCost);
                        }
                        else
                            objectiveReached = true;    //Objetivo alcanzado
                    }
                    direction.lineal.Normalize();
                    direction.angular = agente.aceleracionAngularMax;
                    direction.lineal *= agente.aceleracionMax;
                }
                timeSinceLastChange += Time.deltaTime;
            }
            //Debug.Log(direction.lineal);
            //test
            this.gameObject.GetComponent<Rigidbody>().AddForce(direction.lineal);

            return direction;
        }
    }
}
