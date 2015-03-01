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
using wHiteRabbiT.Unity.UI;

namespace wHiteRabbiT.UnityEditor
{
	public class TreeViewSampleEW : wHiteRabbiTEW
	{

		[MenuItem ("Window/wHiteRabbiT sTudio/Sample/TreeView Sample")]
		public static void CreateCustomPackageEW()
		{
			TreeViewSampleEW wnd = GetWindow<TreeViewSampleEW>();
			wnd.title = "wHiteRabbiT sTudio - TreeView Sample";
		}
		
		#region Destroy
		public override void OnDestroy()
		{
		}
		#endregion
		
		protected GUITreeViewFolder treeView;
		public GUITreeViewFolder TreeViewAssets
		{
			get {
				if (treeView == null)
				{
					treeView = new GUITreeViewFolder("", AssetDatabase.GetAllAssetPaths());
					
					// Expand "Assets" folder
					treeView = treeView.children[0] as GUITreeViewFolder;
					treeView.element.Expanded = true;
					
					treeView.DragDropDependencies = true;
				}
				return treeView;
			}
		}
		
		protected GUITreeView treeView0;
		public GUITreeView TreeView0
		{
			get {
				if (treeView0 == null)
				{
					treeView0 = new GUITreeView("root");
					
					treeView0.AddTreeElement("test0");
					treeView0.AddTreeElement("test1");
					treeView0.AddTreeElement("test0/test00");
					treeView0.AddTreeElement("test0/test01/test010");
					treeView0.AddTreeElement("test1/test10/test100");
				}
				return treeView0;
			}
		}
		
		protected GUITreeView treeView1;
		public GUITreeView TreeView1
		{
			get {
				if (treeView1 == null)
				{
					treeView1 = new GUITreeView("root");
					
					treeView1.AddTreeElement("test0");
					treeView1.AddTreeElement("test1");
					treeView1.AddTreeElement("test0/test00");
					treeView1.AddTreeElement("test0/test01/test010");
					treeView1.AddTreeElement("test1/test10/test100");
					
					treeView1.UseCheckbox = true;
				}
				return treeView1;
			}
		}
		
		public void OnProjectChange()
		{
			var ae = TreeViewAssets.AllElements;
			
			treeView = null;
			
			foreach(var e in ae)
				if (e.element.Checked)
					TreeViewAssets.CheckIfExists(e.element.fullName, e.element.Expanded);
				else if (e.element.Expanded)
					TreeViewAssets.ExpandIfExists(e.element.fullName);
			
			Repaint();
		}
		
		public Vector2 scrollPosition0;
		public override void OnGUI ()
		{
			base.OnGUI();
			
			scrollPosition0 = GUILayout.BeginScrollView(scrollPosition0, GUIStyle.none);			
			
			GUILayout.Label("Simple tree view\n");
			TreeView0.DrawGUILayout(200);
			
			GUILayout.Space(15);
			
			GUILayout.Label("Tree view using checkboxes\n");
			TreeView1.DrawGUILayout(200);
			
			GUILayout.Space(15);
			
			GUILayout.Label("Sample derived class for parsing the asset folder \n");
			TreeViewAssets.DrawGUILayout(300);			
			
			GUILayout.EndScrollView();
		}
	}		
}
// MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM //
//                                                              //
// MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMI7OMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM7. IMMMMMMMMMMMI7IMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM7.?.7MMMMMMMM7  .7MMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM8....7MMMMMI...I7MMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMM:...=MMMM7 ...7MMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMM7=...ZMMI....+MMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMM77...IM7....:DMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMN?...77 ...=DMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMM7...I?...?MMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMM8.===~I?7NMMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMM7.?I ....7MMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMN,?7$?... IMMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM7..........=DMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM,........? .7MMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM7........=..7MMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMM=.?III?I:,7MMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM7...........OMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMI...........NMMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMM7............7MMMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMM:...?.........7MMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMM~...I..........$MMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMI...I..........IMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMI...I ..........IMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMM7 ..+?.......... 7MMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM7..?,...I.......7MMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMI .I...?.........7MMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM8 7..= ..I.......7MMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMM87 .7............,MMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMM+.I..............DMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMM8....... ........$MMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMO~......,.......=MMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMM7......7.......$MMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMM777II .... =.....7MMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMM$.=I7777777I...?=IMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMI7I$8MMMMMMMMMMMMMMMMMMMMMMMMM //
// MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM //
//                                                              //
// MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM //
