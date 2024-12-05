using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectHandller
{
    private int stageNum;

    public void SetStage(int stageNum)
    {
        this.stageNum = stageNum; 
    }

    public int GetStage()
    {
        return stageNum;
    }
}
