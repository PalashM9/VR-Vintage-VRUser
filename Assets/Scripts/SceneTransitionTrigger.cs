using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        Debug.Log("[Portal] Trigger entered by: " + other.name);

        if (other.CompareTag("MainCamera"))
        {
            triggered = true;
            Debug.Log("[Portal] Head detected. Loading scene...");
            SceneManager.LoadScene(sceneToLoad);
        }
    }
}
