using UnityEngine;

public class EnterCarOnProximity : MonoBehaviour
{
    public MonoBehaviour hmdNavigation;
    public CharacterController characterController;
    public CarMovementController carController;

    private bool entered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (entered) return;
        if (!other.CompareTag("MainCamera")) return;

        entered = true;

        GetComponent<Collider>().enabled = false;

        if (hmdNavigation != null)
            hmdNavigation.enabled = false;

        if (characterController != null)
            characterController.enabled = false;

        carController.EnableDriving();

        Debug.Log("Player entered car and is now seated");
    }
}

