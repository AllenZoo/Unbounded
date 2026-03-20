using UnityEngine;

/// <summary>
/// M = model
/// V = view
/// D = config/data (DTO intermediary between controller and view, usually created in controller layer)
/// </summary>
/// <typeparam name="M"></typeparam>
/// <typeparam name="V"></typeparam>
/// <typeparam name="D"></typeparam>
public interface IController<M, V, D> where M : IModel where V : IView<D> where D : class
{
    //// For connecting any events relevant to view.
    //void ConnectView(); // commented out since we cannot have private interface functions, but make sure all controllers follow this format!

    //// For connecting any events relevant to model.
    //void ConnectModel(); // commented out since we cannot have private interface functions, but make sure all controllers follow this format!

    //void Cleanup();

    //D CreateConfig(M model); // commented out since we cannot have private interface functions, but make sure all controllers follow this format!
}


// Example format.
//private void ConnectModel()
//{
//    // Subscribe to any relevant model events here.
//}

//private void ConnectView()
//{
//    // Subscribe to any relevant view events here.
//}

//public void Cleanup()
//{
//    // Unsubscribe from all events here.
//}