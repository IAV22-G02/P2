using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public Text textSize;

    public int sizeMaxLimit = 5;
    public int sizeMinLimit = 5;
    int size = 0;
    GameManager gM;
    // Start is called before the first frame update
    void Start(){
        size = (sizeMinLimit - 1) / 3;

        textSize.text = string.Format("{0} x {0}", (size* 3 +1) );
        gM = GameManager.instance;
        if (gM == null)
            Debug.LogError("There is not GameManager");
    }

    // Update is called once per frame
    void Update(){
        
    }

    public void IncreseSize(){
        if((size * 3 + 1) < sizeMaxLimit){
            size++;
            textSize.text = string.Format("{0} x {0}", (size * 3 + 1));
        }
    }
    
    public void DecreseSize(){
        if( (size * 3 + 1) > sizeMinLimit){
            size--;
            textSize.text = string.Format("{0} x {0}", (size * 3 + 1));
        }
    }

    public void Play(){
        GenerateMap();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void GenerateMap(){
        gM.createMap(size, size);
    }

    public void changeHeuristic()
    {
        gM.ChangeHeuristica();
    }
}