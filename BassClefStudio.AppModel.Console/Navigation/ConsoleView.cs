using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BassClefStudio.AppModel.Navigation
{
    /// <summary>
    /// Represents an abstract 'view' in a Console app.
    /// </summary>
    public interface IConsoleView
    {
        /// <summary>
        /// Show the view, calling <see cref="Console"/> methods and displaying text to the user. Ensure that this <see cref="Task"/> never completes without calling either <see cref="Lifecycle.App.Suspend"/> or navigating to a different <see cref="ConsoleView{T}"/>.
        /// </summary>
        /// <param name="parameter">An <see cref="object"/> parameter that can be passed to the <see cref="IConsoleView{T}"/> on navigation.</param>
        Task ShowView(object parameter);
    }

    /// <summary>
    /// A basic abstract implementation of the <see cref="IConsoleView"/> interface, with an attached <see cref="IViewModel"/>.
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="IViewModel"/> <see cref="ViewModel"/>.</typeparam>
    public abstract class ConsoleView<T> : IConsoleView, IView<T> where T : IViewModel
    {
        private Dictionary<string, TaskCompletionSource<bool>> CompletionTasks;

        /// <inheritdoc/>
        public T ViewModel { get; set; }

        /// <summary>
        /// Creates a new <see cref="ConsoleView{T}"/> and initializes helper methods and properties.
        /// </summary>
        public ConsoleView()
        {
            CompletionTasks = new Dictionary<string, TaskCompletionSource<bool>>();
        }

        /// <inheritdoc/>
        public virtual void Initialize()
        { }

        /// <inheritdoc/>
        public abstract Task ShowView(object parameter);

        /// <summary>
        /// Creates a new completion-task - an awaitable <see cref="Task"/> that can be completed at a later time (i.e. to advance the UI after a specific event).
        /// </summary>
        /// <param name="name">The unique name of the task.</param>
        protected void CreateTask(string name)
        {
            if(!CompletionTasks.ContainsKey(name))
            {
                CompletionTasks.Add(name, new TaskCompletionSource<bool>());
            }
            else
            {
                CompletionTasks[name] = new TaskCompletionSource<bool>();
            }
        }

        /// <summary>
        /// Awaits the completion of a given completion-task (see <see cref="CreateTask(string)"/>).
        /// </summary>
        /// <param name="name">The <see cref="string"/> name of the task.</param>
        /// <param name="removeAfter">A <see cref="bool"/> indicating whether the task should be removed after it is completed.</param>
        protected async Task<bool> AwaitTask(string name, bool removeAfter = true)
        {
            if(CompletionTasks.ContainsKey(name))
            {
                var result = await CompletionTasks[name].Task;
                if (removeAfter)
                {
                    CompletionTasks.Remove(name);
                }
                return result;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Attempts to complete the named completion-task (see <see cref="CreateTask(string)"/>).
        /// </summary>
        /// <param name="name">The unique name of the task.</param>
        /// <param name="result">Optionally, a <see cref="bool"/> result passed to the completion-task on <see cref="AwaitTask(string, bool)"/>.</param>
        /// <returns>A <see cref="bool"/> indicating if the operation succeeded.</returns>
        protected bool CompleteTask(string name, bool result = true)
        {
            if(CompletionTasks.ContainsKey(name))
            {
                return CompletionTasks[name].TrySetResult(result);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Writes to the <see cref="Console"/>, while managing the <see cref="ConsoleColor"/> of the text. Specifying 'null' will use default colors.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> foreground text color.</param>
        protected void Write(string text, ConsoleColor? color = null)
        {
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }
            else
            {
                Console.ResetColor();
            }
            Console.Write(text);
            Console.ResetColor();
        }

        /// <summary>
        /// Writes a full line to the <see cref="Console"/>, while managing the <see cref="ConsoleColor"/> of the text. Specifying 'null' will use default colors.
        /// </summary>
        /// <param name="text">The text to write.</param>
        /// <param name="color">The <see cref="ConsoleColor"/> foreground text color.</param>
        protected void WriteLine(string text, ConsoleColor? color = null)
        {
            if (color.HasValue)
            {
                Console.ForegroundColor = color.Value;
            }
            else
            {
                Console.ResetColor();
            }
            Console.WriteLine(text);
            Console.ResetColor();
        }
    }
}
