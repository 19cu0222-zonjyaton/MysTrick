using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticController : MonoBehaviour
{
    //  共有プロパティ
    //  StageSelectConfirmUIController
    public static bool animIsOver;
    public static bool isCancel;

    //  ExitController
    public static bool exitPanelIsOpen;
    public static bool animIsStart;

    //  StageSelectButtonController
    public static int[] imageIndex = new int[4];
    public static string selectStageName = "Stage01";       //  選択したステージ名(シン―を切り替えてもstatic dataに影響しない)
    public static bool confirmMenuIsOpen;

    //  GoalController
    public static string clearStageName = "";
    public static int[] getCount = new int[4];
}
