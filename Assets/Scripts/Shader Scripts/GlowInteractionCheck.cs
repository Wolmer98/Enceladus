using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlowInteractionCheck : MonoBehaviour
{
    [SerializeField] LayerMask InteractionLayer;

    bool canCheck = true;
    float reach = 0;
    GlowObject glowObject;


    private void Start()
    {
        reach = FindObjectOfType<PlayerController>().Reach;
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(delegate { WorldLoaded(); });
    }

    void WorldLoaded()
    {
        canCheck = true;
    }

    void Update()
    {
        if (canCheck == true)
        {
            StartCoroutine(CheckForGlow());
        }
    }

    IEnumerator CheckForGlow()
    {
        canCheck = false;
        yield return new WaitForSeconds(0.1f);

        //TODO: RAYCAST FROM CENTER OF THE SCREEN FOR A GLOW COMPONENT.
        RaycastHit raycastHit;
        if (Physics.Raycast(transform.position, transform.forward, out raycastHit, reach, InteractionLayer))
        {
            //This will happen if camera moves between two glowable objects without a failed raycast in between.
            if (glowObject != null && raycastHit.transform.GetComponent<GlowObject>() != glowObject)
            {
                glowObject.SetTargetColor(Color.black);
            }

            glowObject = raycastHit.transform.GetComponent<GlowObject>();

            if (glowObject != null)
            {
                glowObject.SetTargetColor(glowObject.GlowColor);
            }

        }
        else
        {
            if (glowObject != null)
            {
                glowObject.SetTargetColor(Color.black);
            }
        }

        canCheck = true;
    }
}
