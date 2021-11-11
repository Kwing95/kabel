using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timekeeper : MonoBehaviour
{

    public static Timekeeper instance;
    private float secondsPlayed = 0;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        secondsPlayed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (ActionManager.GetState() == ActionManager.State.Moving)
            secondsPlayed += Time.deltaTime;
    }

    public float GetSecondsPlayed()
    {
        return Mathf.Round(100 * secondsPlayed) / 100.0f;
    }
}
