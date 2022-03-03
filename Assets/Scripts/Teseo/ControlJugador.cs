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
        
       // AudioSource audio;

        public virtual void Start()
        {
            //SensorialManager.instance.setTarget(this.gameObject);   
            //SensorialManager.instance.getRats().Add(this.gameObject);   

        }
        public override void Awake(){
            base.Awake();
            //audio = GetComponent<AudioSource>();
        }
        public override void Update()
        {
            base.Update();

            if (Input.GetKey(KeyCode.Space))
            {
                //if (!audio.isPlaying)audio.Play();
                //SensorialManager.instance.PlayFlauta(true);
                this.gameObject.GetComponentInChildren<Animator>().enabled = true;
            }
            else { 
                //audio.Pause();
                //SensorialManager.instance.PlayFlauta(false);
                this.gameObject.GetComponentInChildren<Animator>().enabled = false;
            }
        }
        /// <summary>
        /// Obtiene la dirección
        /// </summary>
        /// <returns></returns>
        public override Direccion GetDirection()
        {

            Direccion direccion = new Direccion();
            direccion.lineal.x = Input.GetAxis("Horizontal");
            direccion.lineal.z = Input.GetAxis("Vertical");
            direccion.lineal.Normalize();
            direccion.lineal *= agente.aceleracionMax;

            direccion.orientation = 90.0f;
            // Podrú}mos meter una rotación automática en la dirección del movimiento, si quisiéramos

            return direccion;
        }
    }
}