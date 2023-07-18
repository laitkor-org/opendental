using System;
using System.Collections;
using System.Collections.Generic;

namespace OpenDental.UI{ 
	//Jordan is the only one allowed to edit this file.
	
	///<summary></summary>
	public class ListGridColumns:List<GridColumn>{
		///<summary>Gets the index of the column with the specified heading.</summary>
		public int GetIndex(string heading){
			for(int i=0;i<this.Count;i++){
				if(this[i].Heading==heading){
					return i;
				}
			}
			return -1;//not found
		}

	}
}





















