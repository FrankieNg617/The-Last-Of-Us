using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    public AudioSource movingsfx;
    public AudioSource jumpingsfx;

    public AudioClip walkingSound;
    public AudioClip runningSound;
    public AudioClip jumpingSound;

    private Player player;


    void Start()
    {
        player = GetComponent<Player>();
    }


    void Update()
    {
        if (player.IsGrounded() && (player.IsMoving().x != 0 || player.IsMoving().z != 0) && !player.IsSliding())
        {
            if (player.IsSprinting()) movingsfx.clip = runningSound;
            else movingsfx.clip = walkingSound;

            if (!movingsfx.isPlaying)
            movingsfx.Play();
        }
        else if (movingsfx.isPlaying)
        {
            movingsfx.Pause();
        }

        
        if (player.IsJumping())
        {
            jumpingsfx.Stop();
            jumpingsfx.clip = jumpingSound;
            jumpingsfx.Play();
        }
    }
}
