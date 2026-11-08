using UnityEngine;

public class Player_AnimationEvent_Controller : MonoBehaviour
{
    [SerializeField] AudioClip _landingAudioClip;
    [SerializeField] AudioClip[] _footstepAudioClips;

    [Space(8)]
    [Range(0, 1)] public float _footstepAudioVolume = 0.5f;

    void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (_footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, _footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(_footstepAudioClips[index], transform.TransformPoint(transform.position), _footstepAudioVolume);
            }
        }
    }

    void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(_landingAudioClip, transform.TransformPoint(transform.position), _footstepAudioVolume);
        }
    }
}
