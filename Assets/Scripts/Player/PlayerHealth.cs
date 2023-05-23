using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public HealthController healthController;
    PlayerCamera cam;

    void OnEnable()
    {
        healthController = GetComponent<HealthController>();
        rb = GetComponent<Rigidbody>();
        cam = FindObjectOfType<PlayerCamera>();

        healthController.onDeath += OnDeath;
    }
    void OnDisable()
    {
        healthController.onDeath -= OnDeath;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Backspace) || (healthController.health <= 0 && Input.GetMouseButtonDown(0)))
        {
            ResetLevel();
        }
    }

    void OnDeath()
    {
        StartCoroutine(cam.ShakeCamera(4f, .25f));
        Time.timeScale = 0;
    }

    public void ResetLevel()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*
    public IEnumerator Knockback(Vector3 pos, float knockbackForce, bool stopForce = true)
    {
        pos = (pos - rb.transform.position).normalized;

        rb.velocity = (!stopForce) ? rb.velocity : new Vector3(0f, 0f, 0f);

        yield return new WaitForSeconds(0.05f);
        rb.AddForce(new Vector3(-pos.x, pos.y, -pos.z) * knockbackForce, ForceMode.Impulse);
    }
    */
}
