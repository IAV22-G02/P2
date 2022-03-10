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

        public override void Start()
        {
            base.Start();
            gM = GameManager.instance;
            player = gM.GetPlayer();
            graph = gM.GetGraph() as GraphGrid;
            tstGph = gM.GetTesterGraph();
        }

        //public override void Awake()
        //{
        //    base.Awake();

        //}

        //public override void LateUpdate()
        //{
        //    base.LateUpdate();
        //}

        public override Direccion GetDirection()
        {
            Direccion direccion = new Direccion();
            if (!this.enabled) return direccion;
            //Debug.Log("Hola holita vecinito");
            //GET MAIN DIRECTION
            direccion.lineal = PlayerDetection(transform.forward, minotaurSight);


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

                from.y = from.y + 0.3f;
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
                    if (hit.collider.gameObject.GetComponent<ControlJugador>() != null)
                    {
                        //Vector3 dir = hit.point + hit.normal * avoidDistance;
                        //directionAcc += dir;

                        transform.LookAt(hit.collider.gameObject.transform);

                        Vertex v = graph.GetNearestVertex(transform.position);
                        GameObject endMaze = player;
                        pathToFollow = tstGph.getPathToNodeFrom(v.gameObject, endMaze);

                        chasing = true;
                    }
                }
                

            }

            return directionAcc;
        }
    }

}
