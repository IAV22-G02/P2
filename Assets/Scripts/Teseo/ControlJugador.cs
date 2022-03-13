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

        // AudioSource audio;
        [SerializeField]
        float rotationSpeed;
        GameManager gM;

        Animator animC;

        [SerializeField]
        GraphGrid graphGrid;

        [SerializeField]
        TesterGraph tstGph;

        [SerializeField]
        GameObject globalLight;
        bool activeLight = false;

        bool walking;
        bool nearMinotaur = false;

        public override void Start(){
            base.Start();
            animC = GetComponentInChildren<Animator>();
        }
        public override void Awake(){
            base.Awake();
            gM = GameManager.instance;
            gM.SetPlayer(this.gameObject);
        }
        public override void Update(){
            base.Update();

            if (Input.GetKeyDown(KeyCode.L)){
                Debug.Log("Light");
                activeLight = !activeLight;
                globalLight.SetActive(activeLight);
            }

            if (!Input.GetKey(KeyCode.Space))
                walking = true;
            else walking = false;

            if (Mathf.Abs(Input.GetAxis("Vertical")) > 0 || Mathf.Abs(Input.GetAxis("Horizontal")) > 0){
                animC.SetBool("running", true);
            }
            else{
                if(walking) animC.SetBool("running", false);
            }
        }

        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        /// <returns></returns>
        public override Direccion GetDirection(){
            Direccion direccion = new Direccion();
            if (walking) {
                float slow = (nearMinotaur) ? 0.3f : 1.0f;
                direccion.angular = rotationSpeed;
                direccion.lineal.x = Input.GetAxis("Vertical");
                direccion.lineal.z = Input.GetAxis("Horizontal") * -1;
                direccion.lineal.Normalize();
                direccion.lineal *= agente.aceleracionMax * slow;
            }
            return direccion;
        }

        public void isNearMinotaur(bool state)
        {
            nearMinotaur = state;
        }
    }
}