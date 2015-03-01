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
	public class GUIDragAndDrop
	{
		public static void DrawGUI<T>(Rect drop_area, Color backColor, string Content, T objRef, Action<T> action)
		{
			Event evt = Event.current;
			
			Color oldC = GUI.backgroundColor;
			GUI.backgroundColor = backColor;
			GUI.Box(drop_area, "");
			GUI.backgroundColor = oldC;
	     
	        switch (evt.type) {
		        case EventType.DragUpdated:
		        case EventType.DragPerform:
		            if (!drop_area.Contains (evt.mousePosition))
		                return;
		             
		            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
		         
		            if (evt.type == EventType.DragPerform) {
		                DragAndDrop.AcceptDrag ();
					
						action(objRef);
		            }
		            break;
	        }
		}
	}
}