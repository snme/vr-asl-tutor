using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumWrong : MonoBehaviour
{
    Text num;
    // Start is called before the first frame update
    void Start()
    {
        num = GetComponent<Text>();
    }

    // Update is called once per frame
    public void update(string newNum)
    {
        num.text = newNum;
    }
}
