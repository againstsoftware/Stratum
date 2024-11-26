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
            GetComponent<TextMeshProUGUI>().text = LocalizationGod.GetLocalized("Cards", "deep_roots_desc");

            yield return new WaitForSeconds(1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
