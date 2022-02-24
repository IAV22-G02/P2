using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{


    public Text textSize;

    public int sizeLimit = 5;
    int size = 0;
    
    // Start is called before the first frame update
    void Start(){
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IncreseSize(){
        if(size < 5){
            size++;

            textSize.text = size.ToString();
        }
    }
    
    public void DecreseSize(){
        if(size > 1){
            size--;
            textSize.text = size.ToString();
        }
    }

    public void nextLevel(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    
}
