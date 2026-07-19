using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class Parallax : MonoBehaviour
{
    private float length;
    private float StartPos;
    private Transform cam;
    public float ParallaxEfx;

    void Start()
    {
        StartPos = transform.position.x;
        length = GetComponent<SpriteRenderer>().bounds.size.x;
        cam = Camera.main.transform;
    }


    void Update()
    {
        float RePos = transform.position.x * (1 - ParallaxEfx);
        float Distance = cam.transform.position.x * ParallaxEfx;
        transform.position = new Vector3(StartPos + Distance, transform.position.y, transform.position.z);


    }
}
