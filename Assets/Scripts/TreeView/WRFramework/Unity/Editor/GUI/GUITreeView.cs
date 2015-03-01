// MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM //
//                                                              //
// Copyright 2013 wHiteRabbiT sTudio whiterabbitstudio@live.fr  //
//                                                              //
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY    //
// KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE   //
// WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR      //
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS   //
// OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR     //
// OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR   //
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE    //
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.       //
//                                                              //
// MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM //

using UnityEditor;

using UnityEngine;
using wHiteRabbiT.Unity.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace wHiteRabbiT.Unity.UI
{
	public class GUITreeView
	{
		public class CheckedExpandedElement
		{
			public bool Checked;
			public bool Expanded;
			public string fullName = "";
			public string Name = "";			
		}
		public CheckedExpandedElement element;
		
		public GUITreeView(string rootName)
		{			
			element = new CheckedExpandedElement();
			element.fullName = rootName;
			element.Name = rootName;
		}
		
		public bool isOrChildChecked;
		public bool RefreshIsOrChildChecked()
		{
			// Don't check folders
			if (children.Count > 0)
				element.Checked = false;
			
			isOrChildChecked = element.Checked;
			
			foreach(var child in children)
				if (child.RefreshIsOrChildChecked())
					isOrChildChecked = true;
			
			return isOrChildChecked;
		}
		
		public string nodePath
		{
			get {
				return parent == null || string.IsNullOrEmpty(parent.element.Name) ?
						""
					:	parent.nodePath + parent.element.Name + "/";
			}
		}
		
		public GUITreeView parent;
		public List<GUITreeView> children = new List<GUITreeView>();
		
		protected GUITreeView CreateChild(string s)
		{
			foreach(GUITreeView tve in children)
				if (tve.element.fullName == s)
					return tve;
			
			GUITreeView tve0 = CreateNode(s, Path.GetFileName(s));
			
			// Sort folders before files
			if (tve0.element.Name.Contains("."))
				children.Add(tve0);
			else
			{
				int i=0;
				foreach(var child in children)
				{
					if (child.element.Name.Contains("."))
						break;
					i++;
				}
				
				children.Insert(i, tve0);
			}
			
			return tve0;
		}
		
		protected virtual GUITreeView CreateNode(string fullName, string Name)
		{
			return new GUITreeView(Name) {					
				element = new CheckedExpandedElement {
					fullName = fullName,
					Name = Name
				},
				parent = this
			};
		}
		
		public void AddTreeElement(string s)
		{
			if (parent != null)
			{
				parent.AddTreeElement(s);
				return ;
			}
			
			List<string> folders = new List<string>(s.Split(new string[] {"/"}, System.StringSplitOptions.RemoveEmptyEntries));
			
			folders.RemoveAt(folders.Count - 1);
			
			AddTreeElement(s, folders);
		}			
		protected void AddTreeElement(string s, List<string> folders)
		{
			if (folders.Count == 0)
				CreateChild(s);
			else
			{
				GUITreeView tve = CreateChild(nodePath + element.Name + (string.IsNullOrEmpty(element.Name) ? "" : "/") + folders[0]);
				folders.RemoveAt(0);
				
				tve.AddTreeElement(s, folders);
			}
		}
		
		public int NbExpanded
		{
			get {
				
				if (!element.Expanded)
					return 1;
				
				int nb = 1;					
				
				foreach(GUITreeView tve in children)
					nb += tve.NbExpanded;
				
				return nb;
			}
		}
		
		public List<GUITreeView> AllElements
		{
			get {
				List<GUITreeView> tve = new List<GUITreeView>();
				
				tve.Add(this);
				
				foreach(var child in children)
					tve.AddRange(child.AllElements);
				
				return tve;
			}
		}
		
		public List<GUITreeView> SelectedElements
		{
			get {
				List<GUITreeView> tve = new List<GUITreeView>();
				
				if (element.Checked)
					tve.Add(this);
				
				foreach(var child in children)
					tve.AddRange(child.SelectedElements);
				
				return tve;
			}
		}
		
		public List<string> SelectedFullNames
		{
			get {
				return (from se in SelectedElements
					select se.element.fullName).ToList();
			}
		}
		
		public void CheckRec(bool Checked)
		{
			element.Checked = Checked;
			
			foreach(var child in children)
				child.CheckRec(Checked);
		}
				
		
		protected void DrawElement(ref Rect r, CheckedExpandedElement cee, bool ShowExpanded, bool bold)
		{
			DrawElement(ref r, cee, ShowExpanded, bold, new Color(0.6f,0.6f,1.0f,1.0f));
		}
		protected void DrawElement(ref Rect r, CheckedExpandedElement cee, bool ShowExpanded, bool bold, Color BoldColor)
		{			
			Rect r2 = r;
			
			if (ShowExpanded)
			{
				cee.Expanded = EditorGUI.Foldout(r2, cee.Expanded, "");
				r2.x += 13;
				r2.width = 1000;

				GUIStyle es = new GUIStyle(bold ? EditorStyles.boldLabel : EditorStyles.label);
				es.normal.textColor = bold ? BoldColor : es.normal.textColor;

				GUI.Label(r2, cee.Name, es);
			}
			else
			{
				r2.width = 1000;
				
				if (UseCheckbox)
				{
				GUIStyle es = new GUIStyle(EditorStyles.toggle);
				es.fontStyle = bold ? FontStyle.Bold : es.fontStyle;
				es.onNormal.textColor = BoldColor;
				
				cee.Checked = GUI.Toggle(r2, cee.Checked, cee.Name, es);
				}
				
				else
				{
					r2.x += 13;
					GUIStyle es = new GUIStyle(bold ? EditorStyles.boldLabel : EditorStyles.label);
					es.normal.textColor = bold ? BoldColor : es.normal.textColor;
	
					GUI.Label(r2, cee.Name, es);
				}
			}
			
			r.y += r.height;
		}
		
		protected void DrawGUITreeView(ref Rect r, GUITreeView tve)
		{
			DrawElement(ref r, tve.element, tve.ShowElementExpanded, tve.isOrChildChecked);
			
			if (!tve.element.Expanded)
				return;
			
			Rect r2 = r;
			r2.x += 20;			
			
			foreach(GUITreeView child in tve.children)
				DrawGUITreeView(ref r2, child);
			
			r.y = r2.y;
		}
		
		protected Vector2 scrollPosition;
		public void DrawGUILayout(float Height)//, bool DragDropDependencies)		
		{
			RefreshIsOrChildChecked();
			
			Rect r0 = GUILayoutUtility.GetLastRect();
			float xWidth = r0.width;
			float yMin = r0.y + r0.height;
			float yHeight = Height;//Screen.height - yMin; // Min with max Height)
			
			// For Layout
			GUILayout.Box("", GUIStyle.none, GUILayout.Height(yHeight));
			
			GUIDragAndDrop.DrawGUI(new Rect(0, yMin, xWidth, yHeight), Color.blue, "", this,/* (tw) => {
				foreach(string path in DragAndDrop.paths)
					if (tw.CheckIfExists(path, true) && DragDropDependencies)
						foreach(string dpath in AssetDatabase.GetDependencies(new string[] {path}))
							tw.CheckIfExists(dpath, true);
			}*/ DragAndDropCallBack);
			
			float elementHeight = 20;
			
			scrollPosition = GUI.BeginScrollView(
				new Rect(0, yMin, xWidth, yHeight),
				scrollPosition,
				new Rect(0, 0, 1000, NbExpanded * elementHeight));
			
			Rect r = new Rect(0, 0, 1000, elementHeight);
			
			DrawGUITreeView(ref r, this);
			
			GUI.EndScrollView();
		}
		
		public virtual void DragAndDropCallBack(GUITreeView treeView)
		{
		}

		public virtual bool ShowElementExpanded
		{
			get {
				return children != null && children.Count>0;
			}
		}
		
		public bool UseCheckbox = false;
	}
	
	public class GUITreeViewFolder : GUITreeView
	{
		public bool DragDropDependencies;
		
		public GUITreeViewFolder(string rootName) : base(rootName)
		{
			UseCheckbox = true;
		}
		public GUITreeViewFolder(string rootName, string[] FileFoldersList) : this(rootName, new List<string>(FileFoldersList))
		{			
		}
		public GUITreeViewFolder(string rootName, List<string> FileFoldersList) : base(rootName)
		{
			foreach(string s in FileFoldersList.OrderBy(x => x).ToList())
				AddTreeElement(s);
		}
		
		public bool CheckIfExists(string path, bool forceExpand)
		{
			if (element.fullName == path)
			{
				element.Checked = true;
				element.Expanded = element.Expanded || forceExpand;
				return true;
			}
			
			foreach(GUITreeViewFolder child in children)
				if(child.CheckIfExists(path, forceExpand))
				{
					element.Expanded = element.Expanded || forceExpand;					
					return true;
				}
			
			return false;
		}
		
		public bool ExpandIfExists(string path)
		{
			if (element.fullName == path)
			{
				element.Expanded = true;
				return true;
			}
			
			foreach(GUITreeViewFolder child in children)
				if (child.ExpandIfExists(path))
					return true;
			
			return false;
		}
		
		protected bool? isDirectory;
		public bool IsDirectory
		{
			get {
				if (isDirectory == null)
					isDirectory = Directory.Exists(Application.dataPath + "/../" + element.fullName);
				
				return isDirectory.Value;
			}
		}

		public override void DragAndDropCallBack (GUITreeView treeView)
		{
			GUITreeViewFolder tvf = treeView as GUITreeViewFolder;
			
			if (tvf == null)
				return;
			
			foreach(string path in DragAndDrop.paths)
				if (tvf.CheckIfExists(path, true) && DragDropDependencies)
					foreach(string dpath in AssetDatabase.GetDependencies(new string[] {path}))
						tvf.CheckIfExists(dpath, true);
		}
		
		public override bool ShowElementExpanded {
			get {
				return IsDirectory;
			}
		}
		
		protected override GUITreeView CreateNode(string fullName, string Name)
		{
			return new GUITreeViewFolder(Name) {					
				element = new CheckedExpandedElement {
					fullName = fullName,
					Name = Name
				},
				parent = this
			};
		}
	}
}