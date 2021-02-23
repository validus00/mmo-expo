using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SSWarpAnimations
{
	public class OverviewManager : MonoBehaviour {
		public Transform main_camera;
		public TextMesh ui_text;
		public int i_pos = 0;


		void Start () {
			Destroy(GameObject.Find("Instructions"), 14.5f);
		}
		

		void Update () {
			//////////////////
			//Keyboard Stuff..
			if ( Input.GetKeyDown("s") )
			{
				ScreenCapture.CaptureScreenshot("ScreenShot " + Time.time + ".png");
			}

			if ( Input.GetKeyDown(KeyCode.RightArrow) )
			{
				i_pos++;
				if(i_pos >= 8)
					i_pos = 0;
				SetScene();
			}

			if ( Input.GetKeyDown(KeyCode.LeftArrow) )
			{
				i_pos--;
				if(i_pos < 0)
					i_pos = 7;
				SetScene();
			}
			//Keyboard Stuff/////////////
			/////////////////////////////			
		}

		public void SetScene(){
			if(i_pos == 0){
				main_camera.position = new Vector3(0, 19.11f, -25.56f);
				ui_text.text = "Overview";
			}
			if(i_pos == 1){
				main_camera.position = new Vector3(30, 19.11f, -25.56f);
				ui_text.text = "Constellation";
			}
			if(i_pos == 2){
				main_camera.position = new Vector3(60, 19.11f, -25.56f);
				ui_text.text = "Tech/Sci-fi";
			}
			if(i_pos == 3){
				main_camera.position = new Vector3(90, 19.11f, -25.56f);
				ui_text.text = "Spooky/Spirits/Black Magic";
			}
			if(i_pos == 4){
				main_camera.position = new Vector3(120, 19.11f, -25.56f);
				ui_text.text = "Rainbow/Fun/Happy Place";
			}
			if(i_pos == 5){
				main_camera.position = new Vector3(150, 19.11f, -25.56f);
				ui_text.text = "Mystic/Magic";
			}
			if(i_pos == 6){
				main_camera.position = new Vector3(180, 19.11f, -25.56f);
				ui_text.text = "Generic/Standard 1";
			}
			if(i_pos == 7){
				main_camera.position = new Vector3(210, 19.11f, -25.56f);
				ui_text.text = "Generic/Standard 2";
			}
		}
	}	
}