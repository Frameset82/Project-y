using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*using UnityEditor;*/

public class MeleeWeapon : Weapon
{
    [Header("공격속성")]
    public float attackRange = 5f;
    public float angle = 45f;
    [Header("적 관련")]
    public LayerMask enemyLayers;
    public List<GameObject> enemies;
    public bool isCollision = false;
    

    Color _blue = new Color(0f, 0f, 1f, 0.2f);
    Color _red = new Color(1f, 0f, 0f, 0.2f);

    public override void OnAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void OnActive()
    {
        throw new System.NotImplementedException();
    }

    private void Start()
    {
        
    }

    void Update()
    {
        CollisionCheck();
    }

    public void CollisionCheck()
    {
        isCollision = false;
        enemies = new List<GameObject>();

        Collider[] hitColliders = Physics.OverlapSphere(PlayerKeyboardInput.player.transform.position, attackRange, enemyLayers);
        foreach (Collider enemyCollider in hitColliders)
        {
            GameObject enemy = enemyCollider.gameObject;
            Vector3 relativePos = enemy.transform.position - PlayerKeyboardInput.player.transform.position;
            Vector3 forward = PlayerKeyboardInput.player.transform.forward;
            forward.y = 0.0f;
            relativePos.y = 0;
            if (Vector3.Dot(forward.normalized, relativePos.normalized) > Mathf.Cos(Mathf.Deg2Rad * angle / 2))
            {
                isCollision = true;
                enemies.Add(enemy);
            }
        }
    }

    public override void TrChange()
    {
        throw new System.NotImplementedException();
    }

    /*
        private void OnDrawGizmos()
        {
            Handles.color = isCollision ? _red : _blue;
            Handles.DrawSolidArc(player.transform.position, Vector3.up, player.transform.forward, angle / 2, attackRange);
            Handles.DrawSolidArc(player.transform.position, Vector3.up, player.transform.forward, -angle / 2, attackRange);
            Gizmos.DrawWireSphere(player.transform.position, attackRange);
        }*/
}
