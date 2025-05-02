using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class CutsceneSkipButton : MonoBehaviour
{
    public Button button;
    public float fadeOutDelay = 2f;           // Time to wait before fading out
    public float fadeOutSpeed = 0.5f;         // How slow to fade out
    public float fadeInSpeed = 5f;            // How fast to fade in

    private CanvasGroup canvasGroup;
    private Vector3 lastMousePosition;
    private float idleTime = 0f;
    private bool isFadingOut = false;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        lastMousePosition = Input.mousePosition;
    }

    void Update()
    {
        if (Input.mousePosition != lastMousePosition)
        {
            // Mouse moved
            idleTime = 0f;
            isFadingOut = false;
        }
        else
        {
            idleTime += Time.deltaTime;
        }

        lastMousePosition = Input.mousePosition;

        if (idleTime > fadeOutDelay)
        {
            // Fade out slowly
            isFadingOut = true;
        }

        if (isFadingOut)
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, fadeOutSpeed * Time.deltaTime);
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, fadeInSpeed * Time.deltaTime);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
