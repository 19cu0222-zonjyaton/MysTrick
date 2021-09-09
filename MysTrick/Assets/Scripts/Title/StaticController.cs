using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticController : MonoBehaviour
{
    //  共有プロパティ
    //  StageSelectConfirmUIController
    public static bool animIsOver;
    public static bool isCancel;
    public static Vector3 playerPos = new Vector3(-7.0f, -2.6f, -339.78f);
    public static Vector3 playerRot = new Vector3(0, 65, 0);

    //  ExitController
    public static bool exitPanelIsOpen;
    public static bool animIsStart;

    //  StageSelectButtonController
    public static string selectStageName = "Stage01";       //  選択したステージ名(シン―を切り替えてもstatic dataに影響しない)
    public static bool confirmMenuIsOpen;
    public static bool[] stageCanSelect = {true, true, true, true };

    //  GoalController
    public static string clearStageName = "";
    public static int[] getCount = new int[4];
    public static int[] highScore = new int[4];
    public static bool[] stageIsClear = new bool[4];
    public static bool[] stageIsFirstClear = {true, true, true, true};
}
