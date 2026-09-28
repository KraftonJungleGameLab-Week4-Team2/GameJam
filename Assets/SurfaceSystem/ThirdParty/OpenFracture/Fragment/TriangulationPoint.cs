// OpenFracture 원본 절단 알고리즘. 출처와 변경 사항은 LICENSE.txt에 기록한다.
namespace SurfaceSystem.OpenFracture
{
using UnityEngine;
public class TriangulationPoint: IBinSortable
{
    public Vector2 coords;
    public int bin { get; set; }
    public int index = 0;
    public TriangulationPoint(int index, Vector2 coords)
    {
        this.index = index;
        this.coords = coords;
    }
    public override string ToString()
    {
        return $"{coords} -> {bin}";
    }
}
}

