using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testhis : MonoBehaviour
{
    public MazeGenerator TestCube;
    private int i = 0;
    //  ScriptableObject.CreateInstance(Maze);
    // Use this for initialization
    void Start()
    {

    }
    bool switcher = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (i == 0)
            {

                i++;
                TestCube.Generate(transform.position, transform.localScale);

            }
            else
            {
                TestCube.Delete();
                TestCube.Generate(transform.position, transform.localScale);
            }
        }
    }
}
