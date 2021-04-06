using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorpedoCannonFSM : MonoBehaviour
{
    public float shootDistance;
    public float shootTime;
    public GameObject projectile;
    public GameObject[] projectileList;
    public int poolCount;
    public Collider2D playerCollider;
    public VisionCone vc;
    private float timer;
    private GameObject entity;

    private void Start()
    {
        projectileList = new GameObject[poolCount];
        for (int i = 0; i < poolCount; i++)
        {
            projectileList[i] = Instantiate(projectile, transform.position, transform.rotation);
            projectileList[i].GetComponent<Torpedo>().SetPlayerColliderHashCode(playerCollider.GetHashCode());
            projectileList[i].SetActive(false);
        }
    }

    private void Update()
    {
        entity = vc.GetObjectInVisionCone(0f);

        timer -= Time.deltaTime;

        if (entity != null)
        {
            transform.up = (entity.transform.position - transform.position).normalized;

            if (timer < 0)
            {
                timer = shootTime;
                
                foreach (GameObject proj in projectileList)
                {
                    if (!proj.activeSelf)
                    {
                        proj.transform.position = transform.position;
                        proj.transform.rotation = transform.rotation;
                        proj.SetActive(true);
                        break;
                    }
                }
            }
            
        }
        else
        {
            transform.up = GetComponentInParent<Transform>().up;
        }
    }
    

}
