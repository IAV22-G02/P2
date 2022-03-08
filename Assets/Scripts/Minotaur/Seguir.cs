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
    using System.Collections.Generic;
    using UnityEngine;
    /// <summary>
    /// Clase para modelar el comportamiento de SEGUIR a otro agente
    /// </summary>
    public class Seguir : ComportamientoAgente
    {

        [SerializeField]
        float threshold;

        [SerializeField]
        float decayCoefficient;

        List<GameObject> targets;
        Rigidbody rb;

        SphereCollider sphColl;

        float radiusFactorSeparation = 4;

        float radius;

        #region WallAvoidance 
        //Capa de Colision
        public LayerMask layer;
        //Distancia minima a un muro(debe ser mayor al radio del gameobject
        [SerializeField]
        float avoidDistance;

        //Distancia de vision del raycast
        public float lookAhead = 3.0f;
        public float lookSide = 2.0f;
        #endregion

        public override void Start()
        {
            sphColl = GetComponent<SphereCollider>();
            radius = sphColl.radius;
            //objetivo = SensorialManager.instance.getTarget();
            rb = GetComponent<Rigidbody>();
            //targets = SensorialManager.instance.getRats();
            objetivo = GameManager.instance.GetPlayer();
        }

        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        /// <returns></returns>
        public override Direccion GetDirection()
        {

            Direccion direccion = new Direccion();
            if (!this.enabled) return direccion;

            //GET MAIN DIRECTION
            direccion.lineal = objetivo.transform.position - transform.position;
            direccion.lineal.Normalize();

            direccion.orientation = Vector3.SignedAngle(Vector3.forward, new Vector3(direccion.lineal.x, 0.0f, direccion.lineal.z), Vector3.up);
            direccion.lineal += WallAvoidance();

            direccion.lineal += Separate();

            direccion.lineal *= agente.aceleracionMax;

            // Podríamos meter una rotación automática en la dirección del movimiento, si quisiéramos
            return direccion;
        }

        public Vector3 Separate()
        {
            Vector3 direccion = new Vector3();

            float minDistance = -1f;
            float strength = 100;

            // Para cada entidad
            foreach (GameObject rat in targets)
            {
                // Comprobar que t está cerca
                Vector3 dirOpossite = transform.position - rat.transform.position;

                float distance = dirOpossite.magnitude;

                // Si entra en el área
                if (distance < threshold)
                {
                    // Añadir aceleración
                    dirOpossite.Normalize();
                    direccion += dirOpossite;

                    // Cogemos la fuerza en base al target más cercano
                    if (distance < minDistance || minDistance == -1)
                    {
                        // Fuerza de repulsión
                        strength = Mathf.Min(decayCoefficient / (distance * distance), agente.aceleracionMax);
                        minDistance = distance;
                    }
                }
            }

            // Aplicamos la dirección y fuerza al vector
            direccion.Normalize();
            direccion *= strength;

            return direccion;
        }

        public Vector3 WallAvoidance()
        {
            Vector3 direccion = new Vector3();
            //FORWARD
            Vector3 directionRay = transform.forward;
            checkHitRayCast(ref direccion, directionRay, lookAhead);
            //RIGHT
            directionRay = transform.forward*2 + (transform.right);
            checkHitRayCast(ref direccion, directionRay, lookSide);
            //LEFT
            directionRay = transform.forward*2 -transform.right;
            checkHitRayCast(ref direccion, directionRay, lookSide);

            return direccion;
        }

        private void checkHitRayCast(ref Vector3 directionAcc, Vector3 directionRay, float distance)
        {
            Vector3 from = transform.position;
            from.y += 1.5f; 
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
                directionAcc += dir;
            }
            else
                Debug.DrawRay(from, directionRay * distance, Color.green);
        }

        public Direccion AvoidCollision(ref Direccion direccion)
        {
            //AVOID COLLISION
            float shortestTime = Mathf.Infinity;

            GameObject firstTarget = null;
            float firstMinSeparation = 0;
            float firstDistance = 0;
            Vector3 firstRelativePos = Vector3.zero;
            Vector3 firstRelativeVel = Vector3.zero;
            Vector3 relativePos;


            foreach (GameObject target in targets)
            {
                if (target == gameObject) continue;

                Rigidbody targetRb = target.GetComponent<Rigidbody>();

                relativePos = target.transform.position - this.gameObject.transform.position;
                Vector3 relativeVel = targetRb.velocity - rb.velocity;

                float relativeSpeed = relativeVel.magnitude;

                float timeToCollision = Vector3.Dot(relativePos, relativeVel) / (relativeSpeed * relativeSpeed);

                float distance = relativePos.magnitude;

                float minSeparation = distance - relativeSpeed * timeToCollision;
                if (minSeparation > radiusFactorSeparation * radius)
                    continue;

                if (timeToCollision > 0 && timeToCollision < shortestTime)
                {
                    shortestTime = timeToCollision;
                    firstTarget = target;
                    firstMinSeparation = minSeparation;
                    firstDistance = distance;
                    firstRelativePos = relativePos;
                    firstRelativeVel = relativeVel;
                }
            }

            if (firstTarget == null)
            {
                direccion.lineal *= agente.aceleracionMax;
                //    return direccion;
                //}

                if (firstMinSeparation <= 0 || firstDistance < radiusFactorSeparation * radius)
                    relativePos = gameObject.transform.position - firstTarget.transform.position;
                else relativePos = firstRelativePos + firstRelativeVel * shortestTime;

                //relativePos.Normalize();

                direccion.lineal = relativePos;
            }
            return direccion;
        }
    }
}
