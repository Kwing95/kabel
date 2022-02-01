using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleBurst : MonoBehaviour
{

    public List<ParticleSystem> particles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Burst()
    {
        foreach(ParticleSystem ps in particles)
            ps.Play();
    }

}
