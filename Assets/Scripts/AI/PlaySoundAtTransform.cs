using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundAtTransform : MonoBehaviour
{
    public LayerMask enemydetection;
    public float range = 30f;
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Played sound at " + gameObject);
            DetectableSound.PlayDetectableSound(transform.position, range, enemydetection);
        }
    }
}
