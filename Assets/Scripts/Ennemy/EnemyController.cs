using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private float _detectionRange = 15f;
    private Transform _target;
    private EnemyMovement _movement;

    private void Start()
    {
        _movement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        findTarget();

        if (_target == null) return;
        var distance = Vector3.Distance(_target.position, transform.position);

        if (distance >= _detectionRange) return; // TODO check ca
        FaceTarget();

        if (distance < 3) return;
        MoveToTarget(distance);
    }

    private void MoveToTarget(float distance)
    {
        _movement.Move(distance);
    }

    private void findTarget()
    {
        var targets = Physics.OverlapSphere(transform.position, _detectionRange);
        foreach (var target in targets)
        {
            _target = target.transform;
        }
    }
    
    private void FaceTarget ()
    {
        var direction = (_target.position - transform.position).normalized;
        if (direction.x == 0 || direction.z == 0) return;
        var lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }

    public void TakeDamage()
    {
        print(gameObject.name + "Took damage");
    }
}
