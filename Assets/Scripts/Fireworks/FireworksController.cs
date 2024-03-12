using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksController : MonoBehaviour
{
    public void Explode()
    {
        var explosion = GetComponent<ParticleSystem>();
        explosion.Play();
        Destroy(gameObject, explosion.duration);
    }
}
