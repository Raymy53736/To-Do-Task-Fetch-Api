using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//Logic Class for the task services to ensure that task services respect the interface and hold the logic for it
public class TaskService : ITaskRepository
{
    // This is where we store the list of tasks fetched from the server (caching)
    public TaskChaced ChacedList=new TaskChaced();

    // This function fetches tasks from a URL
    public IEnumerator FetchTasks(string url)
    {
        // Start a web request to get data from the URL
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Wait for the request to finish
            yield return webRequest.SendWebRequest();

            // Check the result of the web request
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.InProgress:
                    // If the request is still in progress, log it 
                    Debug.Log("Request is still in progress.");
                    break;

                case UnityWebRequest.Result.Success:
                    // If the request was successful and we got the data
                    Debug.Log("Request successful!");
                    string jsonValue = webRequest.downloadHandler.text; // Get the data from the response

                    // Attempt to convert the JSON data into our list of tasks
                    try
                    {
                        // Parse the JSON and convert it into a list of TaskItem objects
                        ChacedList.cacheditemList = JsonUtility.FromJson<ListWrapper<TaskItem>>(WrapJsonArray(jsonValue)).items;
                        Debug.Log("Tasks successfully cached.");
                    }
                    catch (System.Exception ex)
                    {
                        // If there’s an error parsing the JSON, log the error message
                        Debug.LogError($"Error parsing JSON: {ex.Message}");
                    }
                    break;

                case UnityWebRequest.Result.ConnectionError:
                    // If there’s a connection error, log it
                    Debug.LogError($"Connection error: {webRequest.error}");
                    break;

                case UnityWebRequest.Result.ProtocolError:
                    // If the server returned an error, log it
                    Debug.LogError($"Protocol error: {webRequest.error} (HTTP Status: {webRequest.responseCode})");
                    break;

                case UnityWebRequest.Result.DataProcessingError:
                    // If there’s an issue processing the data, log the error
                    Debug.LogError($"Data processing error: {webRequest.error}");
                    break;

                default:
                    // If we get an unexpected result, log it
                    Debug.LogError("Unexpected error occurred.");
                    break;
            }
        }
    }

    // This function adds a new task to the list
    public void AddNewTask(string userId, string Id, string Title, string Completed)
    {
        // Create a new task with the provided details
        TaskItem newTask = new TaskItem
        {
            userId = userId,
            id = Id,
            title = Title,
            completed = Completed
        };

        // Add the new task to the list of tasks
        ChacedList.cacheditemList.Add(newTask);

        // Log the new task to confirm it was added
        Debug.Log($"New Task Added: {userId}, ID: {Id}, Title: {Title}, Completed: {Completed}");
    }

    // This function edits an existing task by its ID
    public void EditTask(string taskId, string newTitle, string newCompletedStatus)
    {
        // Find the task with the specified ID
        TaskItem taskToEdit = ChacedList.cacheditemList.Find(task => task.id == taskId);

        if (taskToEdit != null)
        {
            // If the task is found, update its title and completion status
            taskToEdit.title = newTitle;
            taskToEdit.completed = newCompletedStatus;

            // Log the task edit
            Debug.Log($"Task Edited: ID {taskId}, New Title: {newTitle}, New Completed: {newCompletedStatus}");
        }
        else
        {
            // If no task with the specified ID is found, log an error
            Debug.LogError($"Task with ID {taskId} not found.");
        }
    }

    // Helper class to wrap the JSON array because Unity's JsonUtility can’t handle raw arrays directly
    [System.Serializable]
    private class ListWrapper<T>
    {
        public List<T> items; // List of tasks or other items
    }

    // This function wraps the JSON array to fit Unity’s format for parsing
    private string WrapJsonArray(string json)
    {
        return "{\"items\":" + json + "}"; // Adds an "items" key around the array
    }
}
