using UnityEngine;

public class Planet : MonoBehaviour
{
    private static Planet _selected;
    
    private Outline _outline;
    private Rigidbody _rigidbody; 
    
    /// <summary>
    /// Mass of the planet in tonnes
    /// </summary>
    public float Mass { get; private set; } = 500000;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _outline = GetComponent<Outline>();
        SetMass(Mass);
    }

    /// <summary>
    /// Toggles the selection of the planet
    /// </summary>
    public void SetSelected()
    {
        // We are on the selected object, so we can skip it
        if (_selected == this)
        {
            return;
        }

        // The current object is null or something else, so we set this as the selected one
        if (_selected != null)
        {
            _selected.Deselect();
        }

        this.Select();
    }

    /// <summary>
    /// Sets the mass of the planet
    /// </summary>
    /// <param name="mass">The mass of the planet in tons</param>
    public void SetMass(float mass)
    {
        this.Mass = mass;
        this.transform.localScale = (((Mass - UIManager.Instance._minimumMass) / (UIManager.Instance._maximumMass - UIManager.Instance._minimumMass)) * new Vector3(3.5f, 3.5f, 3.5f)) + new Vector3(1.1f, 1.1f, 1.1f);
        this._rigidbody.mass = mass;
    }
    
    private void Select()
    {
        _selected = this;
        _outline.OutlineWidth = 10f;
        UIManager.Instance.OnPlanetSelected(this);
    }

    public void SetPosition(Vector3 position)
    {
        _rigidbody.position = position;
    }

    /// <summary>
    /// Moves the planet in the given direction and a magnitude with respect to it's mass 
    /// </summary>
    /// <param name="direction">The direction and magnitude of the force</param>
    public void Move(Vector3 direction)
    {
        _rigidbody.AddForce(direction);
    }

    public void Enable()
    {
        this._rigidbody.isKinematic = false;
    }

    public void Disable()
    {
        this._rigidbody.isKinematic = true;
    }

    public void Deselect()
    {
        _selected = null;
        _outline.OutlineWidth = 0f;
        UIManager.Instance.DeselectPlanet();
    }

}
