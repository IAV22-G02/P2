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


        public override void Start()
        {
            base.Start();
            gM = GameManager.instance;
            player = gM.GetPlayer();
            graph = gM.GetGraph() as GraphGrid;
        }

        public override void Awake()
        {
            base.Awake();

        }

        // Update is called once per frame
        public override void Update()
        {
            base.Update();
        }

        public override void LateUpdate()
        {
            base.LateUpdate();


        }

        public override Direccion GetDirection()
        {

            Direccion direccion = new Direccion();
            if (!this.enabled) return direccion;

            //GET MAIN DIRECTION
            direccion.lineal = PlayerDetection(transform.forward, minotaurSight);

            // Podr�amos meter una rotaci�n autom�tica en la direcci�n del movimiento, si quisi�ramos
            return direccion;
        }

        public Vector3 PlayerDetection(Vector3 directionRay, float distance)
        {

            Vector3 directionAcc = new Vector3();
            Vector3 from = transform.position;

            from.y = from.y + 0.3f;
            RaycastHit hit;

            Debug.DrawRay(from, directionRay * distance, Color.green);

            if (Physics.Raycast(from, directionRay, out hit, distance))
            {
                // Find the line from the gun to the point that was clicked.
                Vector3 incomingVec = hit.point - transform.position;
                // Use the point's normal to calculate the reflection vector.
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                // Draw lines to show the incoming "beam" and the reflection.
                Debug.DrawLine(from, hit.point, Color.red);
                Debug.DrawRay(hit.point, reflectVec, Color.blue);
                if (hit.collider.gameObject.GetComponent<ControlJugador>() != null)
                {

                    Vector3 dir = hit.point + hit.normal * avoidDistance;
                    directionAcc += dir;
                }
            }
            else
                Debug.DrawRay(from, directionRay * distance, Color.green);

            return directionAcc;
        }
    }

}
