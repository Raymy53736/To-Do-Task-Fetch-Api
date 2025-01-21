using System.Collections;

//Interface that specify what exact function a script that is using it will have
public interface ITaskRepository 
{
    IEnumerator FetchTasks(string url);
    void AddNewTask(string userId, string Id, string Title, string Completed);
}
