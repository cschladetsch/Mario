using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Reuniter
{

    internal class ReUniterItemInfo
    {

        public ReUniterItemInfo(string fullPath, params Object[] unityObjects)
        {
            FullPath = fullPath;
            UnityObjects = unityObjects;
        }

        public string FullPath { get; set; }
        public Object[] UnityObjects { get; set; }
    }

    internal class ReUniterMode
    {
        public Func<string, IEnumerable<ReUniterItemInfo>> RefreshAction { get; set; }
        public Func<ReUniterItemInfo, Object[]> LoadItem { get; set; }

        public string SearchLabel { get; set; }
    }

    public class ReUniterWindow : EditorWindow
    {

        private string itemName = "";
        private string previousItemName;

        private IEnumerable<ReUniterItemInfo> itemInfos = NoResults;
        private static readonly GUIStyle richTextGuiStyle = new GUIStyle { richText = true, fontSize = 12, margin = new RectOffset(5, 5, 5, 5), normal = { textColor = EditorStyles.label.normal.textColor } }; // TODO black color for unity free
        private static readonly GUIStyle rightAlignRichTextGuiStyle = new GUIStyle { richText = true, fontSize = 12, margin = new RectOffset(5,5,5,5), 
            alignment = TextAnchor.MiddleRight, normal = {textColor = new Color(.4f,.4f,.4f)}};

        private  Texture2D selectedLineBackgroundTex = MakeTex(600, 1, new Color(.1f, .5f, .9f, .5f));
        private  Texture2D searchLineBackgroundTex = MakeTex(600, 1, new Color(.9f, .9f, .9f));

        private readonly GUIStyle selectedLineGuiStyle;
        private readonly GUIStyle searchLineGuiStyle;

        private readonly GUIStyle regularLineGuiStyle = new GUIStyle {richText = true, normal = {textColor = new Color(0, 0, 0, 0)}};

        private static readonly ReUniterItemInfo[] NoResults = { };
        private int selectedIndex = 0;
        private bool selectAll;
        private const int ROW_HEIGHT = 24;

        private const float WINDOW_WIDTH = 600;
        private const float WINDOW_HEIGHT = 70;
        private ReUniterMode mode;

        public ReUniterWindow() 
        {
            selectedLineGuiStyle = new GUIStyle
                {
                    richText = true,
                    normal = {background = selectedLineBackgroundTex, textColor = new Color(0, 0, 0, 0)},
                };
            searchLineGuiStyle = new GUIStyle
                {
                    richText = true,
                    fontSize = 12,
                    normal = {background = searchLineBackgroundTex},
                    margin = new RectOffset(5, 5, 10, 10),
                };
        }

        public void OnDestroy()
        {
            DestroyImmediate(selectedLineBackgroundTex);
            DestroyImmediate(searchLineBackgroundTex);
        }



        [MenuItem("Tools/ReUniter/Go To Asset %t")]
        static void GoToAsset()
        {
            ShowWindow(new ReUniterMode
                {
                    RefreshAction = RefreshAssetInfos,
                    SearchLabel = "Enter Asset Name:",
                    LoadItem = LoadAsset
                });
        }


        [MenuItem("Tools/ReUniter/Go To Game Object %g")]
        static void GoToGameObject()
        {
            ShowWindow(new ReUniterMode
                {
                    RefreshAction = RefreshGameObjectInfos,
                    SearchLabel = "Enter Game Object Name:",
                    LoadItem = LoadUnityObjects
                });
        }

        [MenuItem("Tools/ReUniter/Recent Items %e")]
        static void RecentItems()
        {
            var recentItemsMode = new ReUniterMode
            {
                RefreshAction = RefreshRecentItemInfos,
                SearchLabel = "Enter Recent Item Name:",
                LoadItem = LoadUnityObjects
            };

            if (previousWindow != null && previousWindow.mode.SearchLabel == recentItemsMode.SearchLabel)
            {
                previousWindow.selectAll = false;
                previousWindow.selectedIndex++;
                previousWindow.Repaint();
            }
            else
                ShowWindow(recentItemsMode);
        }

        [MenuItem("Tools/ReUniter/Clear Recent Items")]
        static void ClearRecentItems()
        {
            ReUniterSelectionHistoryTracker.PreviousSelections.Clear();   
        }

        private static IEnumerable<ReUniterItemInfo> RefreshRecentItemInfos(string itemName)
        {
            return ReUniterSelectionHistoryTracker.PreviousSelections.Where(x => x.Any(y => y!=null && y.name.ToLower().StartsWith(itemName.ToLower()))).Take(14)
                                   .Select(x => new ReUniterItemInfo(PathForObjects(x), x));
        }

        private static string PathForObjects(Object[] objects)
        {
            var result = new StringBuilder().Append(objects.Length).Append(" item(s)\\");
            objects._Each(x=>result.Append(x.name).Append(", "));
            result.Remove(result.Length - 2, 2);
            return result.ToString();
        }

        private static ReUniterWindow previousWindow;

        private static void ShowWindow(ReUniterMode mode)
        {
            if (previousWindow != null)
            {
                if (previousWindow.mode.SearchLabel == mode.SearchLabel) //reusing existing window
                    return;
                previousWindow.Close();
            }
            var reUniter = CreateInstance<ReUniterWindow>();
            reUniter.wantsMouseMove = true;
            reUniter.mode = mode;
            reUniter.ShowAsDropDown(PositionRect(WINDOW_HEIGHT),
                                    new Vector2(WINDOW_WIDTH, WINDOW_HEIGHT));
            previousWindow = reUniter;
        }

        private static Rect PositionRect(float windowHeight)
        {
            return new Rect(Screen.currentResolution.width / 2 - WINDOW_WIDTH / 2,
                Screen.currentResolution.height / 2 - WINDOW_HEIGHT * 1.5f, WINDOW_WIDTH, windowHeight);
        }


        private static Texture2D MakeTex(int width, int height, Color col)
        {
            var pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            var result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void OnGUI()
        {
            if (mode==null)
                return;
            if (KeyDown(KeyCode.Escape))
            {
                Close();
                return;
            }
            DetectMouseHover();
            if (KeyDown(KeyCode.DownArrow))
            {
                selectAll = false;
                selectedIndex++;
            }
            if (KeyDown(KeyCode.UpArrow))
            {
                selectAll = false;
                selectedIndex--;
            }
            
            if (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "SelectAll"){
                selectAll = !selectAll;
	            Event.current.Use();
            }
            if (string.IsNullOrEmpty(itemName))
            {
                selectAll = false;
            }

            var enterPressed = KeyDown(KeyCode.Return) || KeyDown(KeyCode.Tab);
          
            var shiftPressed = Event.current.shift;
            var controlPressed = IsOSX() ? Event.current.command : Event.current.control;

            previousItemName = itemName;
            DisplaySearchTextField();

            RefreshFileNames();

            if (Event.current.type == EventType.MouseDown)
            {
                var mousePositionIndex = MousePositionIndex();
                if (mousePositionIndex >= 0 && mousePositionIndex < itemInfos.Count())
                    enterPressed = true;
            }
            selectedIndex = itemInfos.Any() ? Mathf.Clamp(selectedIndex, 0, itemInfos.Count() - 1) : -1;
            if (enterPressed)
            {
                if (selectAll)
                    SelectItems(itemInfos, shiftPressed, controlPressed);
                else if (selectedIndex >= 0)
                    SelectItems(new[]{ itemInfos.ElementAt(selectedIndex)}, shiftPressed, controlPressed);
                Close();
                return;
            }

            DisplaySearchResults();
        }

        private static bool IsOSX()
        {
            return Application.platform == RuntimePlatform.OSXEditor;
        }

        private void DetectMouseHover()
        {
            if (Event.current.type == EventType.MouseMove)
            {
                if (Event.current.mousePosition.y > WINDOW_HEIGHT)
                {
                    var hoveredIndex = MousePositionIndex();
                    if (hoveredIndex != selectedIndex)
                    {
                        selectAll = false;
                        selectedIndex = hoveredIndex;
                        Repaint();
                    }
                }
            }
        }

        private static int MousePositionIndex()
        {
            return ((int) (Event.current.mousePosition.y - WINDOW_HEIGHT))/ROW_HEIGHT;
        }

        private void RefreshFileNames()
        {
            if (itemName == "" || previousItemName != itemName)
                itemInfos = mode.RefreshAction(itemName).ToArray();
        }

        private static IEnumerable<ReUniterItemInfo> RefreshAssetInfos(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return NoResults;
            var assets = AssetDatabase.FindAssets(itemName, new[] {"Assets"});
            return assets.Select(x => new ReUniterItemInfo(AssetDatabase.GUIDToAssetPath(x).Substring("Assets/".Length)))
                .Where(x=>ExtractFileName(x.FullPath).StartsWith(itemName, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x.FullPath).Take(14); 
        }

        private static IEnumerable<ReUniterItemInfo> RefreshGameObjectInfos(string itemName)
        {
            if (string.IsNullOrEmpty(itemName))
                return NoResults;
            var allGameObjects = FindObjectsOfType(typeof(GameObject));
            return allGameObjects.Where(x => x.name.ToLower().StartsWith(itemName.ToLower())).Take(14)
                .Select(x=> new ReUniterItemInfo(BuildParentPath(x), x)).OrderBy(x => x.FullPath);
        }

        private static string BuildParentPath(Object obj)
        {
            var parentPath = "";
            var transform = ((GameObject) obj).transform;
            while (transform.parent != null)
            {
                parentPath = transform.parent.name + "/" + parentPath;
                transform = transform.parent;
            }
            return parentPath + obj.name;
        }

        private static string ExtractFileName(string fullPath)
        {
            var lastSlash = Math.Max(fullPath.LastIndexOf("\\"), fullPath.LastIndexOf("/"));
            return fullPath.Substring(lastSlash + 1);
        }

        private void DisplaySearchResults()
        {
            itemInfos._Each((x, index) =>
                {
                    var fullPath = x.FullPath;
                    var lastSlash = Math.Max(fullPath.LastIndexOf("\\"), fullPath.LastIndexOf("/"));
                    var path = lastSlash<0 ? "" : fullPath.Substring(0, lastSlash);
                    var fileName = fullPath.Substring(lastSlash+1);                   
                    GUILayout.BeginHorizontal((selectAll || index == selectedIndex) ? selectedLineGuiStyle : regularLineGuiStyle);
                    GUILayout.Label(Highlight(itemName, fileName), richTextGuiStyle);
                    GUI.Label(GUILayoutUtility.GetLastRect(), path, rightAlignRichTextGuiStyle);
                    GUILayout.EndHorizontal();
                });
            ResizeWindow();
        }

        private void ResizeWindow()
        {
            var desiredHeight = WINDOW_HEIGHT + itemInfos.Count()*ROW_HEIGHT;
            if (Math.Abs(position.height - desiredHeight) > 1)
            {
                maxSize = new Vector2(position.width, desiredHeight);
                minSize = new Vector2(position.width, desiredHeight);
                maxSize = new Vector2(position.width, desiredHeight);
                position = PositionRect(desiredHeight);
            }
        }

        private void DisplaySearchTextField()
        {
            EditorGUILayout.LabelField(mode.SearchLabel, EditorStyles.label);
            const string controlName = "vh_asset_name";
            GUI.SetNextControlName(controlName);
            itemName = GUILayout.TextField((itemName ?? "").Replace("`", ""), searchLineGuiStyle, GUILayout.ExpandWidth(true)).Replace("`", "");
            GUI.FocusControl(controlName);
            ForceCaretToEndOfTextField();
        }

        private void ForceCaretToEndOfTextField()
        {
            var te = (TextEditor) GUIUtility.GetStateObject(typeof (TextEditor), GUIUtility.keyboardControl);            
//            te.MoveCursorToPosition(new Vector2(5555, 5555));
            te.pos = itemName.Length;
            te.selectPos = itemName.Length;
        }

        private string Highlight(string text, string highlight)
        {
            return Regex.Replace(highlight,Regex.Escape(text),"<b>$0</b>", RegexOptions.IgnoreCase);
        }

        private bool KeyDown(KeyCode keyCode)
        {
            return Event.current.type == EventType.keyDown && Event.current.keyCode == keyCode;
        }

        private void SelectItems(IEnumerable<ReUniterItemInfo> itemInfos, bool appendToSelection, bool openAssets)
        {
            var newSelection = itemInfos.Select(mode.LoadItem).SelectMany(x=>x);
            if (appendToSelection)
                newSelection = Selection.objects.ToList().Union(newSelection);
            Selection.objects = newSelection.ToArray();
            if (!openAssets)
            {
                bool allGameObjects = Selection.objects.All(x => x is GameObject);
                bool noneGameObject = !Selection.objects.Any(x => x is GameObject);

                if (allGameObjects)
                    EditorApplication.ExecuteMenuItem("Window/Hierarchy");
                if (noneGameObject)
                    EditorUtility.FocusProjectWindow();

                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            if (openAssets)
                newSelection._Each(x=>AssetDatabase.OpenAsset(x));
        }

        private static Object[] LoadAsset(ReUniterItemInfo itemInfo)
        {
            return new []{AssetDatabase.LoadAssetAtPath("Assets/" + itemInfo.FullPath, typeof (Object))};
        }

        private static Object[] LoadUnityObjects(ReUniterItemInfo itemInfo)
        {
            return itemInfo.UnityObjects;
        }


    }

    public static class IEnumerableExtensions
    {
        public static void _Each<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
                action(item);
        }

        public static void _Each<T>(this IEnumerable<T> ie, Action<T, int> action)
        {
            var i = 0;
            foreach (var e in ie) action(e, i++);
        }
    }
}