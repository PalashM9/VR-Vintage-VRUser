using UnityEngine;


public class HoverDebug : MonoBehaviour
{
    void Awake()
    {
        var grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        grab.hoverEntered.AddListener(_ => Debug.Log("HOVER ENTER"));
        grab.hoverExited.AddListener(_ => Debug.Log("HOVER EXIT"));
        grab.selectEntered.AddListener(_ => Debug.Log("SELECT ENTER"));
        grab.selectExited.AddListener(_ => Debug.Log("SELECT EXIT"));
    }
}
