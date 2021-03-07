using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum soundType {
    ballThrow,
    impact,
    button,
    action,
    move,
    undo,
    cantClick
}

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;

    public List<AudioClip> throwSounds;
    public static float throwVolume = .75f;
    public List<AudioClip> impactSounds;
    public static float impactVolume = .5f;
    public List<AudioClip> buttonSounds;
    public static float buttonVolume = .5f;
    public List<AudioClip> actionSounds;
    public AudioClip undoSound;
    public AudioClip cantClickSound;
    public static float actionVolume = .5f;
    public List<AudioClip> moveSounds;
    public static float moveVolume = .25f;

    private void Awake() {
        instance = this;
    }

    public static void PlayNewSound(soundType sound) {
        AudioClip ac = null;
        GameObject sfxPlayer = Instantiate(Resources.Load("SFX Player") as GameObject);
        AudioSource playerSrc = sfxPlayer.GetComponent<AudioSource>();

        switch (sound) {
            case soundType.ballThrow:
                ac = instance.throwSounds[Random.Range(0, instance.throwSounds.Count)];
                playerSrc.volume = throwVolume;
                break;
            case soundType.impact:
                ac = instance.impactSounds[Random.Range(0, instance.impactSounds.Count)];
                playerSrc.volume = impactVolume;
                break;
            case soundType.button:
                ac = instance.buttonSounds[Random.Range(0, instance.buttonSounds.Count)];
                playerSrc.volume = buttonVolume;
                break;
            case soundType.action:
                ac = instance.actionSounds[Random.Range(0, instance.actionSounds.Count)];
                playerSrc.volume = actionVolume;
                break;
            case soundType.undo:
                ac = instance.undoSound;
                playerSrc.volume = actionVolume;
                break;
            case soundType.cantClick:
                ac = instance.cantClickSound;
                playerSrc.volume = actionVolume;
                break;
            case soundType.move:
                ac = instance.moveSounds[Random.Range(0, instance.moveSounds.Count)];
                playerSrc.volume = moveVolume;
                break;
            default:
                break;
        }

        playerSrc.clip = ac;

        playerSrc.Play();
        Destroy(playerSrc.gameObject, ac.length);
    }
}
