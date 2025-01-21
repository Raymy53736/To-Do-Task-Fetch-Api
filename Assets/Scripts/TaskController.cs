using System.Collections;
using UnityEngine;

public class TaskController : MonoBehaviour
{
    // Reference to the TaskService where we can call its functions
    public TaskService taskService;

    // Sample URL where tasks are fetched from
    public string taskApiUrl = "https://jsonplaceholder.typicode.com/todos/";

    void Start()
    {
        // Initialize the TaskService reference
        if (taskService == null)
        {
            taskService = new TaskService();
        }

        // Fetch tasks from the API when the game starts
        StartCoroutine(FetchTasks());
    }

    // Function to fetch tasks from the server (calls TaskService's FetchTasks method)
    private IEnumerator FetchTasks()
    {
        Debug.Log("Fetching tasks from the server...");
        yield return StartCoroutine(taskService.FetchTasks(taskApiUrl));

        // After tasks are fetched, display each task's details in the console
        if (taskService.ChacedList != null && taskService.ChacedList.cacheditemList.Count > 0)
        {
            Debug.Log("Displaying all fetched tasks:");
            foreach (var task in taskService.ChacedList.cacheditemList)
            {
                Debug.Log($"Task ID: {task.id}, Title: {task.title}, Completed: {task.completed}");
            }
        }
        else
        {
            Debug.Log("No tasks found.");
        }
    }


    // Function to add a new task (can be triggered by a UI button or other events)
    public void AddNewTask(string userId, string taskId, string title, string completed)
    {
        taskService.AddNewTask(userId, taskId, title, completed);

        // After adding the task, we can confirm by logging the new task
        Debug.Log("New task added successfully.");
    }

    // Function to edit an existing task (can be triggered by a UI button or other events)
    public void EditTask(string taskId, string newTitle, string newCompletedStatus)
    {
        taskService.EditTask(taskId, newTitle, newCompletedStatus);

        // Log the edit action to confirm it's working
        Debug.Log($"Task with ID {taskId} edited successfully.");
    }

    // Sample UI Triggered function to simulate adding and editing tasks
    public void SimulateTaskActions()
    {
        // Add a new task (just for demonstration, values can come from UI inputs)
        AddNewTask("1", "11", "New Task 1", "false");

        // Edit a task (simulate editing an existing task)
        EditTask("11", "Updated Task 1", "true");
    }
}
