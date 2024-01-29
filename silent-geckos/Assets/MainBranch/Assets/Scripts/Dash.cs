﻿using System.Collections;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] private AudioClip clip;

    [SerializeField] private GameObject player;
    [SerializeField] private bool isDashOnCooldown = false;
    [SerializeField] static public bool isMovementEnabled = true;
    [SerializeField] private float dashCooldown = 1f;

    [Header("Unlocks")] [SerializeField] private Unlock dash;
    [SerializeField] private Unlock dash1;
    [SerializeField] private Unlock dash2;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        if (dash1.isUnlocked) dashCooldown = .8f;
        if (dash2.isUnlocked) dashCooldown = .6f;
        if (Input.GetKeyDown("l") && !isDashOnCooldown && dash.isUnlocked)
        {
            StartCoroutine(DashCR());
        }
    }

    IEnumerator DashCR()
    {
        AudioController.instance.PlaySFX(clip);

        isDashOnCooldown = true;
        float temp = player.GetComponent<Rigidbody2D>().gravityScale;
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        isMovementEnabled = false;
        player.GetComponent<Rigidbody2D>().velocity = new Vector3(player.transform.localScale.x * 4, 0, 0);
        yield return new WaitForSeconds(.2f);
        player.GetComponent<Rigidbody2D>().velocity = new Vector3(0, 0, 0);
        player.GetComponent<Rigidbody2D>().gravityScale = temp;
        isMovementEnabled = true;
        yield return new WaitForSeconds(dashCooldown);
        isDashOnCooldown = false;
    }
}
