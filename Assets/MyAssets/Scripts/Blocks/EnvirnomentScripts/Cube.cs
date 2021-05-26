using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CubeColor
{
    blue,
    red
}

[RequireComponent(typeof(Rigidbody2D))]
public class Cube : MonoBehaviour
{

    //Serialized fields
    [SerializeField]
    private CubeColor _cubeColor;

    [SerializeField]
    private Vector2 _dropForceForRightDirection;

    [SerializeField]
    private Vector2 _dropForceForLeftDirection;

    [SerializeField]
    private float _timeBeforeDespawn = 2.0f;

    [SerializeField]
    private int _cubeInArmsLayer = 10;

    //Properties
    public CubeColor CubeColor { get { return _cubeColor; } }

    //Private fields
    private Rigidbody2D _rigidBody;

    //UnityCallbacks
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if(_rigidBody == null)
        {
            Debug.LogError("Rigidbody2D on " + gameObject.name + " not found!");
        }
    }

    //Public methods
    public void DropCube()
    {
        if (_rigidBody != null)
        {
            _rigidBody.isKinematic = false;

            if (_cubeColor == CubeColor.blue)
            {
                _rigidBody.AddForce(_dropForceForRightDirection);

            }
            else if (_cubeColor == CubeColor.red)
            {
                _rigidBody.AddForce(_dropForceForLeftDirection);

            }
        }
        Invoke("DisableCube", _timeBeforeDespawn);
    }

    public void PickUpCube()
    {
        gameObject.layer = _cubeInArmsLayer;

        if(_rigidBody != null)
        {
            _rigidBody.isKinematic = true;
        }
    }

    //Private methods
    private void DisableCube()
    {
        transform.SetParent(PoolManager.Instance.transform);
        gameObject.SetActive(false);
    }

}
