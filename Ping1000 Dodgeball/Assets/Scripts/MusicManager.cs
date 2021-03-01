using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum teamName {
    red,
    blue,
    not_assigned
}

public class MusicManager : MonoBehaviour
{
    public AudioSource neutral_src;
    public AudioSource wb_src;
    public AudioSource lb_src;
    public AudioSource wm_src;
    public AudioSource lm_src;
    public AudioSource win_src;
    public AudioSource lose_src;
    [Range(0,1)]
    public float maxVolume;
    public float fadeTime;

    private AudioSource[] src_list;

    private PhaseController _pc;
    private teamName yourTeam;
    private int yourPlayers;
    private int enemyPlayers;

    // Start is called before the first frame update
    void Start()
    {
        src_list = new AudioSource[5];
        src_list[0] = neutral_src;
        src_list[1] = wb_src;
        src_list[2] = lb_src;
        src_list[3] = wm_src;
        src_list[4] = lm_src;

        yourTeam = teamName.not_assigned;
        yourPlayers = -1;
        enemyPlayers = -1;

        _pc = FindObjectOfType<PhaseController>();
        // start fading in neutral here? or somewhere else?
        StartCoroutine(FadeIn(neutral_src));

        // TESTING
        SetTeam(teamName.red);
    }

    public void SetTeam(teamName team) {
        yourTeam = team;
        switch (team) {
            case teamName.red:
                break;
            case teamName.blue:
                break;
            default:
                Debug.LogError("Tried to assign to invalid team!");
                return;
        }
        yourPlayers = 3;
        enemyPlayers = 3;

        foreach (AudioSource src in src_list) {
            src.volume = 0;
            src.loop = true;
            src.Play();
        }
    }

    public void UpdateSounds(int redPlayers, int bluePlayers) {
        int yourNewPlayers, enemyNewPlayers;
        switch (yourTeam) {
            case teamName.red:
                yourNewPlayers = redPlayers;
                enemyNewPlayers = bluePlayers;
                break;
            case teamName.blue:
                yourNewPlayers = bluePlayers;
                enemyNewPlayers = redPlayers;
                break;
            default:
                Debug.LogError("Tried to update sounds before assigning teams!");
                return;
        }

        // 3 IS HARDCODED
        if (yourPlayers == 3 && enemyPlayers == 3) {
            if (enemyNewPlayers < 3) {
                StartCoroutine(FadeIn(wb_src));
            } else if (yourNewPlayers < 3) {
                StartCoroutine(FadeIn(lb_src));
            }
        }
        if (enemyNewPlayers == 0) {
            DisableMusic();
            StartCoroutine(PlayingEndMusic(win_src));
        } else if (yourNewPlayers == 0) {
            DisableMusic();
            StartCoroutine(PlayingEndMusic(lose_src));
        } else {
            if (yourNewPlayers <= 1 && yourPlayers > 1) {
                StartCoroutine(FadeIn(lm_src));
            }
            if (enemyNewPlayers <= 1 && enemyPlayers > 1) {
                StartCoroutine(FadeIn(wm_src));
            }
        }
        yourPlayers = yourNewPlayers;
        enemyPlayers = enemyNewPlayers;
    }

    IEnumerator FadeIn(AudioSource src) {
        for (float progress = 0; progress < fadeTime; progress += Time.deltaTime) {
            src.volume = Mathf.Lerp(0, maxVolume, progress / fadeTime);
            yield return null;
        }
    }

    IEnumerator PlayingEndMusic(AudioSource src) {
        src.volume = 0;
        src.Play();
        for (float progress = 0; progress < fadeTime; progress += Time.deltaTime) {
            src.volume = Mathf.Lerp(0, maxVolume, progress / fadeTime);
            yield return null;
        }
    }

    public void DisableMusic() {
        foreach (AudioSource src in src_list) {
            StartCoroutine(FadeOut(src));
        }
    }

    IEnumerator FadeOut(AudioSource src) {
        for (float progress = 0; progress < fadeTime; progress += Time.deltaTime) {
            src.volume = Mathf.Lerp(maxVolume, 0, progress / fadeTime);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
