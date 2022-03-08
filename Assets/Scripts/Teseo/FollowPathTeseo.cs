using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Movimiento
{
    using UnityEngine;
    using UCM.IAV.Navegacion;
    using System.Collections.Generic;
    public class FollowPathTeseo : ComportamientoAgente
    {
        GameManager gM;
        Rigidbody rb;
        Animator animC;

        [SerializeField]
        float rotationSpeed;

        [SerializeField]
        GraphGrid graphGrid;

        [SerializeField]
        TesterGraph tstGph;

        [SerializeField]
        float distanceThreshold;

        List<Vertex> pathToFollow;

        GameObject endMaze;

        bool following;

        int pathIterator;
        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            pathIterator = 0;
            rb = GetComponent<Rigidbody>();
            animC = GetComponentInChildren<Animator>();

            if (graphGrid == null) Debug.LogError("No hay graphGrid");
            endMaze = graphGrid.getEndMaze();

            if (endMaze == null) Debug.LogError("No hay final de Maze");

            if (tstGph == null) Debug.LogError("No hay TesterGraph");

            pathToFollow = new List<Vertex>();
        }

        // Update is called once per frame
        public override void Update() {
            if (Input.GetKeyDown(KeyCode.Space)){
                pathIterator = 0;
                animC.SetBool("running", true);
            }

            if (Input.GetKey(KeyCode.Space)) {
                following = true;
                animC.SetBool("running", true);
                Debug.Log("Calculating Path");
                Vertex v = graphGrid.GetNearestVertex(transform.position);
                pathToFollow = new List<Vertex>();
                pathToFollow = tstGph.getPathToNodeFrom(endMaze, v.gameObject);
            }
            else {
                animC.SetBool("running", false);
                following = false;
            }

            if (pathToFollow.Count != 0)
            {
                Debug.Log("Showing Path");
                tstGph.ShowPath(pathToFollow, Color.white);
                //pathToFollow = new List<Vertex>();
            }
        }

        public void OnDrawGizmos() {
            if (!Application.isPlaying)
                return;

            if (ReferenceEquals(graphGrid, null))
                return;

            Vertex v;

            int i;
            Gizmos.color = Color.yellow;
            for (i = 0; i < pathToFollow.Count; i++)
            {
                v = pathToFollow[i];
                Gizmos.DrawSphere(v.transform.position, 0.433f);
                //if (smoothPath && i != 0)
                //{
                //    Vertex prev = path[i - 1];
                //    Gizmos.DrawLine(v.transform.position, prev.transform.position);

                //}
            }
        }

        public override Direccion GetDirection(){
            Direccion direccion = new Direccion();
            if (following && pathToFollow.Count != 0){
                direccion.angular = rotationSpeed;
                direccion.lineal = pathToFollow[pathIterator].transform.position - transform.position;
                if (direccion.lineal.magnitude <= distanceThreshold)
                    pathIterator++;

                direccion.lineal.Normalize();
                direccion.lineal *= agente.aceleracionMax;
            }
            return direccion;
        }
    }
}
