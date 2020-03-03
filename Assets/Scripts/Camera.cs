using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    public LayerMask layerMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(Vector3.zero);
        transform.Translate(Vector3.right * 2 * Time.deltaTime);

        if (Physics.Raycast(transform.position, Vector3.forward, Mathf.Infinity, layerMask))
        {
            Debug.Log("The ray hit the trees");
        }
    }
}
