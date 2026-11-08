using UnityEngine;
using UnityEngine.UI;

public class Torch : MonoBehaviour
{
    [SerializeField] MeshRenderer _torchMeshRenderer;
    [SerializeField] Light _light;

    [SerializeField] Button _button;
    [SerializeField] Sprite _onSprite, _offSprite;

    [SerializeField] internal Canvas _canvas;

    [SerializeField] Image _batteryPowerImage;

    [SerializeField] float _speed = 1 / 256f;

    bool _isOn = true;

    private void Awake()
    {
        _button.onClick.AddListener(ButtonClicked);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(ButtonClicked);
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
