using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Letter : MonoBehaviour
{

    Text letter;
    // Start is called before the first frame update
    void Start()
    {
        letter = GetComponent<Text>();
    }

    // Update is called once per frame
    public void update(string newLetter)
    {
        //letter.text = newLetter;
        Debug.Log("hi i'm here");
    }
}
