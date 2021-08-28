using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Система частиц, которая самоуничтожается через некоторое время
public class ParticlesScript : MonoBehaviour
{
    [SerializeField] private float systemLifetime;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyParticles());
    }

    IEnumerator DestroyParticles()
    {
        yield return new WaitForSeconds(systemLifetime);
        Destroy(gameObject);
    }
}
