using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    TextMeshProUGUI text;
    void Awake()
    {
        text = gameObject.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position += new Vector3(0, 2.5f * Time.deltaTime, 0);
        text.color = new Color(text.color.r, text.color.g + 0.0005f, text.color.b + 0.0005f);
    }
}
