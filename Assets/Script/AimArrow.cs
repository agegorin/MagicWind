using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimArrow : MonoBehaviour
{
    private float timeChange;

    // Start is called before the first frame update
    void Start()
    {
        timeChange = Random.Range(-0.5f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.rotation * new Vector3(Mathf.Sin((Time.time + timeChange) * 10f) * 0.1f, 0, 0);
    }
}
