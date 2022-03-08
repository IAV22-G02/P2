/*    
   Copyright (C) 2020 Federico Peinado
   http://www.federicopeinado.com

   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Informática de la Universidad Complutense de Madrid (España).

   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento
{

    using UnityEngine;
    using UCM.IAV.Navegacion;
    using System.Collections.Generic;
    /// <summary>
    /// Clara para el comportamiento de agente que consiste en ser el jugador
    /// </summary>
    public class ControlJugador: ComportamientoAgente
    {

        // AudioSource audio
        // ;
        [SerializeField]
        float rotationSpeed;
        GameManager gM;
        Rigidbody rb;
        Animator animC;

        [SerializeField]
        GraphGrid graphGrid;

        [SerializeField]
        TesterGraph tstGph;

        List<Vertex> pathToFollow;

        GameObject endMaze;
        public override void Start(){
            base.Start();
            rb = GetComponent<Rigidbody>();
            animC = GetComponentInChildren<Animator>();

            if (graphGrid == null) Debug.LogError("No hay graphGrid");
            endMaze = graphGrid.getEndMaze();

            if (endMaze == null) Debug.LogError("No hay final de Maze");

            if (tstGph == null) Debug.LogError("No hay TesterGraph");

            pathToFollow = new List<Vertex>();

        }
        public override void Awake(){
            base.Awake();
            gM = GameManager.instance;  
            gM.SetPlayer(this.gameObject);
        }
        public override void Update(){
            base.Update();

            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
                animC.SetBool("running", true);
            else
                animC.SetBool("running", false);


            if (Input.GetKey(KeyCode.Space)){
                Debug.Log("Calculating Path");
                Vertex v = graphGrid.GetNearestVertex(transform.position);
                pathToFollow = tstGph.getPathToNodeFrom(v.gameObject, endMaze);
            }

            if (pathToFollow.Count != 0){
                Debug.Log("Showing Path");
                tstGph.ShowPath(pathToFollow, Color.white);
                //pathToFollow = new List<Vertex>();
            }
        }

        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        /// <returns></returns>
        public override Direccion GetDirection(){
            Direccion direccion = new Direccion(rotationSpeed);
            direccion.lineal.x = Input.GetAxis("Vertical");
            direccion.lineal.z = Input.GetAxis("Horizontal") * -1;
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;
            return direccion;
        }

        public void OnDrawGizmos()
        {
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
    }
}