using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_Days : GameTask
{
    public EResource ResourceType;
    public TaskCountComparison TaskCountComparison;
    public int TargetCount;
    public int CountNow;

    public Task_Days(TaskCountComparison taskCountComparison, int targetCount)
    {
        TaskType = ETask.Days;
        TaskCountComparison = taskCountComparison;
        TargetCount = targetCount;
    }

    public override bool IsTaskCompleted()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return CountNow < TargetCount;
            case TaskCountComparison.MoreOrEqualTo:
                return CountNow >= TargetCount;
        }
        return false;
    }

    public override bool IsTaskFailed()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return CountNow >= TargetCount;
        }
        return false;
    }

    public override string GetTaskString()
    {
        switch (TaskCountComparison)
        {
            case TaskCountComparison.LessThan:
                return $"Complete the task in {TargetCount} days";
            case TaskCountComparison.MoreOrEqualTo:
                return $"Survive {TargetCount} days";
        }
        return $"Complete the task in {TargetCount} days";
    }

    public override string GetTaskInMenuString()
    {
        return GetTaskString();
    }
}
