using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Procedural.MazeGeneration
{
    //Draws square-shaped mazes (gamma-type mazes).
    //Used for 2D & 3D.
    public static class OrthogonalMaze
    {

        #region UI Fields
        private static Canvas _canvas;
        private static RectTransform _bg;
        private static Transform _tiles;
        private static Transform _lines;
        private static Transform _imgHolder;

        private static Canvas Canvas
        {
            get
            {
                if (!_canvas) _canvas = Object.FindObjectOfType<Canvas>();
                return _canvas;
            }
        }
        private static RectTransform Bg
        {
            get
            {
                if (!_bg) _bg = (RectTransform)Canvas.transform.GetChild(0);
                return _bg;
            }
        }
        private static Transform Tiles
        {
            get
            {
                if (!_tiles) _tiles = Bg.transform.GetChild(0);
                return _tiles;
            }
        }
        private static Transform Lines
        {
            get
            {
                if (!_lines) _lines = Bg.transform.GetChild(1);
                return _lines;
            }
        }
        private static Transform ImgHolder
        {
            get
            {
                if (!_imgHolder) _imgHolder = Bg.parent.GetChild(1);
                return _imgHolder;
            }
        }

        #endregion

        #region Mesh Fields

        private static GameObject _mazeObj;
        private static GameObject MazeObj
        {
            get
            {
                if (!_mazeObj) _mazeObj = GameObject.Find("Maze");
                return _mazeObj;
            }
        }

        #endregion




        #region 3D Mesh

        public static void DisplayOnMesh(Grid grid, float inset = 0f)
        {
            float cellWidth = 3.5f;
            float cellHeight = 3.5f;
            float halfH = cellHeight / 2f;
            inset = cellWidth * inset;

            Mesh mesh = new Mesh
            {
                subMeshCount = 2
            };

            List<Vector3> newVertices = new List<Vector3>();
            List<Vector2> newUVs = new List<Vector2>();

            List<int> floorTriangles = new List<int>();
            List<int> wallTriangles = new List<int>();


            for (int i = 0; i < grid.Rows; i++)
            {
                for (int j = 0; j < grid.Columns; j++)
                {
                    Cell cell = grid[i, j];

                    if (cell is null) continue;


                    if (!Mathf.Approximately(inset, 0f) && !Mathf.Approximately(inset, .5f * cellWidth))
                    {
                        float x = cell.Column * cellWidth;
                        float y = cell.Row * cellWidth;

                        //The triangles of the floor & ceiling will be in the same mesh
                        AddFloorAndCeilingWithInset(grid, cell, cellWidth, i, j, x, y, inset, ref newVertices, ref newUVs, ref floorTriangles);
                        AddWallsWithInset(grid, cell, cellWidth, cellHeight, i, j, x, y, inset, ref newVertices, ref newUVs, ref wallTriangles);
                    }
                    else
                    {

                        //The triangles of the floor & ceiling will be in the same mesh
                        AddFloorAndCeilingWithoutInset(grid, cell, cellWidth, i, j, ref newVertices, ref newUVs, ref floorTriangles);
                        AddWallsWithoutInset(grid, cell, cellWidth, cellHeight, i, j, ref newVertices, ref newUVs, ref wallTriangles);

                    }
                }
            }


            mesh.vertices = newVertices.ToArray();
            mesh.uv = newUVs.ToArray();

            mesh.SetTriangles(wallTriangles.ToArray(), 0);
            mesh.SetTriangles(wallTriangles.ToArray(), 1);

            mesh.RecalculateNormals();

            MeshFilter mf = MazeObj.GetComponent<MeshFilter>();
            MeshCollider mc = MazeObj.GetComponent<MeshCollider>();
            mf.mesh = mc.sharedMesh = mesh;

        }


        private static void AddFloorAndCeilingWithInset(Grid grid, Cell cell, float cellWidth, int i, int j, float x, float y, float inset, 
            ref List<Vector3> newVertices, ref List<Vector2> newUVs, ref List<int> floorTriangles)
        {

        }

        private static void AddWallsWithInset(Grid grid, Cell cell, float cellWidth, float cellHeight, int i, int j, float x, float y, float inset,
            ref List<Vector3> newVertices, ref List<Vector2> newUVs, ref List<int> wallTriangles)
        {

        }


        private static void AddFloorAndCeilingWithoutInset(Grid grid, Cell cell, float cellWidth, int i, int j, 
            ref List<Vector3> newVertices, ref List<Vector2> newUVs, ref List<int> floorTriangles)
        {

        }

        private static void AddWallsWithoutInset(Grid grid, Cell cell, float cellWidth, float cellHeight, int i, int j, 
            ref List<Vector3> newVertices, ref List<Vector2> newUVs, ref List<int> wallTriangles)
        {

        }

        #endregion


        #region UI

        public static void DisplayOnUI(Grid grid, float inset = 0f)
        {
            //cleanup pooler.
            //stored temp. in an array to avoid Bg's resizing
            int nbCells = Tiles.childCount;
            int nbLines = Lines.childCount;
            List<Transform> children = new(nbCells + nbLines);
            for (int i = 0; i < nbCells; i++)
            {
                children.Add(Tiles.GetChild(i));
            }
            for (int i = 0; i < nbLines; i++)
            {
                children.Add(Lines.GetChild(i));
            }
            for (int i = 0; i < children.Count; i++)
            {
                Transform child = children[i];
                child.SetParent(ImgHolder);
                child.gameObject.SetActive(false);
                MazePrefabs.UIImagePooler.ReturnToPool(child.gameObject, child.name.Replace("(Clone)", ""));
            }



            float cellSize = Mathf.Min(Bg.rect.width / grid.Columns, Bg.rect.height / grid.Rows);
            inset = cellSize * inset;


            for (int i = 0; i < grid.Rows; i++)
            {
                for (int j = 0; j < grid.Columns; j++)
                {
                    Cell cell = grid[i, j];

                    if (cell is null) continue;

                    if (!Mathf.Approximately(inset, 0f) && !Mathf.Approximately(inset, .5f * cellSize))
                    {
                        float x = cell.Column * cellSize;
                        float y = cell.Row * cellSize;

                        DisplayCellImgWithInset(grid, cell, cellSize, i, j, x, y, inset);
                        DisplayLineImgWithInset(cell, cellSize, x, y, inset);
                    }
                    else
                    {
                        DisplayCellImgWithoutInset(grid, cell, cellSize, i, j);
                        DisplayLineImgWithoutInset(cell, cellSize);
                    }
                }
            }


        }




        private static (Vector4, Vector4) CellCoordsWithInset(float x, float y, float cellSize, float inset)
        {
            float x1 = x;
            float x4 = x + cellSize;
            float x2 = x1 + inset;
            float x3 = x4 - inset;

            float y1 = y;
            float y4 = y + cellSize;
            float y2 = y1 + inset;
            float y3 = y4 - inset;

            return (new(x1, x2, x3, x4), new( y1, y2, y3, y4));
        }



        private static void DisplayCellImgWithInset(Grid grid, Cell cell, float cellSize, int i, int j, float x, float y, float inset)
        {
            (Vector4 xc, Vector4 yc) = CellCoordsWithInset(x, y, cellSize, inset);
            float x1 = xc.x;
            float x2 = xc.y;
            float x3 = xc.z;
            float x4 = xc.w;

            float y1 = yc.x;
            float y2 = yc.y;
            float y3 = yc.z;
            float y4 = yc.w;

            //Draws the img for the center of the cell
            DrawCell(new Vector2(cellSize - inset * 2f, cellSize - inset * 2f),
                new Vector3(x2, -y2, 0),
                //Color.black);
                (cell is null) ? Color.black : grid.BackgroundColorFor(cell));


            //Draws 2 imgs to fill the outer regions of the cell
            if (cell.IsLinked(cell.North))
            {
                DrawCell(new Vector2(cellSize - inset * 2f, inset),
                new Vector3(x2, -y1, 0),
                //Color.red);
                (cell is null) ? Color.black : grid.BackgroundColorFor(cell));
            }
            if (cell.IsLinked(cell.West))
            {
                DrawCell(new Vector2(inset, cellSize - inset * 2f),
                new Vector3(x1, -y2, 0),
                //Color.blue);
                (cell is null) ? Color.black : grid.BackgroundColorFor(cell));
            }
            if (cell.IsLinked(cell.East))
            {
                DrawCell(new Vector2(inset, cellSize - inset * 2f),
                new Vector3(x3, -y2, 0),
                //Color.yellow);
                (cell is null) ? Color.black : grid.BackgroundColorFor(cell));
            }
            if (cell.IsLinked(cell.South))
            {
                DrawCell(new Vector2(cellSize - inset * 2f, inset),
                new Vector3(x2, -y3, 0),
                //Color.green);
                (cell is null) ? Color.black : grid.BackgroundColorFor(cell));
            }
        }


        private static void DisplayLineImgWithInset(Cell cell, float cellSize, float x, float y, float inset)
        {
            //width and height of the UI Image in pixels
            //TODO : Scale these sizes for smaller cells
            float lineThickness = Mathf.Lerp(5f, 1f, inset / cellSize / 0.5f);


            Vector2 anchorH = new(0f, 1f);
            Vector2 pivotH = new(0f, 1f);
            Vector2 anchorV = new(0f, 1f);
            Vector2 pivotV = new(0.5f, 1f);

            (Vector4 xc, Vector4 yc) = CellCoordsWithInset(x, y, cellSize, inset);
            float x1 = xc.x;
            float x2 = xc.y;
            float x3 = xc.z;
            float x4 = xc.w;

            float y1 = yc.x;
            float y2 = yc.y;
            float y3 = yc.z;
            float y4 = yc.w;

            Vector3 pos, size;

            if (cell.IsLinked(cell.North))
            {
                size = new(lineThickness, inset + lineThickness);
                pos = new(x2, -y1);

                //Line V
                DrawLine(anchorV, pivotV, size, pos);

                pos = new(x3, -y1);

                //Line V
                DrawLine(anchorV, pivotV, size, pos);
            }
            else
            {
                size = new(cellSize - inset * 2f, lineThickness);
                pos = new(x2, -y2);

                //Line H
                DrawLine(anchorH, pivotH, size, pos);
            }
            if (cell.IsLinked(cell.South))
            {
                size = new(lineThickness, inset + lineThickness);
                pos = new(x2, -y3);

                //Line V
                DrawLine(anchorV, pivotV, size, pos);

                pos = new(x3, -y3);

                //Line V
                DrawLine(anchorV, pivotV, size, pos);
            }
            else
            {

                size = new(cellSize - inset * 2f, lineThickness);
                pos = new(x2, -y3);

                //Line H
                DrawLine(anchorH, pivotH, size, pos);
            }
            if (cell.IsLinked(cell.West))
            {
                size = new(inset, lineThickness);
                pos = new(x1, -y2);

                //Line H
                DrawLine(anchorH, pivotH, size, pos);

                pos = new(x1, -y3);

                //Line H
                DrawLine(anchorH, pivotH, size, pos);
            }
            else
            {

                size = new(lineThickness, cellSize - inset * 2f + lineThickness);
                pos = new(x2, -y2);

                //Line V
                DrawLine(anchorV, pivotV, size, pos);
            }
            if (cell.IsLinked(cell.East))
            {
                size = new(inset, lineThickness);
                pos = new(x3, -y2);

                //Line H
                DrawLine(anchorH, pivotH, size, pos);

                pos = new(x3, -y3);

                //Line H
                DrawLine(anchorH, pivotH, size, pos);
            }
            else
            {

                size = new(lineThickness, cellSize - inset * 2f + lineThickness);
                pos = new(x3, -y2);

                //Line V
                DrawLine(anchorV, pivotV, size, pos);
            }
        }


        private static void DisplayCellImgWithoutInset(Grid grid, Cell cell, float cellSize, int i, int j)
        {
            DrawCell(new Vector2(cellSize, cellSize),
                new Vector3(cellSize * j, -cellSize * i, 0),
                (cell is null) ? Color.black : grid.BackgroundColorFor(cell));
        }
        
        
        private static void DisplayLineImgWithoutInset(Cell cell, float cellSize)
        {
            //width and height of the UI Image in pixels
            float lineThickness = 5f;


            Vector2 anchorH = new(0f, 1f);
            Vector2 pivotH = new(0f, 0.5f);
            Vector2 anchorV = new(0f, 1f);
            Vector2 pivotV = new(0.5f, 1f);

            if (cell.North is null)
            {
                //Line North
                DrawLine(anchorH, pivotH,
                    new Vector2(cellSize, lineThickness), 
                    new Vector3(cellSize * cell.Column, -cellSize * cell.Row, 0));
            }

            if (cell.West is null)
            {
                //Line West
                DrawLine(anchorV, pivotV,
                    new Vector2(lineThickness, cellSize + lineThickness),
                    new Vector3(cellSize * cell.Column, -cellSize * cell.Row + lineThickness / 2f, 0));
            }

            if (!cell.IsLinked(cell.East))
            {
                //Line East
                DrawLine(anchorV, pivotV,
                    new Vector2(lineThickness, cellSize + lineThickness),
                    new Vector3(cellSize * (cell.Column + 1), -cellSize * cell.Row + lineThickness / 2f, 0));
            }

            if (!cell.IsLinked(cell.South))
            {
                //Line South
                DrawLine(anchorH, pivotH,
                    new Vector2(cellSize, lineThickness),
                    new Vector3(cellSize * cell.Column, -cellSize * (cell.Row + 1), 0));
            }
        }



        public static void CleanupUI()
        {

            //cleanup pooler.
            //stored temp. in an array to avoid Bg's resizing
            int nbCells = Tiles.childCount;
            int nbLines = Lines.childCount;
            int nbHeld = ImgHolder.childCount;
            List<Transform> children = new(nbCells + nbLines + nbHeld);
            for (int i = 0; i < nbCells; i++)
            {
                children.Add(Tiles.GetChild(i));
            }
            for (int i = 0; i < nbLines; i++)
            {
                children.Add(Lines.GetChild(i));
            }
            for (int i = 0; i < nbHeld; i++)
            {
                children.Add(ImgHolder.GetChild(i));
            }
            for (int i = 0; i < children.Count; i++)
            {
                Object.DestroyImmediate(children[i].gameObject);
            }
        }

        private static void DrawCell(Vector2 size, Vector3 anchoredPos, Color col)
        {
            RectTransform cellImg = MazePrefabs.UIImagePooler.GetFromPool<GameObject>("cell ui img").GetComponent<RectTransform>();
            cellImg.SetParent(Tiles);
            cellImg.gameObject.SetActive(true);

            cellImg.pivot = cellImg.anchorMin = cellImg.anchorMax = new Vector2(0f, 1f);
            cellImg.anchoredPosition = anchoredPos;
            cellImg.localScale = Vector3.one;

            cellImg.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            cellImg.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            cellImg.GetComponent<Image>().color = col;
        }

        private static void DrawLine(Vector2 anchor, Vector2 pivot, Vector2 size, Vector3 anchoredPos)
        {
            RectTransform line = MazePrefabs.UIImagePooler.GetFromPool<GameObject>("line ui img").GetComponent<RectTransform>();
            line.SetParent(Lines);
            line.gameObject.SetActive(true);

            line.anchorMin = line.anchorMax = anchor;
            line.pivot = pivot;
            line.anchoredPosition = anchoredPos;
            line.localScale = Vector3.one;

            line.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            line.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
        }
        #endregion
    }
}