namespace UCM.IAV.Movimiento
{
    using UnityEngine;
    using System.Collections.Generic;
    using UCM.IAV.Navegacion;

    public class ControlMinotaur : ComportamientoAgente
    {

        public GraphGrid graph;
        GameManager gM;
        GameObject player;
        public float avoidDistance;

        public float minotaurSight;
        List<Vertex> pathToFollow;

        TesterGraph testGraph;

        float lostFocusTime, timeToChange, timeSinceLastChange;
        bool chasing = false;
        private bool objectiveReached = false;

        GameObject[] mapCells;

        List<Vertex> path;

        int pathCount = 0;

        public override void Start()
        {
            base.Start();
            gM = GameManager.instance;
            player = gM.GetPlayer();
            graph = gM.GetGraph() as GraphGrid;
            testGraph = gM.GetTesterGraph();
            lostFocusTime = 6.0f;
            timeToChange = 0;
            timeSinceLastChange = 0;
        }
        public override void Update()
        {
            base.Update();
        }

        public override Direccion GetDirection()
        {
            Direccion direccion = new Direccion();
            if (!this.enabled) return direccion;
            //GET Chasing State
            PlayerDetection(transform.forward, minotaurSight);
            //Si no caza
            if (!chasing)
            {

                return direccion;
            }
            else
            {
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
                        path = testGraph.getPathToNodeFrom(player, vert);
                        pathCount = 0;
                    }
                }
                //Seguir camino si existe
                if (path.Count > 0 && pathCount >= 0)
                {
                    direccion.lineal = -(path[pathCount].transform.position - this.gameObject.transform.position);   //Sigue el Camino
                                                                                                                     //Si ha llegado a la siguiente casilla
                    if (direccion.lineal.magnitude <= 0.8)
                    {
                        if (pathCount < path.Count - 1)
                            pathCount++;    //Avanza el camino
                    }
                    direccion.lineal.Normalize();
                    direccion.angular = agente.aceleracionAngularMax;
                    direccion.lineal *= agente.aceleracionMax;
                }
                Debug.Log(direccion.lineal);
                timeSinceLastChange += Time.deltaTime;

                return direccion;
            }
        }


        //if (!objectiveReached)
        //{
        //    //Asignacion de objetivo
        //    //Obtencion del camino
        //    Vector3 directionRay = -transform.up;
        //    Vector3 from = this.transform.position;
        //    from.y += 0.5f;
        //    RaycastHit hit;
        //    if (Physics.Raycast(from, directionRay, out hit, 1))
        //    {
        //        Debug.DrawRay(from, directionRay, Color.yellow);
        //        GameObject vert = hit.collider.gameObject;
        //        if (vert.GetComponent<Vertex>())
        //        {
        //            path = tstGph.getPathToNodeFrom(player, this.gameObject);
        //            pathCount = 0;
        //            //Reset timer y objetivo
        //            objectiveReached = true;
        //        }
        //    }
        //}
        //else if(chasing)
        //{
        //    Debug.Log("Hola holita vecinito");
        //    //Seguir camino si existe
        //    if (path.Count > 0 && pathCount >= 0)
        //    {
        //        direccion.lineal = path[pathCount].transform.position - this.gameObject.transform.position;   //Sigue el Camino
        //        //Si ha llegado a la siguiente casilla
        //        if (direccion.lineal.magnitude <= 0.2)
        //        {
        //            if (pathCount < path.Count - 1)
        //                pathCount++;    //Avanza el camino
        //            else
        //                objectiveReached = true;    //Objetivo alcanzado
        //        }
        //        direccion.lineal.Normalize();
        //        direccion.lineal *= agente.aceleracionMax;
        //    }
        //}
        //// Podr僘mos meter una rotaci autom疸ica en la direcci del movimiento, si quisi駻amos
        //return direccion;
    

        public void PlayerDetection(Vector3 directionRay, float distance)
        {
            Vector3 from = transform.position;
            from.y += 0.5f;
            RaycastHit hit;
            if (Physics.Raycast(from, directionRay, out hit, distance))
            {
                if (hit.collider.gameObject == player)
                {
                    //Timer para que deje de seguir al jugador si no lo ve
                    timeToChange = timeSinceLastChange + lostFocusTime;
                    chasing = true;
                }
            }

            if (timeSinceLastChange >= timeToChange)
                chasing = false;
        }
    }

}
