using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenParticlesPlayed : MonoBehaviour
{
    [SerializeField]
    List<ParticleSystem> m_particles = new List<ParticleSystem>();

    void Update()
    {
        int aliveParticleSystems = 0;
        foreach(var particle in m_particles)
		{
            if(particle.IsAlive())
			{
                ++aliveParticleSystems;
            }
		}

        if(aliveParticleSystems != m_particles.Count)
		{
            Destroy(gameObject);
		}
    }
}
