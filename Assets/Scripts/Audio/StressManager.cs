using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StressManager : MonoBehaviour
{
    public MusicManager musicManager;

    public int stressMeter = 0;

    private int stressTemp = 0;

    private bool isStressed = false;
    private bool once = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && stressMeter < 10)
        {
            stressMeter++;
            //Debug.Log("Stress Scale: " + stressMeter);
        }

        if (Input.GetKeyDown(KeyCode.N) && stressMeter > 0)
        {
            stressMeter--;
            //Debug.Log("Stress Scale: " + stressMeter);
        }

        if (stressMeter == 5 && once == false)
        {
            State5();
            Switch();
            stressTemp = stressMeter;
        }

        if (stressMeter != stressTemp && once == true)
        {
            State5();
            Switch();
        }
    }

    private void State5()
    {
        //musicManager.ToggleStressedState();
    }

    void Switch()
    {
        if (once == false)
        {
            once = true;
        }
        else
        {
            once = false;
        }
    }
}
