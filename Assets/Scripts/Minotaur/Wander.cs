using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Movimiento {
    public class Wander : ComportamientoAgente{
        [Range(0f,360f)]
        public float angularThreshold;

        float timeToChange = 0.4f;
        float auxFactor = 0.7f;
        float timeSinceLastChange;
        Direccion direction;
        bool change;

        public LayerMask layer;

        [Range(0, 100)]
        public float probToChange;

        [SerializeField]
        float avoidDistance;

        //Distancia de vision del raycast
        public float lookAhead = 3.0f;

        public override void Start(){
            timeSinceLastChange = 0;
            change = false;
            direction = new Direccion();
            //Starts off with random ori
            direction.orientation = Random.Range(0, 361);
        }

        public override Direccion GetDirection(){
            //Decidir si sumar o no
            int changeProb = Random.Range(0, 2000);
            if(changeProb <= probToChange && !change){
                change = true;
                int addProb = Random.Range(0, 11);
                //Decidir si sumar o restar
                if (addProb <= 5) auxFactor = auxFactor * -1;

                timeToChange = Random.Range(0.2f, 0.5f);
                timeSinceLastChange = 0;
            }

            //Sumar
            if (change && timeSinceLastChange <= timeToChange){
                timeSinceLastChange += Time.deltaTime;
                direction.orientation += auxFactor;
            }
            else change = false;
       
            //Dar direccion
            direction.lineal = OriToVec(direction.orientation);

            Vector3 directionRay = transform.forward;
            direction.lineal += WallAvoidance(directionRay, lookAhead);
            directionRay = transform.forward + transform.right;
            direction.lineal += WallAvoidance(directionRay, lookAhead * 0.25f);
            directionRay = transform.forward + (transform.right*-1);
            direction.lineal += WallAvoidance(directionRay, lookAhead * 0.25f);

            direction.lineal.Normalize();

            if(agente != null)
                direction.lineal *= agente.aceleracionMax;

            return direction;
        }

        public Vector3 WallAvoidance(Vector3 directionRay, float distance) {
            Vector3 directionAcc = new Vector3();
            Vector3 from = transform.position;

            from.y = from.y +  0.3f;
            RaycastHit hit;
            if (Physics.Raycast(from, directionRay, out hit, distance, layer))
            {
                // Find the line from the gun to the point that was clicked.
                Vector3 incomingVec = hit.point - transform.position;
                // Use the point's normal to calculate the reflection vector.
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                // Draw lines to show the incoming "beam" and the reflection.
                Debug.DrawLine(from, hit.point, Color.red);
                Debug.DrawRay(hit.point, reflectVec, Color.blue);

                Vector3 dir = hit.point + hit.normal * avoidDistance;
                timeSinceLastChange = timeToChange;
                directionAcc += dir;
            }
            else
                Debug.DrawRay(from, directionRay * distance, Color.green);

            return directionAcc;
        }
    }
}
