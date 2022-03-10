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

        TesterGraph tstGph;

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
            tstGph = gM.GetTesterGraph();
        }

        public override Direccion GetDirection()
        {
            Direccion direccion = new Direccion();
            if (!this.enabled) return direccion;
            //GET MAIN DIRECTION
            direccion.lineal = PlayerDetection(transform.forward, minotaurSight);

            if (!objectiveReached)
            {
                //Obtencion de las posibles casillas del tablero
                mapCells = graph.getVertex();
                //Asignacion de objetivo
                //Obtencion del camino
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
                        path = tstGph.getPathToNodeFrom(player, this.gameObject);
                        pathCount = 0;
                        //Reset timer y objetivo
                        objectiveReached = true;
                    }
                }
            }
            else if(chasing)
            {
                Debug.Log("Hola holita vecinito");
                //Seguir camino si existe
                if (path.Count > 0 && pathCount >= 0)
                {
                    direccion.lineal = path[pathCount].transform.position - this.gameObject.transform.position;   //Sigue el Camino
                    //Si ha llegado a la siguiente casilla
                    if (direccion.lineal.magnitude <= 0.2)
                    {
                        if (pathCount < path.Count - 1)
                            pathCount++;    //Avanza el camino
                        else
                            objectiveReached = true;    //Objetivo alcanzado
                    }
                    direccion.lineal.Normalize();
                    direccion.lineal *= agente.aceleracionMax;
                }
            }
            // Podrú}mos meter una rotación automática en la dirección del movimiento, si quisiéramos
            return direccion;
        }

        public Vector3 PlayerDetection(Vector3 directionRay, float distance)
        {
            Vector3 directionAcc = new Vector3();

            if (chasing)
            {
                Vertex v = graph.GetNearestVertex(transform.position);
                pathToFollow = tstGph.getPathToNodeFrom(v.gameObject, player);
            }
            else
            {
                Vector3 from = transform.position;

                from.y = from.y + 2.3f;
                RaycastHit hit;

                Vector3 playerDir = -(from - player.transform.position);

                if (Physics.Raycast(from, playerDir, out hit, distance))
                {
                    // Find the line from the gun to the point that was clicked.
                    Vector3 incomingVec = hit.point - transform.position;
                    // Use the point's normal to calculate the reflection vector.
                    Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                    Debug.DrawRay(from, playerDir, Color.green);
                    // Draw lines to show the incoming "beam" and the reflection.
                    //Debug.DrawLine(from, hit.point, Color.red);
                    //Debug.DrawRay(hit.point, reflectVec, Color.blue);

                    Debug.Log(hit.collider);
                    if (hit.collider.gameObject.GetComponent<ControlJugador>() != null)
                    {
                        //Debug.Log("Hola holita vecinito");
                        //Vector3 dir = hit.point + hit.normal * avoidDistance;
                        //directionAcc += dir;

                        transform.LookAt(hit.collider.gameObject.transform);

                        Vertex v = graph.GetNearestVertex(transform.position);
                        GameObject endMaze = player;
                        pathToFollow = tstGph.getPathToNodeFrom(v.gameObject, endMaze);

                        chasing = true;

                        hit.collider.gameObject.GetComponent<Wander>().enabled = false;
                    }
                }
            }
            return directionAcc;
        }
    }

}
