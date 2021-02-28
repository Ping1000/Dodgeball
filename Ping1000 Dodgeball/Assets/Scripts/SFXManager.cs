using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum soundType {
    ballThrow,
    impact,
    button,
    action,
    move
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public List<AudioClip> throwSounds;
    public List<AudioClip> impactSounds;
    public List<AudioClip> buttonSounds;
    public List<AudioClip> actionSounds;
    public List<AudioClip> moveSounds;

    private void Awake() {
        instance = this;
    }

    public static void PlayNewSound(soundType sound) {
        AudioClip ac = null;
        switch (sound) {
            case soundType.ballThrow:
                ac = instance.throwSounds[Random.Range(0, instance.throwSounds.Count)];
                break;
            case soundType.impact:
                ac = instance.impactSounds[Random.Range(0, instance.impactSounds.Count)];
                break;
            case soundType.button:
                ac = instance.buttonSounds[Random.Range(0, instance.buttonSounds.Count)];
                break;
            case soundType.action:
                ac = instance.actionSounds[Random.Range(0, instance.actionSounds.Count)];
                break;
            case soundType.move:
                ac = instance.moveSounds[Random.Range(0, instance.moveSounds.Count)];
                break;
            default:
                break;
        }
        
        GameObject sfxPlayer = Instantiate(Resources.Load("SFX Player") as GameObject);
        AudioSource playerSrc = sfxPlayer.GetComponent<AudioSource>();

        playerSrc.clip = ac;
        playerSrc.Play();
        Destroy(playerSrc.gameObject, ac.length);
    }
}
