using UnityEngine;
using System.Collections.Generic;

public class CellScript : MonoBehaviour
{
    //Serialized fields
    [SerializeField]
    private GameObject _leftWall;

    [SerializeField]
    private GameObject _rightWall;

    [SerializeField]
    private GameObject _upWall;

    [SerializeField]
    private GameObject _downWall;

    //Private fields
    private SpriteRenderer _spriteRenderer;

    private bool _isGoalCell = false;

    private List<CellScript> _neighbourCells = new List<CellScript>();

    private bool _isVisitedByPathfinding = false;

    //Properties
    public GameObject LeftWall { get { return _leftWall; } }
    public GameObject RightWall { get { return _rightWall; } }
    public GameObject UpWall { get { return _upWall; } }
    public GameObject DownWall { get { return _downWall; } }
    public bool IsGoalCell { get { return _isGoalCell; } set { _isGoalCell = value; } }
    public List<CellScript> NeighbourCells { get { return _neighbourCells; } set { _neighbourCells = value; } }

    public bool IsVisitedByPathfinding { get { return _isVisitedByPathfinding; } set { _isVisitedByPathfinding = value; } }



    //UnityCallbacks
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(_spriteRenderer == null)
        {
            Debug.LogError("Cells are missing SpriteRenderer!");
        }
    }

    //Public methods
    public void SetCellColor(Color newColor)
    {
        if(_spriteRenderer != null)
        {
            _spriteRenderer.color = newColor;
        }
    }

}
