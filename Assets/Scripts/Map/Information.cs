using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UCM.IAV.Navegacion;

public class Information : MonoBehaviour
{


    // Variables para el frameRate
    int contadoFrames = 0;
    float contadorDeTiempo = 0.0f;
    float ultimaVez = 0.0f;
    float tiempoDeRefresco = 0.5f;

    [SerializeField]
    Text fps;
    [SerializeField]
    Text size;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (contadorDeTiempo < tiempoDeRefresco)
        {
            contadorDeTiempo += Time.deltaTime;
            contadoFrames++;
        }
        else
        {
            ultimaVez = (float)contadoFrames / contadorDeTiempo;
            contadoFrames = 0;
            contadorDeTiempo = 0.0f;
        }

        // Escibimos el ratio de frame

        Graph graph = GameManager.instance.GetGraph(); 

        if (fps != null) fps.text = "Frames:" + (((int)(ultimaVez * 100 + .5) / 100.0)).ToString();

        if (size != null) size.text = "Size:" + (graph.GetSize() / 10).ToString() + "x"
                 + (graph.GetSize() / 10).ToString();
    }
}
