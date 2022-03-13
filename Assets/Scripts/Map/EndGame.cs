using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UCM.IAV.Movimiento {
    public class EndGame : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponent<ControlJugador>())
            {
                GameManager.instance.win();
            }
    }
    }

}

