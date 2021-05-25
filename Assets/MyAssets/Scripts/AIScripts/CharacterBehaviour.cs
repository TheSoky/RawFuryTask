using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    SearchingForBox,
    GoingTowardsBox,
    GoingToDropBox
}

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterBehaviour : MonoBehaviour
{
    //Serialized fields
    [Header("Movement")]

    [SerializeField]
    private float _characterSpeed = 2.0f;

    [SerializeField]
    private Transform _redDropoffPoint;

    [SerializeField]
    private Transform _blueDropoffPoint;

    [SerializeField]
    private float _goalDistanceTolerance = 0.002f;

    [Header("Cube Detection")]

    [SerializeField]
    private float _detectionRayLength = 14.0f;

    [SerializeField]
    private Transform _raycastOrigin;

    [SerializeField]
    private LayerMask _cubeOnGroundLayer;

    [Header("Cube Handling")]

    [SerializeField]
    private Transform _cubeHoldingPoint;

    [Header("Visuals")]
    
    [SerializeField]
    private Sprite _characterHandsDown;

    [SerializeField]
    private Sprite _characterHandsUp;

    [SerializeField]
    private SpriteRenderer _characterSprite;

    //Private fields
    private CharacterState _currentState = CharacterState.SearchingForBox;

    private Vector3 _currentDestination;

    private Cube _currentCube;

    private Rigidbody2D _rigidBody;

    private ContactFilter2D _contactFilter;

    //Unity callbacks
    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if(_rigidBody == null)
        {
            Debug.LogError("Missing RigidBody2D on character");
        }

        if(_characterSprite == null)
        {
            Debug.LogError("Missing SpriteRenderer on character");
        }

        _contactFilter = new ContactFilter2D();
        _contactFilter.useLayerMask = true;
        _contactFilter.layerMask = _cubeOnGroundLayer;
    }

    private void Update()
    {
        switch (_currentState)
        {
            case CharacterState.SearchingForBox:
                OnSearchingForBox();
                break;

            case CharacterState.GoingTowardsBox:
                if((transform.position.x < _currentDestination.x + _goalDistanceTolerance) && (transform.position.x > _currentDestination.x - _goalDistanceTolerance))
                {
                    PickUpBox();
                }
                break;

            case CharacterState.GoingToDropBox:
                if ((transform.position.x < _currentDestination.x + _goalDistanceTolerance) && (transform.position.x > _currentDestination.x - _goalDistanceTolerance))
                {
                    _currentCube.DropCube();
                    if(_characterSprite != null)
                    {
                        _characterSprite.sprite = _characterHandsDown;
                    }
                    _currentState = CharacterState.SearchingForBox;
                }
                break;

            default:
                if(_rigidBody != null)
                {
                    _rigidBody.velocity = Vector2.zero;
                }
                break;
        }
    }

    //Private methods
    private void OnSearchingForBox()
    {
        RaycastHit2D[] leftHitResult = new RaycastHit2D[20];
        RaycastHit2D[] rightHitResult = new RaycastHit2D[20];
        RaycastHit2D targetBox;

        int numberOfLeftBoxes = Physics2D.Raycast(_raycastOrigin.position, Vector2.left, _contactFilter, leftHitResult, _detectionRayLength);
        int numberOfRightBoxes = Physics2D.Raycast(_raycastOrigin.position, Vector2.right, _contactFilter, rightHitResult, _detectionRayLength);

        if (numberOfLeftBoxes > 0 || numberOfRightBoxes > 0)
        {
            float nearestBoxDistance;

            if (numberOfLeftBoxes > 0)
            {
                nearestBoxDistance = leftHitResult[0].distance;
                targetBox = leftHitResult[0];
            }
            else
            {
                nearestBoxDistance = rightHitResult[0].distance;
                targetBox = rightHitResult[0];
            }

            for (int i = 0; i < numberOfLeftBoxes; i++)
            {
                if (leftHitResult[i].distance < nearestBoxDistance)
                {
                    nearestBoxDistance = leftHitResult[i].distance;
                    targetBox = leftHitResult[i];
                }
            }

            for (int i = 0; i < numberOfRightBoxes; i++)
            {
                if (rightHitResult[i].distance < nearestBoxDistance)
                {
                    nearestBoxDistance = rightHitResult[i].distance;
                    targetBox = rightHitResult[i];
                }
            }

            _currentDestination = targetBox.point;
            _currentCube = targetBox.transform.GetComponent<Cube>();

            Vector2 characterVelocity = new Vector2((transform.position.x - _currentDestination.x), 0.0f);
            characterVelocity.Normalize();
            characterVelocity *= -_characterSpeed;
            _rigidBody.velocity = characterVelocity;
            _currentState = CharacterState.GoingTowardsBox;
        }
    }

    private void PickUpBox()
    {
        _rigidBody.velocity = Vector2.zero;
        _currentCube.PickUpCube();
        _currentCube.transform.SetParent(_cubeHoldingPoint);
        _currentCube.transform.localPosition = Vector3.zero;
        
        if (_characterSprite != null)
        {
            _characterSprite.sprite = _characterHandsUp;
        }
        
        switch (_currentCube.CubeColor)
        {
            case CubeColor.blue:
                _currentDestination = _blueDropoffPoint.position;
                break;

            case CubeColor.red:
                _currentDestination = _redDropoffPoint.position;
                break;

            default:
                _currentDestination = Vector3.zero;
                Debug.LogError("Unknown object picked up");
                break;
        }

        Vector2 characterVelocity = new Vector2((transform.position.x - _currentDestination.x), 0.0f);
        characterVelocity.Normalize();
        characterVelocity *= -_characterSpeed;
        _rigidBody.velocity = characterVelocity;

        _currentState = CharacterState.GoingToDropBox;

    }

}
