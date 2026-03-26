using UnityEngine;

public class ParticlePoolObject : Poolable
{
    private void OnParticleSystemStopped()
    {
        Despawn();
    }

}
