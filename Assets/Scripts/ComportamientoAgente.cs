/*    
   Copyright (C) 2020 Federico Peinado
   http://www.federicopeinado.com

   Este fichero forma parte del material de la asignatura Inteligencia Artificial para Videojuegos.
   Esta asignatura se imparte en la Facultad de Inform�tica de la Universidad Complutense de Madrid (Espa�a).

   Autor: Federico Peinado 
   Contacto: email@federicopeinado.com
*/
namespace UCM.IAV.Movimiento
{

    using UnityEngine;

    /// <summary>
    /// Clase abstracta que funciona como plantilla para todos los comportamientos de agente
    /// </summary>
    public class ComportamientoAgente : MonoBehaviour
    {
        /// <summary>
        /// Peso
        /// </summary>
        public float peso = 1.0f;
        /// <summary>
        /// Prioridad
        /// </summary>
        public int prioridad = 1;
        /// <summary>
        /// Objetivo (para aplicar o representar el comportamiento, depende del comportamiento que sea)
        /// </summary>
        public GameObject objetivo;
        /// <summary>
        /// Agente que hace uso del comportamiento
        /// </summary>
        protected Agente agente;

        //private Wander wanderCmp;

        /// <summary>
        /// Al despertar, establecer el agente que har�Euso del comportamiento
        /// </summary>
        public virtual void Awake()
        {
            agente = gameObject.GetComponent<Agente>();
            //wanderCmp = gameObject.GetComponent<Wander>();
        }

        public virtual void Start() {

        }
        /// <summary>
        /// En cada tick, establecer la direcci�n que corresponde al agente, con peso o prioridad si se est�n usando
        /// </summary>
        public virtual void Update(){
            asignDirection();
        }

        public virtual void LateUpdate(){
            asignDirection();
        }

        private void asignDirection(){
            if (agente == null) return;

            if (agente.combinarPorPeso)
                agente.SetDirection(GetDirection(), peso);
            else if (agente.combinarPorPrioridad)
                agente.SetDirection(GetDirection(), prioridad);
            else
                agente.SetDirection(GetDirection());
        }

        /// <summary>
        /// Devuelve la direccion calculada
        /// </summary>
        /// <returns></returns>
        public virtual Direccion GetDirection()
        {
            return new Direccion();
        }

        /// <summary>
        /// Asocia la rotaci�n al rango de 360 grados
        /// </summary>
        /// <param name="rotacion"></param>
        /// <returns></returns>
        public float RadianesAGrados(float rotacion)
        {
            rotacion %= 360.0f;
            if (Mathf.Abs(rotacion) > 180.0f)
            {
                if (rotacion < 0.0f)
                    rotacion += 360.0f;
                else
                    rotacion -= 360.0f;
            }
            return rotacion;
        }

        /// <summary>
        /// Cambia el valor real de la orientaci�n a un Vector3 
        /// </summary>
        /// <param name="orientacion"></param>
        /// <returns></returns>
        public Vector3 OriToVec(float orientacion)
        {
            Vector3 vector = Vector3.zero;
            vector.x = Mathf.Sin(orientacion * Mathf.Deg2Rad) * 1.0f;
            vector.z = Mathf.Cos(orientacion * Mathf.Deg2Rad) * 1.0f;
            return vector.normalized;
        }

        void OnCollisionStay(Collision collision)
        {
            //if(wanderCmp != null && collision.gameObject.GetComponent<BoxCollider>() != null){
            //    asignDirection();
            //}
        }

        public virtual void onCollisionEnter(Collision collision){

        }

        public virtual void onCollisionExit(Collision collision)
        {

        }
    }
}
