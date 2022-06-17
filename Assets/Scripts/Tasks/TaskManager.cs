using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public static TaskManager singleton;

    public static List<GameTask> MainTasks;
    public static List<GameTask> ExtraTasks;

    public static Dictionary<ETask, List<GameTask>> SortedTasks;
    public static List<TaskStatus> MainTasksStatus;
    public static List<TaskStatus> ExtraTasksStatus;
    static TasksDisplay TasksDisplay;

    private void Awake()
    {
        singleton = this;
        SortedTasks = new Dictionary<ETask, List<GameTask>>();
    }

    public void Init(List<GameTask> mainTasks, List<GameTask> extraTasks, TasksDisplay tasksDisplay)
    {
        SortedTasks = new Dictionary<ETask, List<GameTask>>();
        MainTasksStatus = new List<TaskStatus>();
        ExtraTasksStatus = new List<TaskStatus>();
        MainTasks = mainTasks;
        ExtraTasks = extraTasks;

        for (var i = 0; i < MainTasks.Count; i++)
        {
            var task = MainTasks[i];
            task.Index = i;
            task.IsMain = true;

            if (!SortedTasks.ContainsKey(task.TaskType))
                SortedTasks[task.TaskType] = new List<GameTask>();
            SortedTasks[task.TaskType].Add(task);

            MainTasksStatus.Add(TaskStatus.None);
        }

        for (var i = 0; i < ExtraTasks.Count; i++)
        {
            var task = ExtraTasks[i];
            task.Index = i;
            task.IsMain = false;

            if (!SortedTasks.ContainsKey(task.TaskType))
                SortedTasks[task.TaskType] = new List<GameTask>();
            SortedTasks[task.TaskType].Add(task);

            ExtraTasksStatus.Add(TaskStatus.None);
        }
        TasksDisplay = tasksDisplay;
        TasksDisplay.Init(MainTasks.Count, ExtraTasks.Count);
        UpdateAllTasks();
    }

    public static void UpdateAllTasks() {
        foreach (var e in SortedTasks.Keys)
        {
            UpdatedETask(e);
        }
    }

    public static void UpdatedETask(ETask eTask) {
        if (!SortedTasks.ContainsKey(eTask))
            return;
        foreach (var task in SortedTasks[eTask]) {
            UpdateTask(task);
        }
    }

    private static void UpdateTask(GameTask task)
    {
        switch (task.TaskType)
        {
            case ETask.ResourceCount:
                var t1 = (Task_ResourceCount)task;
                t1.CountNow = ResourceManager.singleton.GetResourceCount(t1.ResourceType);
                break;
            case ETask.Days:
                var t2 = (Task_Days)task;
                t2.CountNow = Ticker.singleton.Day;
                break;
            case ETask.DeadPercent:
                var t3 = (Task_DeadPercent)task;
                t3.SetPercent((TownManager.singleton.DeadPeopleCount / (float)TownManager.singleton.TotalPeopleCount) * 100);
                break;
            case ETask.InfectedPercent:
                var t4 = (Task_InfectedPercent)task;
                t4.SetPercent((TownManager.singleton.InfectedPeopleCount / (float)TownManager.singleton.TotalPeopleCount) * 100);
                break;
            case ETask.DiscontentMedium:
                var t5 = (Task_DiscontentMedium)task;
                t5.SetPercent((DiscontentManager.TotalDiscontent / (float)DiscontentManager.MaxDiscontent) * 100);
                break;
        }

        var isFailed = task.IsTaskFailed();
        var isCompleted = task.IsTaskCompleted();

        var color = (isFailed ? "red" : (isCompleted ? "#8AD68D" : "#ffffff"));
        var res = $"<color={color}>O  {task.GetTaskString()}</color>";

        if (task.IsMain)
        {
            TasksDisplay.UpdateMainTaskText(task.Index, res);
            MainTasksStatus[task.Index] = isFailed ? TaskStatus.Failed : (isCompleted ? TaskStatus.Completed : TaskStatus.None);

            switch (MainTasksStatus[task.Index]) {
                case TaskStatus.Failed:
                    GameFailed();
                    break;
                case TaskStatus.Completed:
                    CheckGameCompleted();
                    break;
            }
        }
        else
        {
            TasksDisplay.UpdateExtraTaskText(task.Index, res);
            ExtraTasksStatus[task.Index] = isFailed ? TaskStatus.Failed : (isCompleted ? TaskStatus.Completed : TaskStatus.None);
        }
    }

    private static void GameFailed()
    {
        GameManager.singleton.GameEnded(false);
    }

    private static void CheckGameCompleted() {
        foreach (var taskStatus in MainTasksStatus) {
            if (taskStatus != TaskStatus.Completed)
                return;
        }
        GameManager.singleton.GameEnded(true);
    }
}
