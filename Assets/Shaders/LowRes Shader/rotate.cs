using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    private float _angle;

    // Start is called before the first frame update
    void Start()
    {
        _angle = Random.Range(-1.0f, 1.0f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(transform.up, _angle);
    }
}
