using System;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public float lifeDuration = 3f;
    
    private BulletPool pool;
    public BulletPool Pool
    {
        get => pool;
        set => pool = value;
    }

    private void OnEnable()
    {
        Invoke(nameof(ReturnPool), lifeDuration);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * (speed * Time.deltaTime));
    }

    private void ReturnPool()
    {
        pool.Return(this);
    }
}