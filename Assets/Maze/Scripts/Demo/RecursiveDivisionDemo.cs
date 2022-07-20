using UnityEngine;

namespace Project.Procedural.MazeGeneration
{
    public class RecursiveDivisionDemo : MonoBehaviour
    {
        [field: SerializeField] private bool BiasTowardsRooms { get; set; }
        [field: SerializeField] private Vector2Int RoomSize { get; set; } = new(5, 5);
        [field: SerializeField] private Vector2Int GridSize { get; set; } = new(4, 4);


#if UNITY_EDITOR

        private void OnValidate()
        {
            RoomSize = new(Mathf.Clamp(RoomSize.x, 1, 100), Mathf.Clamp(RoomSize.y, 1, 100));
            GridSize = new(Mathf.Clamp(GridSize.x, 1, 100), Mathf.Clamp(GridSize.y, 1, 100));
        }
#endif


        [ContextMenu("Cleanup UI")]
        void CleanupUI()
        {
            OrthogonalMaze.CleanupUI();
        }

        [ContextMenu("Execute Generation Algorithm")]
        void Execute()
        {
            var grid = new ColoredGrid(GridSize.x, GridSize.y);
            RecursiveDivision.Init(RoomSize, BiasTowardsRooms);
            grid.Execute(GenerationType.RecursiveDivision);

            Cell start = grid[grid.Rows / 2, grid.Columns / 2];
            grid.SetDistances(start.GetDistances());

            grid.DisplayGrid(DisplayMode.UIImage);
        }
    }
}