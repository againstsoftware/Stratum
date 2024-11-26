using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TestLocGod : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        while (true)
        {
            GetComponent<TextMeshProUGUI>().text = LocalizationGod.GetLocalized("Tutorial", "eco_0_0_0");

            yield return new WaitForSeconds(1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
