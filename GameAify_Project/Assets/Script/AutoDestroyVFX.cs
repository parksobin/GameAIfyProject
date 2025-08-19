using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    [Tooltip("���� �ָ� ���� �������� �ı�. ����θ� �ִϸ��̼�/��ƼŬ ���̸� �ڵ� ����")]
    [SerializeField] private float overrideLifetime = 0f;

    private void OnEnable()
    {
        float life = overrideLifetime;

        // 1) �켱 ���� ������ ������ �װ� ���
        if (life > 0f)
        {
            Destroy(gameObject, life);
            return;
        }

        // 2) ��ƼŬ�̸� ��ƼŬ ��� �ð� ���
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            // duration + �ִ� ����(��ŸƮ ������Ÿ��) ���� ���
            float psLife = main.duration;
            if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                psLife += main.startLifetime.constantMax;
            else if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                psLife += main.startLifetime.constant;

            Destroy(gameObject, psLife);
            return;
        }

        // 3) �ִϸ����͸� ���� ��Ʈ�ѷ��� ù ��° Ŭ�� ���� ���
        var anim = GetComponent<Animator>();
        if (anim != null && anim.runtimeAnimatorController != null)
        {
            var clips = anim.runtimeAnimatorController.animationClips;
            if (clips != null && clips.Length > 0)
            {
                Destroy(gameObject, clips[0].length);
                return;
            }
        }

        // 4) ������ ������(1�� �� �ı�)
        Destroy(gameObject, 1f);
    }
}
