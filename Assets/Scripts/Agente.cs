/*    
   Copyright (C) 2020 Federico Peinado
   http://www.federicopeinado.com

   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Inform·tica de la Universidad Complutense de Madrid (EspaÒa).

   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento {

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine; 

/// <summary>
/// La clase Agente es responsable de modelar los agentes y gestionar todos los comportamientos asociados para combinarlos (si es posible) 
/// </summary>
    public class Agente : MonoBehaviour {
        /// <summary>
        /// Combinar por peso
        /// </summary>
        [Tooltip("Combinar por peso.")]
        public bool combinarPorPeso = false;
        /// <summary>
        /// Combinar por prioridad
        /// </summary>
        [Tooltip("Combinar por prioridad.")]
        public bool combinarPorPrioridad = false;
        /// <summary>
        /// Umbral de prioridad para tener el valor en cuenta
        /// </summary>
        [Tooltip("Umbral de prioridad.")]
        public float umbralPrioridad = 0.2f;

        /// <summary>
        /// Velocidad m·xima
        /// </summary>
        [Tooltip("Velocidad (lineal) m·xima.")]
        public float velocidadMax;
        /// <summary>
        /// RotaciÛn m·xima
        /// </summary>
        [Tooltip("RotaciÛn (velocidad angular) m·xima.")]
        public float rotacionMax;
        /// <summary>
        /// AceleraciÛn m·xima
        /// </summary>
        [Tooltip("AceleraciÛn (lineal) m·xima.")]
        public float aceleracionMax;
        /// <summary>
        /// AceleraciÛn angular m·xima
        /// </summary>
        [Tooltip("AceleraciÛn angular m·xima.")]
        public float aceleracionAngularMax;

        /// <summary>
        /// Velocidad (se puede dar una velocidad de inicio).
        /// </summary>
        [Tooltip("Velocidad.")]
        public Vector3 velocidad;
        /// <summary>
        /// RotaciÛn (o velocidad angular; se puede dar una rotaciÛn de inicio)
        /// </summary>
        [Tooltip("RotaciÛn (velocidad angular).")]
        public float rotacion;
        /// <summary>
        /// Orientacion (hacia donde encara el agente)
        /// </summary>
        [Tooltip("OrientaciÛn.")]
        public float orientacion;

        /// <summary>
        /// Valor de direcciÛn (instrucciones de movimiento)
        /// </summary>
        [Tooltip("DirecciÛn (instrucciones de movimiento).")]
        protected Direccion direccion;
        /// <summary>
        /// Grupos de direcciones, organizados seg˙n su prioridad
        /// </summary>
        [Tooltip("Grupos de direcciones.")]
        private Dictionary<int, List<Direccion>> grupos;

        /// <summary>
        /// Componente de cuerpo r˙Ñido (si la tiene el agente)
        /// </summary>
        [Tooltip("Cuerpo r˙Ñido.")]
        private Rigidbody cuerpoRigido;


        /// <summary>
        /// Al comienzo, se inicialian algunas variables
        /// </summary>
        public void Start()
        {
            // Descomentar estas l˙ãeas si queremos ignorar los valores iniciales de velocidad y rotaciÛn
            //velocidad = Vector3.zero; 
            //rotacion = 0.0f
            direccion = new Direccion();
            grupos = new Dictionary<int, List<Direccion>>();

            cuerpoRigido = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// En cada tick fijo, si hay movimiento din·mico, uso el simulador f˙êico aplicando las fuerzas que corresponda para moverlo.
        /// Un cuerpo r˙Ñido se puede mover con movePosition, cambiando la velocity o aplicando fuerzas, que es lo habitual y que permite respetar otras fuerzas que estÈn actuando sobre Èl a la vez.
        /// </summary>
        public virtual void FixedUpdate()
        {
            if (cuerpoRigido == null)
                return; // El movimiento serÅEcinem·tico, fotograma a fotograma con Update

            // Limitamos la aceleraciÛn al m·ximo que acepta este agente (aunque normalmente vendrÅEya limitada)
            if (direccion.lineal.sqrMagnitude > aceleracionMax)
                direccion.lineal = direccion.lineal.normalized * aceleracionMax;

            // La opciÛn por defecto ser˙} usar ForceMode.Force, pero eso implicar˙} que el comportamiento de direcciÛn tuviese en cuenta la masa a la hora de calcular la aceleraciÛn que se pide
            cuerpoRigido.AddForce(direccion.lineal, ForceMode.Acceleration);

            // Limitamos la aceleraciÛn angular al m·ximo que acepta este agente (aunque normalmente vendrÅEya limitada)
            //if (direccion.angular > aceleracionAngularMax)
            //    direccion.angular = aceleracionAngularMax;

            // Rotamos el objeto siempre sobre su eje Y (hacia arriba), asumiendo que el agente estÅEsobre un plano y quiere mirar a un lado o a otro
            // La opciÛn por defecto ser˙} usar ForceMode.Force, pero eso implicar˙} que el comportamiento de direcciÛn tuviese en cuenta la masa a la hora de calcular la aceleraciÛn que se pide

            Vector3 orientationVector = OriToVec(direccion.orientation);
            cuerpoRigido.rotation = Quaternion.LookRotation(orientationVector, Vector3.up);

            /* //El tema de la orientaciÛn, descomentarlo si queremos sobreescribir toda la cuestiÛn de la velocidad angular
            orientacion += rotacion / Time.deltaTime; // En lugar de * he puesto / para asÅEcalcular la aceleraciÛn, que es lo que debe ir aquÅE
            // Necesitamos "constreÒir" inteligentemente la orientaciÛn al rango (0, 360)
            if (orientacion < 0.0f)
                orientacion += 360.0f;
            else if (orientacion > 360.0f)
                orientacion -= 360.0f;

            Vector3 orientationVector = OriToVec(orientacion);
            cuerpoRigido.rotation = Quaternion.LookRotation(orientationVector, Vector3.up);
            */

            // Aunque tambiÈn se controlen los m·ximos en el LateUpdate, entiendo que conviene tambiÈn hacerlo aquÅE en FixedUpdate, que puede llegar a ejecutarse m·s veces

            // Limito la velocidad lineal al terminar 
            if (cuerpoRigido.velocity.magnitude > velocidadMax)
                cuerpoRigido.velocity = cuerpoRigido.velocity.normalized * velocidadMax;

            // Limito la velocidad angular al terminar
            if (cuerpoRigido.angularVelocity.magnitude > rotacionMax)
                cuerpoRigido.angularVelocity = cuerpoRigido.angularVelocity.normalized * rotacionMax;
            if (cuerpoRigido.angularVelocity.magnitude < -rotacionMax)
                cuerpoRigido.angularVelocity = cuerpoRigido.angularVelocity.normalized * -rotacionMax;
        }

        /// <summary>
        /// En cada tick, hace lo b·sico del movimiento cinem·tico del agente
        /// Un objeto que no atiende a f˙êicas se mueve a base de trasladar su transformada.
        /// Al no haber Freeze Rotation, ni rozamiento ni nada... seguramente vaya todo mucho m·s r·pido en cinem·tico que en din·mico
        /// </summary>
        public virtual void Update()
        {
            //KINEMATIC
            if (cuerpoRigido != null)
                return; // El movimiento serÅEdin·mico, controlado por la f˙êica y FixedUpdate

            DebugTest(2);
            // Limito la velocidad lineal antes de empezar
            if (velocidad.magnitude > velocidadMax)
                velocidad = velocidad.normalized * velocidadMax;

            // Limito la velocidad angular antes de empezar
            if (rotacion > rotacionMax)
                rotacion = rotacionMax;
            if (rotacion < -rotacionMax)
                rotacion = -rotacionMax;

            Vector3 desplazamiento = velocidad * Time.deltaTime;
            transform.Translate(desplazamiento, Space.World);

            orientacion += rotacion * Time.deltaTime;
            // Vamos a mantener la orientaciÛn siempre en el rango canÛnico de 0 a 360 grados
            if (orientacion < 0.0f)
                orientacion += 360.0f;
            else if (orientacion > 360.0f)
                orientacion -= 360.0f;

            // Elimino la rotaciÛn totalmente, dej·ndolo en el estado inicial, antes de rotar el objeto lo que nos marque la variable orientaciÛn
            transform.rotation = new Quaternion();
            transform.Rotate(Vector3.up, orientacion);
        }

        /// <summary>
        /// En cada parte tard˙} del tick, hace tareas de correcciÛn numÈrica 
        /// </summary>
        public virtual void LateUpdate()
        {
            if (combinarPorPrioridad)
            {
                direccion = GetPrioridadDireccion();
                grupos.Clear();
            }


            //KINEMATIC
            if (cuerpoRigido != null)
            {
                return; // El movimiento serÅEdin·mico, controlado por la f˙êica y FixedUpdate
            }
            DebugTest(1);

            // Limitamos la aceleraciÛn al m·ximo que acepta este agente (aunque normalmente vendrÅEya limitada)
            if (direccion.lineal.sqrMagnitude > aceleracionMax)
                direccion.lineal = direccion.lineal.normalized * aceleracionMax;

            // Limitamos la aceleraciÛn angular al m·ximo que acepta este agente (aunque normalmente vendrÅEya limitada)
            if (direccion.angular > aceleracionAngularMax)
                direccion.angular = aceleracionAngularMax;

            // AquÅEse calcula la prÛxima velocidad y rotaciÛn en funciÛn de las aceleraciones  
            velocidad += direccion.lineal * Time.deltaTime;
            rotacion += direccion.angular * Time.deltaTime;

            // Opcional: Esto es para actuar con contundencia si nos mandan parar (no es muy realista)
            if (direccion.angular == 0.0f)
                rotacion = 0.0f;
            if (direccion.lineal.sqrMagnitude == 0.0f)
                velocidad = Vector3.zero;

            /// En cada parte tard˙} del tick, encarar el agente (al menos para el avatar).... si es que queremos hacer este encaramiento
            transform.LookAt(transform.position + OriToVec(orientacion));

            // Se deja la direcciÛn vac˙} para el prÛximo fotograma
            direccion = new Direccion();
        }


        /// <summary>
        /// Establece la direcciÛn tal cual
        /// </summary>
        public void SetDirection(Direccion direccion)
        {
            this.direccion = direccion;
        }

        /// <summary>
        /// Establece la direcciÛn por peso
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="peso"></param>
        public void SetDirection(Direccion direccion, float peso)
        {
            this.direccion.lineal += (peso * direccion.lineal);
            this.direccion.angular += (peso * direccion.angular);
        }

        /// <summary>
        /// Establece la direcciÛn por prioridad
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="prioridad"></param>
        public void SetDirection(Direccion direccion, int prioridad)
        {
            if (!grupos.ContainsKey(prioridad))
            {
                grupos.Add(prioridad, new List<Direccion>());
            }
            grupos[prioridad].Add(direccion);
        }

        /// <summary>
        /// Devuelve el valor de direcciÛn calculado por prioridad
        /// </summary>
        /// <returns></returns>
        private Direccion GetPrioridadDireccion()
        {
            Direccion direccion = new Direccion();
            List<int> gIdList = new List<int>(grupos.Keys);
            gIdList.Sort();
            foreach (int gid in gIdList)
            {
                direccion = new Direccion();
                foreach (Direccion direccionIndividual in grupos[gid])
                {
                    // Dentro del grupo la mezcla es por peso
                    direccion.lineal += direccionIndividual.lineal;
                    direccion.angular += direccionIndividual.angular;
                }
                // SÛlo si el resultado supera un umbral, entonces nos quedamos con esta salida y dejamos de mirar grupos con menos prioridad
                if (direccion.lineal.magnitude > umbralPrioridad
                     || Mathf.Abs(direccion.angular) > umbralPrioridad)
                {
                    return direccion;
                }
            }
            return direccion;
        }
        /// <summary>
        /// Calcula el Vector3 dado un cierto angulo
        /// </summary>
        /// <param name="angulo"></param>
        /// <returns></returns>
        public Vector3 OriToVec(float angulo)
        {
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Sin(angulo * Mathf.Deg2Rad) * 1.0f; //  * 1.0f se aÒade para asegurar que el tipo es float
            vector.z = Mathf.Cos(angulo * Mathf.Deg2Rad) * 1.0f; //  * 1.0f se aÒade para asegurar que el tipo es float
            return vector.normalized;
        }

        private void DebugTest(int i){
            Debug.Log("Test:" + i);
        }
    }
}
