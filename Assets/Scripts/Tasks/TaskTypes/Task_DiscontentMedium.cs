using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_DiscontentMedium : GameTask
{
    public EResource ResourceType;
    public TaskCountComparison TaskCountComparison;
    public int TargetPecent;
    private float PercentNow;
    private string PercentStr;

    public Task_DiscontentMedium(TaskCountComparison taskCountComparison, int targetPecent)
    {
        TaskType = ETask.DiscontentMedium;
        TaskCountComparison = taskCountComparison;
        TargetPecent = targetPecent;
    }

    public override bool IsTaskCompleted()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return PercentNow < TargetPecent;
            case TaskCountComparison.MoreOrEqualTo:
                return PercentNow >= TargetPecent;
        }
        return false;
    }

    public override bool IsTaskFailed()
    {
        //switch (TaskCountComparison)
        //{
        //    case TaskCountComparison.LessThan:
        //        return PercentNow >= TargetPecent;
        //    case TaskCountComparison.MoreOrEqualTo:
        //        return PercentNow < TargetPecent;
        //}
        return false;
    }

    public void SetPercent(float percent)
    {
        PercentNow = percent;
        PercentStr = percent.ToString("0.0");
    }

    public override string GetTaskString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return $"Average discontent is no more than {TargetPecent}% (now {PercentStr}%)";
            case TaskCountComparison.MoreOrEqualTo:
                return $"Average discontent not less than {TargetPecent}% (now {PercentStr}%)";
        }
        return $"Average discontent is no more than {TargetPecent}% (now {PercentStr}%)";
    }
    public override string GetTaskInMenuString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return $"Average discontent is no more than {TargetPecent}%";
            case TaskCountComparison.MoreOrEqualTo:
                return $"Average discontent not less than {TargetPecent}%";
        }
        return $"Average discontent is no more than {TargetPecent}%";
    }
}
