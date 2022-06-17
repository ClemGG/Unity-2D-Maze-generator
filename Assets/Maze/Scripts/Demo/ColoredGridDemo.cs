using UnityEngine;

namespace Project.Procedural.MazeGeneration
{
    public class ColoredGridDemo : MonoBehaviour
    {
        [field: SerializeField] private Vector2Int GridSize { get; set; } = new(4, 4);
        [field: SerializeField] private GenerationType GenerationType { get; set; } = GenerationType.BinaryTree;


#if UNITY_EDITOR

        private void OnValidate()
        {
            GridSize = new(Mathf.Clamp(GridSize.x, 1, 100), Mathf.Clamp(GridSize.y, 1, 100));
        }
#endif

        [ContextMenu("Execute Generation Algorithm")]
        void Execute()
        {
            var grid = new ColoredGrid(GridSize.x, GridSize.y);
            grid.Execute(GenerationType);

            Cell start = grid[grid.Rows / 2, grid.Columns / 2];
            grid.SetDistances(start.GetDistances());

            grid.DisplayGrid(DisplayMode.UIImage);
        }
    }
}