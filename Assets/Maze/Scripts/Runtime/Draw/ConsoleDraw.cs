#if UNITY_EDITOR
using UnityEngine;

namespace Project.Procedural.MazeGeneration
{
    public class ConsoleDraw : IDraw
    {

        public void Cleanup()
        {
            // This simply does "LogEntries.Clear()" the long way:
            var logEntries = System.Type.GetType("UnityEditor.LogEntries,UnityEditor.dll");
            var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
            clearMethod.Invoke(null, null);
        }


        public void Draw(Grid grid)
        {
            Debug.Log(grid.ToString());
        }
    }
}

#endif