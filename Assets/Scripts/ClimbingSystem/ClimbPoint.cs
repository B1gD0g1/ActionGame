using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbPoint : MonoBehaviour
{
    //是否为最高点
    [SerializeField] private bool mountPoint;
    [SerializeField] List<Neighbour> neighbours;
    

    private void Awake()
    {
        var twoWayNeighbours = neighbours.Where(n => n.isTowWay);

        foreach (var neighbour in twoWayNeighbours)
        {
            neighbour.climbPoint?.CreateConnection(this, -neighbour.direction,
                neighbour.connectionType, neighbour.isTowWay);
        }
    }

    //创建双向连接
    public void CreateConnection(ClimbPoint climbPoint, Vector2 direction, 
        ConnectionType connectionType, bool isTwoWay = true)
    {
        var neighbour = new Neighbour()
        {
            climbPoint = climbPoint,
            direction = direction,
            connectionType = connectionType,
            isTowWay = isTwoWay
        };

        neighbours.Add(neighbour);
    }

    public Neighbour GetNeighbour(Vector2 direction)
    {
        Neighbour neighbour = null;

        if (direction.y != 0)
            neighbour = neighbours.FirstOrDefault(n =>  n.direction.y == direction.y);

        if (neighbour == null && direction.x != 0)
            neighbour = neighbours.FirstOrDefault(n => n.direction.x == direction.x);

        return neighbour;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward, Color.blue);

        foreach (var neighbour in neighbours)
        {
            if (neighbour.climbPoint != null)
            {
                Debug.DrawLine(transform.position, neighbour.climbPoint.transform.position, 
                    (neighbour.isTowWay) ? Color.cyan : Color.yellow);
            }
        }
    }

    public bool MountPoint => mountPoint;
}

[System.Serializable]
public class Neighbour
{
    public ClimbPoint climbPoint;
    public Vector2 direction;
    public ConnectionType connectionType;
    public bool isTowWay = true;
}

public enum ConnectionType
{
    Jump,
    Move
}