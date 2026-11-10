using ArbazAminKhan.Cameras.Components;
using System;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class Torch : MonoBehaviour
{
    [SerializeField] MeshRenderer _torchMeshRenderer;
    [SerializeField] Light _light;

    [SerializeField] internal Button _interactionButton;
    [SerializeField] Button _button;
    [SerializeField] Sprite _onSprite, _offSprite;

    [SerializeField] internal Canvas _canvas;

    [SerializeField] Image _batteryPowerImage;

    [SerializeField] float _speed = 1 / 256f;

    [SerializeField] AudioSource _click;

    bool _isOn = true;

    private void Awake()
    {
        _button.onClick.AddListener(ButtonClicked);
        _interactionButton.onClick.AddListener(InteractionButtonClicked);
    }

    private void InteractionButtonClicked()
    {
        GrabTorch();
    }

    public void GrabTorch()
    {
        Audio_Manager.Instance._click.Play();

        _canvas.gameObject.SetActive(true);
        _interactionButton.gameObject.SetActive(false);

        transform.parent = Camera_Manager.Instance._root;
        transform.SetLocalPositionAndRotation(new(.0625f, -.125f, .25f), default);

        transform.GetComponent<Collider>().enabled = false;
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(ButtonClicked);
        _interactionButton.onClick.RemoveListener(InteractionButtonClicked);
    }

    private void Update()
    {
        if (_isOn && _batteryPowerImage.fillAmount > 0) _batteryPowerImage.fillAmount -= _speed * Time.deltaTime;
        else LightBehaviour(false);
    }

    public void ButtonClicked()
    {
        SetIsOn(!_isOn);
    }

    public void SetIsOn(bool isOn)
    {
        _isOn = isOn;
        Audio_Manager.Instance._click.Play();

        _button.image.sprite = _isOn ? _onSprite : _offSprite;


        LightBehaviour(_isOn);
    }

    public void LightBehaviour(bool isOn)
    {
        isOn = isOn && _batteryPowerImage.fillAmount > 0;

        if (isOn) _torchMeshRenderer.material.EnableKeyword("_EMISSION");
        else _torchMeshRenderer.material.DisableKeyword("_EMISSION");

        _light.gameObject.SetActive(isOn);
    }
}
