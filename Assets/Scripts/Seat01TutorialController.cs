using System.Collections;
using UnityEngine;

public class Seat01TutorialController : MonoBehaviour
{
    [Header("References")]
    public Transform playerRoot;
    public Transform seat01Point;
    public BottomTutorialController tutorialController;
    public GameObject millikanObject;

    [Header("Seat Detection")]
    public float controlRadius = 0.9f;

    [Tooltip("Wenn aktiv, ignoriert die Sitzplatz-Distanz die Höhe (Y-Achse). " +
             "Empfohlen, damit das Verstellen der Höhe mit dem linken Stick die Story nicht erneut auslöst.")]
    public bool ignoreHeightForSeatCheck = true;

    [Header("Millikan Visibility")]
    public bool hideMillikanAtStart = true;
    public bool keepMillikanVisibleAfterAppearing = true;

    private bool wasInsideSeat01 = false;
    private bool millikanHasAppeared = false;
    private bool tutorialStartPending = false;
    private bool tutorialHasStartedOnce = false;   // One-Shot-Latch: Tutorial startet nur EINMAL

    private void Start()
    {
        if (millikanObject != null && hideMillikanAtStart)
            millikanObject.SetActive(false);
    }

    private void Update()
    {
        if (playerRoot == null || seat01Point == null || tutorialController == null)
            return;

        float distance = GetSeatDistance();
        bool isInsideSeat01 = distance <= controlRadius;

        if (isInsideSeat01 && !wasInsideSeat01)
        {
            wasInsideSeat01 = true;
            ShowMillikan();
            StartTutorialAfterMillikanIsReady();
        }
        else if (!isInsideSeat01 && wasInsideSeat01)
        {
            wasInsideSeat01 = false;

            if (!keepMillikanVisibleAfterAppearing && millikanObject != null)
                millikanObject.SetActive(false);
        }
    }

    // Distanz zum Sitzplatz. Standardmäßig horizontal (XZ),
    // damit Höhenänderungen die Erkennung nicht beeinflussen.
    private float GetSeatDistance()
    {
        if (!ignoreHeightForSeatCheck)
            return Vector3.Distance(playerRoot.position, seat01Point.position);

        Vector3 a = playerRoot.position;
        Vector3 b = seat01Point.position;
        a.y = 0f;
        b.y = 0f;
        return Vector3.Distance(a, b);
    }

    private void ShowMillikan()
    {
        if (millikanObject == null)
            return;

        if (millikanHasAppeared && keepMillikanVisibleAfterAppearing)
            return;

        millikanObject.SetActive(true);
        millikanHasAppeared = true;
    }

    private void StartTutorialAfterMillikanIsReady()
    {
        // Tutorial nie mehr als einmal starten.
        if (tutorialHasStartedOnce || tutorialStartPending)
            return;

        StartCoroutine(StartTutorialNextFrame());
    }

    private IEnumerator StartTutorialNextFrame()
    {
        tutorialStartPending = true;

        yield return null;

        if (tutorialController != null)
            tutorialController.BeginTutorialSession();

        tutorialHasStartedOnce = true;   // Latch setzen: ab jetzt kein Neustart mehr
        tutorialStartPending = false;
    }
}