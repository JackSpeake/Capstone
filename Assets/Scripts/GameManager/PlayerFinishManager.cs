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

    public static void ResetEvent()
    {
        onPlayerFinished = null;
    }

    private void Awake()
    {
        GameManager.onRaceReset += ResetFinish;     
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetFinish()
    {
        alreadyFinished = false;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Finish") && !alreadyFinished)
        {
            Debug.Log("Hit the finish object");
            finishPlace = onPlayerFinished.Invoke();
            PlayerCanvasTimersManager playerCanvasTimersManager = GetComponent<PlayerCanvasTimersManager>();
            playerCanvasTimersManager.PlayerFinished(finishPlace);
            Cursor.lockState = CursorLockMode.Confined;
            alreadyFinished = true;
        }
    }
}
