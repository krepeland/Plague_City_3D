using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_DeadPercent : GameTask
{
    public EResource ResourceType;
    public TaskCountComparison TaskCountComparison;
    public int TargetPecent;
    private float PercentNow;
    private string PercentStr;

    public Task_DeadPercent(TaskCountComparison taskCountComparison, int targetPecent)
    {
        TaskType = ETask.DeadPercent;
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
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return PercentNow >= TargetPecent;
            case TaskCountComparison.MoreOrEqualTo:
                return PercentNow < TargetPecent;
        }
        return false;
    }

    public void SetPercent(float percent) {
        PercentNow = percent;
        PercentStr = percent.ToString("0.0");
    }

    public override string GetTaskString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return $"Less than {TargetPecent}% of people died (now {PercentStr}%)";
            case TaskCountComparison.MoreOrEqualTo:
                return $"At least {TargetPecent}% of people died (now {PercentStr}%)";
        }
        return $"Less than {TargetPecent}% of people died (now {PercentStr}%)";
    }

    public override string GetTaskInMenuString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return $"Less than {TargetPecent}% of people died";
            case TaskCountComparison.MoreOrEqualTo:
                return $"At least {TargetPecent}% of people died";
        }
        return $"Less than {TargetPecent}% of people died";
    }
}
