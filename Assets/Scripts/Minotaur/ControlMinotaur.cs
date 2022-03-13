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

        Vertex  lastCell, currentCell; 

        List<Vertex> path;

        int pathCount = 0;


        public override void Start()
        {
            base.Start();
            gM = GameManager.instance;
            player = gM.GetPlayer();
            graph = gM.GetGraph() as GraphGrid;
            testGraph = gM.GetTesterGraph();
            lostFocusTime = 4.0f;
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
                this.transform.LookAt(player.transform.position);
                if (path == null || pathCount >= path.Count)
                {
                    //Asignacion de objetivo
                    Vector3 directionRay = -transform.up;
                    Vector3 from = this.transform.position;
                    from.y += 0.5f;
                    RaycastHit hit, hit2;
                    if (Physics.Raycast(from, directionRay, out hit, 1))
                    {
                        GameObject vertMin = hit.collider.gameObject;
                        from = player.transform.position; from.y += 0.5f;
                        if (Physics.Raycast(from, directionRay, out hit2, 1))
                        {
                            GameObject vertPlayer = hit2.collider.gameObject;
                            if (vertMin.GetComponent<Vertex>() && vertPlayer.GetComponent<Vertex>())
                            {
                                path = testGraph.getPathToNodeFrom(vertPlayer, vertMin);
                                lastCell = currentCell = vertMin.GetComponent<Vertex>();
                                setCostCell(ref lastCell, graph.minotaurCost);
                                pathCount = 0;
                            }
                        }
                    }
                }
                else
                {
                    //Seguir camino si existe
                    if (path.Count > 0 && pathCount >= 0)
                    {
                        direccion.lineal = path[pathCount].transform.position - this.gameObject.transform.position;   //Sigue el Camino
                                                                                                                      //Si ha llegado a la siguiente casilla
                        if (direccion.lineal.magnitude <= 0.8)
                        {
                            if (pathCount < path.Count - 1)
                            {
                                pathCount++;    //Avanza el camino
                                lastCell = path[pathCount - 1];
                                setCostCell(ref lastCell, graph.defaultCost);
                                currentCell = path[pathCount];
                                setCostCell(ref currentCell, graph.minotaurCost);
                            }
                            else if (pathCount == path.Count - 1)
                                pathCount++;
                            else
                                return direccion;
                        }
                        direccion.lineal.Normalize();
                        direccion.angular = agente.aceleracionAngularMax;
                        direccion.lineal *= agente.aceleracionMax;
                    }
                }
                //Debug.Log(direccion.lineal);
                timeSinceLastChange += Time.deltaTime;

                return direccion;
            }
        }

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
                    this.GetComponent<Wander>().enabled = false;
                    Debug.DrawRay(from, directionRay*distance, Color.green);
                }

            }

            if (timeSinceLastChange >= timeToChange)
            {
                Debug.DrawRay(from, directionRay*distance, Color.white);    
                chasing = false;
                this.GetComponent<Wander>().enabled = true;
            }
        }

        public void setCostCell(ref Vertex vertex, in float cost){
            if (vertex.vecinos.Count > 0)
            {
                foreach(Edge e in vertex.vecinos){
                    e.cost = cost;
                }
            }
        }

        public void OnTriggerEnter(Collider collision)
        {
            if (collision.gameObject == player){
                player.GetComponent<ControlJugador>().isNearMinotaur(true);
            }
        }
        public void OnTriggerExit(Collider collision)
        {
            if (collision.gameObject == player){
                player.GetComponent<ControlJugador>().isNearMinotaur(false);
            }
        }
    }

}
