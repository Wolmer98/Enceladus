using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterManager : MonoBehaviour
{
    [SerializeField] float startDelay = 5;
    [SerializeField] float metersRaisedPerSecond = 0.01f;

    [SerializeField] CanvasGroup underWaterCanvas;

    [SerializeField] FMODUnity.StudioEventEmitter underWaterSource;

    Transform player;
    float playerHeight;
    bool waterInitialized;
    //public bool CatchBreath = false;

    float delayTimer = 0;

    bool playerInWater;
    bool playerBeneathWater;

    public bool PlayerInWater { get { return playerInWater; } }
    public bool PlayerBeneathWater { get { return playerBeneathWater; } }

    public FMODUnity.StudioEventEmitter UnderWaterSource { get { return underWaterSource; } }

    void Start()
    {
        FindObjectOfType<WorldGenerator>().OnWorldStart.AddListener(delegate { ResetWater(); });
    }

    private void ResetWater()
    {
        player = FindObjectOfType<PlayerController>().transform;

        GameObject[] floors = GameObject.FindGameObjectsWithTag("Floor");

        float minY = Mathf.Infinity;

        for (int i = 0; i < floors.Length; i++)
        {
            if (floors[i].transform.position.y < minY)
            {
                minY = floors[i].transform.position.y;
            }
        }

        transform.position = new Vector3(player.position.x, minY - 0.2f, player.position.z);

        delayTimer = 0;

        waterInitialized = true;
    }

    void Update()
    {
        if (waterInitialized && delayTimer >= startDelay)
        {
            transform.position += Vector3.up * metersRaisedPerSecond * Time.deltaTime;

            if (player != null && playerHeight == 0)
            {
                playerHeight = player.GetComponent<CharacterController>().height;
            }

            if ((player.position.y - playerHeight) <= transform.position.y - 0.35f)
            {
                playerInWater = true;
            }
            else
            {
                playerInWater = false;
            }

            if (player.position.y <= transform.position.y - 0.6f && playerBeneathWater == false)
            {
                if (underWaterCanvas != null)
                {
                    underWaterCanvas.alpha = 1;
                }

                playerBeneathWater = true;

                underWaterSource.Play();
                underWaterSource.EventInstance.setParameterByName("Underwater", 1);
            }
            else if(player.position.y > transform.position.y - 0.6f && playerBeneathWater == true)
            {
                if (underWaterCanvas != null)
                {
                    underWaterCanvas.alpha = 0;
                }

                playerBeneathWater = false;

                underWaterSource.EventInstance.setParameterByName("Underwater", 0);
                underWaterSource.EventInstance.release();
            }
        }
        else
        {
            delayTimer += Time.deltaTime;
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        underWaterSource.EventInstance.setParameterByName("Underwater", 0);
        underWaterSource.EventInstance.release();
    }
}
