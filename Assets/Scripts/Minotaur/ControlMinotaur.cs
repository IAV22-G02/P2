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

            direccion.orientation = Vector3.SignedAngle(Vector3.forward, new Vector3(direccion.lineal.x, 0.0f, direccion.lineal.z), Vector3.up);

            // Podríamos meter una rotación automática en la dirección del movimiento, si quisiéramos
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
