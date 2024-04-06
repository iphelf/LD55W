using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Button btn = this.GetComponent<Button>();
        btn.onClick.AddListener(QuitGame);
    }
    void QuitGame()
    { 
        Debug.Log("退出游戏");
        Application.Quit();
    }
    
}






