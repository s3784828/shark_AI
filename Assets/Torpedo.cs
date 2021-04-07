using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Torpedo : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameObject explosionParticles;
    public TrailRenderer tr;
    public float movementSpeed;
    public float existenceTime;

    private float timer;
    private GameObject particle;
    private bool destroyed;
    private int playerColliderHashCode;

    private void Start()
    {
        particle = Instantiate(explosionParticles, transform.position, Quaternion.identity);
        particle.GetComponent<ParticleSystem>();
        particle.GetComponent<ParticleSystem>().Stop();
        particle.SetActive(false);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timer -= Time.deltaTime;

        if (!destroyed && timer < 0)
        {
            StartCoroutine(TurnOff(0.25f));
            
        }

        rb.velocity = transform.up * movementSpeed;
    }

    private IEnumerator TurnOff(float time)
    {
        particle.SetActive(true);
        destroyed = true;
        transform.GetChild(0).gameObject.SetActive(false);
        tr.Clear();
        particle.transform.position = transform.position;
        particle.GetComponent<ParticleSystem>().Play();
        yield return new WaitForSeconds(time);
        gameObject.SetActive(false);
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Torpedo"))
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), collision.collider);

        else if (!collision.collider.CompareTag("Torpedo") && collision.collider.GetHashCode() != playerColliderHashCode && !destroyed)
            StartCoroutine(TurnOff(0.25f));
    }


    private void OnEnable()
    {
        tr.Clear();
        destroyed = false;
        transform.GetChild(0).gameObject.SetActive(true);
        timer = existenceTime;
    }

    public void SetPlayerColliderHashCode(int hashCode)
    {
        playerColliderHashCode = hashCode;
    }
}
