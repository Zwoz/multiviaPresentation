using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class SfxScript : MonoBehaviour
{

    public AudioSource audioSource1, audioSource2, audioSource3;
    public AudioClip dmg_taken, sword_swing1, sword_swing2, sword_swing3, place_block, LaserBeam, jump, land, step_grass1, step_grass2, step_stone1, step_stone2, death;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayDmgTaken()
    {
        audioSource1.clip = dmg_taken;
        audioSource1.Play();
    }

    public void PlaySwordSwing()
    {
        int sfxNum = Random.Range(1, 4);
        
        if(sfxNum == 1)
        {
            audioSource2.clip = sword_swing1;
            audioSource2.volume = 0.45f;
            audioSource2.Play();
        }

        if (sfxNum == 2)
        {
            audioSource2.clip = sword_swing2;
            audioSource2.volume = 0.45f;
            audioSource2.Play();
        }

        if (sfxNum == 3)
        {
            audioSource2.clip = sword_swing3;
            audioSource2.volume = 0.45f;
            audioSource2.Play();
        }
    }
    public void PlaySwordSwingLast()
    {
        int sfxNum = Random.Range(1, 4);

        if (sfxNum == 1)
        {
            audioSource2.clip = sword_swing1;
            audioSource2.volume = 0.2f;
            audioSource2.Play();

        }

        if (sfxNum == 2)
        {
            audioSource2.clip = sword_swing2;
            audioSource2.volume = 0.2f;
            audioSource2.Play();
        }

        if (sfxNum == 3)
        {
            audioSource2.clip = sword_swing3;
            audioSource2.volume = 0.2f;
            audioSource2.Play();
        }
    }

    public void PlayStepSound()
    {
        int sfxNum2 = Random.Range(1, 3);

        if (sfxNum2 == 1)
        {
            audioSource3.clip = step_grass1;
            audioSource3.volume = 0.3f;
            audioSource3.Play();
        }

        if (sfxNum2 == 2)
        {
            audioSource3.clip = step_grass2;
            audioSource3.volume = 0.3f;
            audioSource3.Play();
        }

        if (sfxNum2 == 3)
        {
            audioSource3.clip = step_stone1;
            audioSource3.volume = 0.3f;
            audioSource3.Play();
        }

        if (sfxNum2 == 4)
        {
            audioSource3.clip = step_stone2;
            audioSource3.volume = 0.3f;
            audioSource3.Play();
        }
    }

    public void PlaceBlockSound()
    {
        audioSource2.clip = place_block;
        audioSource2.volume = 0.65f;
        audioSource2.Play();
    }

    public void DashSound()
    {
        audioSource3.clip = sword_swing1;
        audioSource3.volume = 0.3f;
        audioSource3.Play();
    }

    public void DeathSound()
    {
        audioSource3.clip = death;
        audioSource3.volume = 0.5f;
        audioSource3.Play();
    }

    public void JumpSound()
    {
        audioSource3.clip = jump;
        audioSource3.volume = 0.3f;
        audioSource3.Play();
    }

    public void LandSound()
    {
        audioSource3.clip = step_grass1;
        audioSource3.volume = 0.25f;
        audioSource3.Play();
    }

}
