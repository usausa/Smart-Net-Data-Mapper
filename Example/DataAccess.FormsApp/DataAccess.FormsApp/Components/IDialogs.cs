namespace DataAccess.FormsApp.Components
{
    using System.Threading.Tasks;

    public interface IDialogs
    {
        Task<bool> Confirm(string message, string title = null, string acceptButton = "OK", string cancelButton = "Cancel");

        Task Information(string message, string title = null, string cancelButton = "OK");
    }
}
