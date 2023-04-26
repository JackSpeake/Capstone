using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinishManager : MonoBehaviour
{
    public delegate Placement OnPlayerFinished();
    public static event OnPlayerFinished onPlayerFinished;
    private Placement finishPlace;
    private bool alreadyFinished = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Finish") && !alreadyFinished)
        {
            finishPlace = onPlayerFinished.Invoke();
            PlayerCanvasTimersManager playerCanvasTimersManager = GetComponent<PlayerCanvasTimersManager>();
            playerCanvasTimersManager.PlayerFinished(finishPlace);
            alreadyFinished = true;
        }
    }
}
