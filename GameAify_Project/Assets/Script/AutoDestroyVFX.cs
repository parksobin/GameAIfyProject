using UnityEngine;

public class AutoDestroyVFX : MonoBehaviour
{
    [Tooltip("값을 주면 고정 수명으로 파괴. 비워두면 애니메이션/파티클 길이를 자동 측정")]
    [SerializeField] private float overrideLifetime = 0f;

    private void OnEnable()
    {
        float life = overrideLifetime;

        // 1) 우선 고정 수명이 있으면 그것 사용
        if (life > 0f)
        {
            Destroy(gameObject, life);
            return;
        }

        // 2) 파티클이면 파티클 재생 시간 사용
        var ps = GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            // duration + 최대 수명(스타트 라이프타임) 까지 고려
            float psLife = main.duration;
            if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                psLife += main.startLifetime.constantMax;
            else if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                psLife += main.startLifetime.constant;

            Destroy(gameObject, psLife);
            return;
        }

        // 3) 애니메이터면 현재 컨트롤러의 첫 번째 클립 길이 사용
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

        // 4) 마지막 안전망(1초 후 파괴)
        Destroy(gameObject, 1f);
    }
}
