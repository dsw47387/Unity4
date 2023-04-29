using System;
using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;
using Random = UnityEngine.Random;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private float projectileLifeTime = 5f;
    [SerializeField] private float baseFiringRate = 0.2f;
    [SerializeField] private float firingRateVariance = 0;
    [SerializeField] private float minimumFiringRate = 0.1f;
    [SerializeField] private bool useAI;
    [SerializeField] [Range(1, 3)] private int gunAmount = 1;
    [SerializeField] private GameObject gunMain;
    [SerializeField] private GameObject gunLeft;
    [SerializeField] private GameObject gunRight;

    [HideInInspector]
    public bool isFiring;

    private Coroutine firingCor;
    private Vector2 moveDirection;
    private readonly GameObject[][] gunConfigurations = new GameObject[3][];

    private void Start()
    {
        LoadGunObjects();
        
        if (useAI)
        {
            isFiring = true;
            moveDirection = transform.up * -1;
        }
        else
        {
            moveDirection = transform.up;
        }
    }

    private void LoadGunObjects()
    {
        if (gunMain)
        {
            gunConfigurations[0] = new [] { gunMain };
        }
        if (gunLeft && gunRight)
        {
            gunConfigurations[1] = new [] { gunMain };
        }
        if (gunLeft && gunMain && gunRight)
        {
            gunConfigurations[2] = new [] { gunLeft, gunMain, gunRight };
        }
    }

    private void Update()
    {
        Fire();
    }

    void Fire()
    {
        if (isFiring && firingCor == null)
        {
            firingCor = StartCoroutine(FireContinuously());
        }
        else if(!isFiring && firingCor != null)
        {
            StopCoroutine(firingCor);
            firingCor = null;
        }
    }

    IEnumerator FireContinuously()
    {
        while (true)
        {
            foreach (var gun in gunConfigurations[gunAmount-1]) 
            {
                GameObject projectile = Instantiate(projectilePrefab, gun.transform.position, Quaternion.identity);

                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.velocity = moveDirection * projectileSpeed;
                }
            
                Destroy(projectile, projectileLifeTime);
            }
            float timeToNextProjectile =
                Random.Range(baseFiringRate - firingRateVariance, baseFiringRate + firingRateVariance);

            timeToNextProjectile = Mathf.Clamp(timeToNextProjectile, minimumFiringRate, float.MaxValue);
            yield return new WaitForSeconds(timeToNextProjectile);
        }
    }
}
