namespace Project.Procedural.MazeGeneration
{
    public class EllerDemo : MazeGenerator
    {
        public override void SetupGrid()
        {
            Grid = new ColoredGrid(Settings);
        }

        public override void Generate()
        {
            Eller algorithm = new();
            algorithm.Execute(Grid);
            
            Cell start = Grid[Grid.Rows -1, Grid.Columns / 2];
            (Grid as ColoredGrid).SetDistances(start.GetDistances());
        }
    }
}