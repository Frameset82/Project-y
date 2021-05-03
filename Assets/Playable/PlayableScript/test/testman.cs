using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class testman : Weapon
{
    public Transform attackPoint;
    public float attackRange = 5f;
    public LayerMask enemyLayers;
    public float angle = 45f;
    public List<GameObject> enemies;
    public bool isCollision = false;
    Color _blue = new Color(0f, 0f, 1f, 0.2f);
    Color _red = new Color(1f, 0f, 0f, 0.2f);

    public override void OnActive()
    {
        throw new System.NotImplementedException();
    }

    public override void OnAttack()
    {
        throw new System.NotImplementedException();
    }

    void Attack()
    {
        isCollision = false;
        enemies = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, enemyLayers);
        foreach (Collider enemyCollider in hitColliders)
        {
            GameObject enemy = enemyCollider.gameObject;
            Vector3 relativePos = enemy.transform.position - transform.position;
            Vector3 forward = transform.forward;
            forward.y = 0.0f;
            relativePos.y = 0;
            if (Vector3.Dot(forward.normalized, relativePos.normalized) > Mathf.Cos(Mathf.Deg2Rad * angle / 2))
            {
                isCollision = true;
                enemies.Add(enemy);
            }
        }
    }
    void Update()
    {
        Attack();
    }

    private void OnDrawGizmos()
    {
        Handles.color = isCollision ? _red : _blue;
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, angle / 2, attackRange);
        Handles.DrawSolidArc(transform.position, Vector3.up, transform.forward, -angle / 2, attackRange);
        Gizmos.DrawWireSphere(transform.position, attackRange);
/*        Gizmos.DrawWireSphere(attackPoint.position, attackRange);*/
    }
}