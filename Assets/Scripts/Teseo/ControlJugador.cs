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

    /// <summary>
    /// Clara para el comportamiento de agente que consiste en ser el jugador
    /// </summary>
    public class ControlJugador: ComportamientoAgente
    {

        // AudioSource audio
        // ;
        [SerializeField]
        float rotationSpeed;

        Rigidbody rb;
        Animator animC;
        public virtual void Start(){
            rb = GetComponent<Rigidbody>();
            animC = GetComponentInChildren<Animator>();
        }
        public override void Awake(){
            base.Awake();
        }
        public override void Update(){
            base.Update();

            if(Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0)
                animC.SetBool("running", true);
            else
                animC.SetBool("running", false);
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
    }
}