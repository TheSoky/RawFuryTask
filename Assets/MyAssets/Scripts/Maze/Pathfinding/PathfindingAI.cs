using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingAI : MonoBehaviour
{
    //Helper Class
    private class BranchData
    {
        public List<CellScript> CellData = new List<CellScript>();
        public BranchData ParentBranch;
        public List<BranchData> ChildrenBranches = new List<BranchData>();
        public bool IsEveryChildDeadEnd = false;
    }


    [SerializeField]
    private MazeGenerator _mazeGenerator;

    [Header("Maze solving color settings")]

    [SerializeField]
    private Color _currentPoints = Color.yellow;

    [SerializeField]
    private Color _activeBranches = Color.blue;

    [SerializeField]
    private Color _deadEndColor = Color.red;

    [SerializeField]
    private Color _solutionColor = Color.green;

    private List<BranchData> _currentBranches = new List<BranchData>();
    private bool _isGoalFound = false;
    private List<BranchData> _deadEndBranches = new List<BranchData>();

    private void Start()
    {
        CellScript startingCell = _mazeGenerator.CentreCells[0].cScript;
  
        BranchData startingBranch = new BranchData();
        startingBranch.CellData.Add(startingCell);
        _currentBranches.Add(startingBranch);
        startingCell.SetCellColor(_currentPoints);
        StartCoroutine(ExploreNextBranches());
    }

    private IEnumerator ExploreNextBranches()
    {

        while(!_isGoalFound)
        {
            List<BranchData> branchesToAdd = new List<BranchData>();

            foreach (BranchData currentBranch in _currentBranches)
            {
                List<CellScript> cellsToAdd = new List<CellScript>();

                foreach (CellScript cell in currentBranch.CellData)
                {
                    if(!cell.IsVisitedByPathfinding)
                    {
                        cell.IsVisitedByPathfinding = true;
                        cell.SetCellColor(_activeBranches);
                        //found goal
                        if (cell.IsGoalCell)
                        {
                            MarkBranchesAsGoal(_currentBranches);
                        }
                        //Dead end
                        else if (cell.NeighbourCells.Count == 1)
                        {
                            MarkBranchAsDeadEnd(currentBranch);
                        }
                        //proceed on branch
                        else if (cell.NeighbourCells.Count == 2)
                        {
                            //Handle starting case in which there are 2 unvisited cells in 2*2 starting grid
                            if(!cell.NeighbourCells[0].IsVisitedByPathfinding && !cell.NeighbourCells[1].IsVisitedByPathfinding)
                            {
                                cellsToAdd.Add(cell.NeighbourCells[0]);
                                cellsToAdd.Add(cell.NeighbourCells[1]);

                                cell.NeighbourCells[0].SetCellColor(_currentPoints);
                                cell.NeighbourCells[1].SetCellColor(_currentPoints);


                            }
                            //Handle other cases where it is proceeding through lines and corners without branching
                            else
                            {
                                bool branchIsDeadEnd = true;
                                foreach (CellScript neighbourCell in cell.NeighbourCells)
                                {
                                    if (!neighbourCell.IsVisitedByPathfinding)
                                    {
                                        branchIsDeadEnd = false;
                                        cellsToAdd.Add(neighbourCell);
                                        neighbourCell.SetCellColor(_currentPoints);

                                    }
                                }
                                if (branchIsDeadEnd)
                                {
                                    MarkBranchAsDeadEnd(currentBranch);
                                }
                            }


                        }
                        //create new branches
                        else if (cell.NeighbourCells.Count > 2)
                        {
                            bool branchIsDeadEnd = true;
                            foreach (CellScript neighbour in cell.NeighbourCells)
                            {
                                if (!neighbour.IsVisitedByPathfinding)
                                {
                                    branchIsDeadEnd = false;
                                    BranchData newBranch = new BranchData();
                                    newBranch.CellData.Add(neighbour);
                                    newBranch.ParentBranch = currentBranch;
                                    branchesToAdd.Add(newBranch);
                                    currentBranch.ChildrenBranches.Add(newBranch);
                                    neighbour.SetCellColor(_currentPoints);
                                }
                            }
                            if (branchIsDeadEnd)
                            {
                                MarkBranchAsDeadEnd(currentBranch);
                            }
                        }
                    }
                }
                
                foreach (CellScript cellToAdd in cellsToAdd)
                {
                    currentBranch.CellData.Add(cellToAdd);
                }
            }

            foreach (BranchData branchToAdd in branchesToAdd)
            {
                _currentBranches.Add(branchToAdd);
            }

            RemoveDeadEndBranches();
            yield return null;
        }
    }

    private void MarkBranchAsDeadEnd(BranchData deadBranch)
    {
        deadBranch.IsEveryChildDeadEnd = true;
        foreach (CellScript branchCell in deadBranch.CellData)
        {
            branchCell.SetCellColor(_deadEndColor);
        }
        _deadEndBranches.Add(deadBranch);

        if (deadBranch.ParentBranch != null)
        {
            bool isEverySiblingDeadEnd = true;
            foreach (BranchData siblingBranch in deadBranch.ParentBranch.ChildrenBranches)
            {
                if (!siblingBranch.IsEveryChildDeadEnd)
                {
                    isEverySiblingDeadEnd = false;
                    break;
                }

            }
            if(isEverySiblingDeadEnd)
            {
                MarkBranchAsDeadEnd(deadBranch.ParentBranch);
            }
        }
    }

    private void RemoveDeadEndBranches()
    {
        foreach (BranchData deadBranch in _deadEndBranches)
        {
            _currentBranches.Remove(deadBranch);
        }
        _deadEndBranches.Clear();
    }

    private void MarkBranchesAsGoal(List<BranchData> branchesToGoal)
    {
        _isGoalFound = true;
        Debug.Log("Goal found!");

        List<BranchData> goalBranch = new List<BranchData>();

        for (int i = branchesToGoal.Count - 1; i > 0; i--)
        {
            foreach (CellScript finalCell in branchesToGoal[i].CellData)
            {
                if(finalCell.IsGoalCell)
                {
                    goalBranch.Add(branchesToGoal[i]);
                    break;
                }
            }
            if(goalBranch.Count>0)
            { 
                break; 
            }
        }

        while(goalBranch[goalBranch.Count - 1].ParentBranch != null)
        {
            goalBranch.Add(goalBranch[goalBranch.Count - 1].ParentBranch);
        }

        foreach (BranchData branch in goalBranch)
        {
            foreach (CellScript goalCell in branch.CellData)
            {
                goalCell.SetCellColor(_solutionColor);
            }
        }

    }

}
