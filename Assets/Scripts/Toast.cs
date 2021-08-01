using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toast : MonoBehaviour
{

    public static Toast instance;
    public TextMeshProUGUI toastMesh;
    private static int toastNonce = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (instance)
            Destroy(gameObject);
        else
            instance = this;
        
        toastMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ToastInstanceWrapper(string message)
    {
        instance.StartCoroutine(instance.DisplayToast(message));
    }

    public static void ToastWrapper(string message)
    {
        instance.StartCoroutine(instance.DisplayToast(message));
    }

    public IEnumerator DisplayToast(string message, float duration = 3)
    {
        int currentToast = ++toastNonce;
        toastMesh.text = message;

        yield return new WaitForSeconds(duration);

        if (currentToast == toastNonce)
            toastMesh.text = "";
    }

}
