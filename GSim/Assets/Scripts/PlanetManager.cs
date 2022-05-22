using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] private Planet _plantPrefab;
    public static PlanetManager Instance { get; private set; }

    [Min(2)] [SerializeField] private int _numberPlanets = 2;

    [SerializeField] private Color[] _planetColors;

    private Planet _selected = null;
    private Camera _camera;
    
    private bool _isPlaying = false;

    private readonly List<Planet> _instantiatedPlanets = new List<Planet>();

    private readonly RaycastHit[] _raycastHits = new RaycastHit[10];
    private static Random _random;
    
    // Start is called before the first frame update
    private void Start()
    {
        _camera = Camera.main;
    }

    private Planet SpawnPlanet()
    {
        Planet planet = Instantiate(_plantPrefab);
        planet.GetComponent<Renderer>().material.color = _planetColors[_random.Next(0, _planetColors.Length)];
        _instantiatedPlanets.Add(planet);
        return planet;
    }

    void Awake()
    {
        if (Instance)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;
        _random = new Random();

        for (int i = 0; i < _numberPlanets; i++)
        {
            var planet = SpawnPlanet();
            planet.transform.position = new Vector3(-10 + i * 20, 0, 0);
            planet.gameObject.name = i.ToString();
        }
    }

    private float SetFromBounds(float value, float range)
    {
        if (Mathf.Abs(value) > range)
        {
            value = range * Mathf.Sign(value);
        }

        return value;
    }

    // Update is called once per frame
    private void Update()
    {
        if (_isPlaying || !Input.GetMouseButton(0) || UIUtils.IsPointerOverUIElement()) return;

        var size = Physics.RaycastNonAlloc(GetMouseRay(), _raycastHits);
        SelectPlanet(size);
        MovePlanet(size);
    }

    public void OnPlanetAdd()
    {
        var planet = SpawnPlanet();
        planet.transform.position = Vector3.zero;
        planet.gameObject.name = _instantiatedPlanets.Count.ToString();
        
        UIManager.Instance.SetPlanetCount(_instantiatedPlanets.Count);
    }

    public void OnPlanetRemoved()
    {
        if (_selected)
        {
            _instantiatedPlanets.Remove(_selected);
            Destroy(_selected.gameObject);
        }
        else
        {
            Planet planet = _instantiatedPlanets[_instantiatedPlanets.Count - 1];
            _instantiatedPlanets.Remove(planet);
            Destroy(planet.gameObject);
        }
        UIManager.Instance.SetPlanetCount(_instantiatedPlanets.Count);
    }

    private void FixedUpdate()
    {
        if (!_isPlaying) return;

        foreach (var planet in _instantiatedPlanets)
        {
            Vector3 position = planet.transform.position;
            Vector3 force = Vector3.zero;
            
            foreach (var other in _instantiatedPlanets)
            {
                Vector3 otherPosition = other.transform.position;
                float distance = Vector3.Distance(position, otherPosition);

                if (distance <= 1.51)
                {
                    continue;
                }
                
                Vector3 direction = (otherPosition - position).normalized;

                force += (float)(Constants.Gravity * planet.Mass * other.Mass / Mathf.Pow(
                    distance, 2)) * direction;
            }
            
            planet.Move(force);
        }
    }

    private void SelectPlanet(int size)
    {
        for (int i = 0; i < size; i++)
        {
            var hit = _raycastHits[i];
            if (!hit.transform.CompareTag("Planet")) continue;

            _selected = hit.transform.GetComponent<Planet>();
            _selected.SetSelected();
        }
    }

    private void MovePlanet(int size)
    {
        if (!_selected)
        {
            return;
        }

        for (int i = 0; i < size; i++)
        {
            var hit = _raycastHits[i];
            if (!hit.transform.CompareTag("InteractionPlane")) continue;

            var point = hit.point;

            point.x = SetFromBounds(point.x, 24);
            point.y = SetFromBounds(point.y, 12);

            _selected.SetPosition(point);
            // _selected.transform.position = selected;
        }
    }

    public void Play()
    {
        _isPlaying = !_isPlaying;
        if (_isPlaying)
        {
            UIManager.Instance.DeselectPlanet();
            foreach (var planet in _instantiatedPlanets)
            {
                planet.Deselect();
                planet.Enable();
            }
        }
        else
        {
            foreach (var planet in _instantiatedPlanets)
            {
                planet.Disable();
            }
        }
    }

    public void ResetLevel()
    {
        SceneManager.LoadScene(0);
    }

    private Ray GetMouseRay()
    {
        return _camera.ScreenPointToRay(Input.mousePosition);
    }
}