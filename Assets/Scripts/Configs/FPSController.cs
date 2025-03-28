using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FPSController : MonoBehaviour
{
    [SerializeField]
    private TMP_Text textFPS;

    private float deltaTime = 0.0f;

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        textFPS.text = Mathf.Ceil(fps).ToString() + " FPS";
    }
}
