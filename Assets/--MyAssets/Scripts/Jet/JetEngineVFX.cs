using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetEngineVFX : MonoBehaviour
{
    ParticleSystem engineVFX;

    private void OnEnable()
    {
        engineVFX = GetComponent<ParticleSystem>();

        JetController.JetEngineVfx += OnOffEngineVFX;
        JetController.EngineVFXStartTime += SetVFXStartLifeTime;
    }
    private void OnDisable()
    {
        JetController.JetEngineVfx -= OnOffEngineVFX;
        JetController.EngineVFXStartTime -= SetVFXStartLifeTime;
    }

    void OnOffEngineVFX(bool play, bool vfxChildControl)
    {
        if (play == true)
        {
            engineVFX.Play(vfxChildControl);
        }
        else
        {
            engineVFX.Stop(vfxChildControl);
        }
    }

    void SetVFXStartLifeTime(float lifeTime)
    {
        engineVFX.startLifetime = lifeTime;
    }
}
