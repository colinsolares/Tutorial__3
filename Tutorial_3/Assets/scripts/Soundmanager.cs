using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soundmanager : MonoBehaviour
{
     public static AudioClip winSound;
    public static AudioClip loseSound;
    static AudioSource audioSrc;
    // Start is called before the first frame update
    void Start()
    {
        winSound = Resources.Load<AudioClip>("QuestComplete");
        loseSound = Resources.Load<AudioClip>("GameOver");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void PlaySound(string clip)
    {
        switch (clip)
        {
            case "Quest Complete":
                audioSrc.PlayOneShot(winSound);
                break;

            case "GameOver":
                audioSrc.PlayOneShot(loseSound);
                break;
        }
    }
}
