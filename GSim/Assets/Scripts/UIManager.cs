using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private Planet _planet;

    [SerializeField] private Slider _slider;

    [SerializeField] private Button _addButton;
    [SerializeField] private Button _removeButton;

    [SerializeField] public float _minimumMass;
    [SerializeField] public float _maximumMass;

    [SerializeField] private int _maxPlanets = 6;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        _slider.onValueChanged.AddListener(OnValueChanged);
        _addButton.onClick.AddListener(PlanetManager.Instance.OnPlanetAdd);
        _removeButton.onClick.AddListener(PlanetManager.Instance.OnPlanetRemoved);
    }

    public void SetPlanetCount(int count)
    {
        print(_maxPlanets);
        _addButton.interactable = count < _maxPlanets;
        _removeButton.interactable = count > 2;
    }

    private void OnValueChanged(float value)
    {
        if (_planet)
        {
            _planet.SetMass(Mathf.Lerp(_minimumMass, _maximumMass, value));
        }
    }

    public void OnPlanetSelected(Planet planet)
    {
        _planet = planet;
        _slider.value = (planet.Mass - _minimumMass) / (_maximumMass - _minimumMass);
        _slider.interactable = true;
    }

    public void DeselectPlanet()
    {
        _planet = null;
        _slider.value = 0.5f;
        _slider.interactable = false;
    }
}